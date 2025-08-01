using System.Collections.Generic;
using System.Linq;
using Common;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.ItemService;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using static ConvenientChests.StashToChests.Framework.StashLogic;

namespace ConvenientChests.StashToChests;

internal class StashToChestsModule : IModule
{
    public bool IsActive { get; private set; }

    /// <summary>
    /// 判断箱子接受物品的函数。
    /// The function to determine whether the chest accepts the item.
    /// </summary>
    public AcceptingFunc AcceptingFunc { get; private set; }

    /// <summary>
    /// 判断箱子拒绝物品的函数。
    /// The function to determine whether the chest rejects the item.
    /// </summary>
    public RejectingFunc RejectingFunc { get; private set; }

    /// <summary>
    /// 存储至附近的箱子功能是否启用。
    /// Whether stash to nearby function is enabled.
    /// </summary>
    private bool IsStashToNearbyActive { get; set; }

    /// <summary>
    /// 存储至附近的箱子功能是否启用。
    /// Whether stash anywhere function is enabled.
    /// </summary>
    private bool IsStashAnywhereActive { get; set; }

    public StashToChestsModule()
    {
        ModEntry.ModHelper.Events.Input.ButtonPressed += OnButtonPressed;
        ModEntry.ModHelper.Events.GameLoop.TimeChanged += OnTimeChanged;
    }

    public void Activate()
    {
        IsActive = true;
        RefreshConfig();
        // 初始化存储逻辑函数
        // Init the stack logic function.
        CreateJudgementFunction();
    }

    public void Deactivate()
    {
        IsActive = false;
        RefreshConfig();
        AcceptingFunc = (_, _) => false;
        RejectingFunc = _ => true;
    }

    /// <summary>
    /// 创建/更新判断逻辑函数
    /// Create/Update the judgement function.
    /// </summary>
    public void CreateJudgementFunction()
    {
        AcceptingFunc = CreateAcceptingFunction();
        RejectingFunc = CreateRejectingFunction();
    }

    /// <summary>
    /// 判断给定的箱子是否接受给定的物品。
    /// Whether the given item is accepted by the given chest. 
    /// <seealso cref="CreateAcceptingFunction"/>
    /// </summary>
    private static bool ChestAcceptsItem(Chest chest, Item item)
    {
        var itemKey = item.ToBase().ToItemKey();
        return chest.GetChestData().Accepts(itemKey);
    }

    /// <summary>
    /// 判断给定的箱子内是否包含给定的物品。
    /// Whether the given item is contained in the given chest.
    /// <seealso cref="CreateAcceptingFunction"/>
    /// </summary>
    private static bool ChestContainsItem(Chest chest, Item item)
    {
        return chest.Items.Any(item.canStackWith);
    }

    /// <summary>
    /// 判断给定的物品是否在当前玩家的背包中被锁定。
    /// Whether the given item is locked in the current player's inventory.
    /// <seealso cref="CreateRejectingFunction"/>
    /// </summary>
    private static bool InventoryLocksItem(Item item)
    {
        var itemKey = item.ToBase().ToItemKey();
        return InventoryManager.GetInventoryData(Game1.player).Locks(itemKey);
    }

    /// <summary>
    /// 根据配置选项设置将物品存储至箱子时的接受物品判断函数。
    /// Set the accepting function based on modconfig.
    /// </summary>
    /// <returns>接受物品判断逻辑函数 Accepting function to determine whether the chest accepts the item</returns>
    private static AcceptingFunc CreateAcceptingFunction()
    {
        return (ModEntry.Config.CategorizeChests, ModEntry.Config.StashToExistingStacks) switch
        {
            (true, true)
                => (chest, item) => ChestAcceptsItem(chest, item) || ChestContainsItem(chest, item),
            (true, false)
                => ChestAcceptsItem,
            (false, true)
                => ChestContainsItem,
            (false, false)
                => (_, _) => false
        };
    }

    /// <summary>
    /// 根据配置选项设置将物品存储至箱子时的拒绝物品判断函数。
    /// Set the rejecting function based on modconfig.
    /// </summary>
    /// <returns>拒绝物品判断逻辑函数 Rejecting function to determine whether the chest rejects the item</returns>
    private static RejectingFunc CreateRejectingFunction()
    {
        if (ModEntry.Config.NeverStashTools)
            return item => item is Tool || InventoryLocksItem(item);
        return InventoryLocksItem;
    }

    private void RefreshConfig()
    {
        IsStashToNearbyActive = ModEntry.Config.StashToNearby;
        IsStashAnywhereActive = ModEntry.Config.StashAnywhere;
    }

    /// <summary>
    /// Get all game locations.
    /// </summary>
    private static IEnumerable<GameLocation> GetLocations()
    {
        return Game1.locations
            .Concat(
                from location in Game1.locations
                where location is not null
                from building in location.buildings
                where building.indoors.Value != null
                select building.indoors.Value
            );
    }

    /// <summary>
    /// 将物品存储至箱子中。
    /// Stash items to nearby chest(s).
    /// </summary>
    private void StashToNearby()
    {
        // 未获取到玩家位置信息，结束函数。
        // If unable to get the player's current location, stop.
        if (Game1.player.currentLocation == null)
            return;

        // 打开某个箱子时，将物品存储至当前的箱子。
        // While opening a chest, stash items into the current chest.
        if (Game1.activeClickableMenu is ItemGrabMenu { context: Chest chest })
            StashToCurrentChest(chest, AcceptingFunc, RejectingFunc);

        // 未打开箱子时，将物品存储至附近的箱子。
        // While no chests is opening, stash items into nearby chests.
        else if (IsStashToNearbyActive)
            StashToNearbyChests(ModEntry.Config.StashRadius, AcceptingFunc, RejectingFunc);
    }

    /// <summary>
    /// 将物品存储至任意位置箱子中。
    /// Stash items to chest(s) anywhere.
    /// </summary>
    private void StashAnywhere()
    {
        // try to stash to the fridge first
        var success = false;
        if (ModEntry.Config.StashAnywhereToFridge && Game1.player.GetFridge() is { } fridge)
            success |= StashToChest(fridge, AcceptingFunc, RejectingFunc);

        // try to find all chests by location
        if (Game1.player.currentLocation == null)
            return;

        var chests = GetLocations()
            .SelectMany(location => location.Objects.Pairs)
            .Select(pair => pair.Value)
            .OfType<Chest>()
            .ToList();

        // stash by category
        success |= StashToGivenChests(chests, AcceptingFunc, RejectingFunc);

        if (success)
            Game1.playSound(StashCueName);

        ModEntry.Log("Stash to anywhere");
    }

    /// <summary>
    /// 按下存储按钮或按键时，将物品存储至子里。
    /// When stash button or stash key is pressed, try stash items to nearby chests.
    /// </summary>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (ModEntry.Config.StashAnywhereKey.JustPressed() && IsStashAnywhereActive)
            StashAnywhere();

        if (ModEntry.Config.StashToNearbyKey.JustPressed() || e.Button == ModEntry.Config.StashButton)
            StashToNearby();
    }

    private void OnTimeChanged(object sender, TimeChangedEventArgs e)
    {
        var config = ModEntry.Config;
        if (e.NewTime % 100 % 30 != 0) return;
        switch (Game1.player.currentLocation)
        {
            case MineShaft { mineLevel: > 0 and <= 120 and not (20 or 60 or 100) } when config.AutoStashInTheMine:
            case MineShaft { mineLevel: > 120 and not 77377 } when config.AutoStashInSkullCavern:
            case VolcanoDungeon when config.AutoStashInVolcanoDungeon:
                StashAnywhere();
                break;
        }
    }
}