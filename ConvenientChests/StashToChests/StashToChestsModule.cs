using System;
using System.Linq;
using ConvenientChests.Framework;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.ItemService;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using static ConvenientChests.StashToChests.Framework.StashLogic;
using Utility = ConvenientChests.Framework.Utility;

namespace ConvenientChests.StashToChests;

public class StashToChestsModule : Module
{
    internal InventoryDataManager InventoryDataManager { get; } = new();
    public AcceptingFunc AcceptingFunc { get; private set; }
    public RejectingFunc RejectingFunc { get; private set; }
    private bool IsStashToNearbyActive { get; set; }
    private bool IsStashAnyweherActive { get; set; }
    private static string PlayerName => Game1.player.Name;

    private bool InventoryLocksItem(Item item)
    {
        var itemKey = item.ToBase().ToItemKey();
        return InventoryDataManager.GetInventoryData(PlayerName).Locks(itemKey);
    }

    public StashToChestsModule(ModEntry modEntry) : base(modEntry)
    {
        modEntry.Helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    public override void Activate()
    {
        IsActive = true;
        RefreshConfig();
        // 初始化堆叠逻辑函数
        // Init the stack logic function.
        AcceptingFunc = CreateAcceptingFunction();
        RejectingFunc = CreateRejectingFunction();
    }

    public override void Deactivate()
    {
        IsActive = false;
        RefreshConfig();
        AcceptingFunc = (_, _) => false;
        RejectingFunc = _ => true;
    }

    /// <summary>
    /// 更新判断逻辑函数
    /// </summary>
    public void RefreshJudgementFunction()
    {
        AcceptingFunc = CreateAcceptingFunction();
        RejectingFunc = CreateRejectingFunction();
    }

    /// <summary>
    /// 根据配置选项设置将物品堆叠至箱子时的判断逻辑函数。
    /// Set the stack logic function based on modconfig.
    /// </summary>
    /// <returns>判断逻辑函数</returns>
    private AcceptingFunc CreateAcceptingFunction()
    {
        if (ModConfig.CategorizeChests && ModConfig.StashToExistingStacks)
            return (chest, item) => ModEntry.CategorizeChests.ChestAcceptsItem(chest, item) || chest.ContainsItem(item);

        if (ModConfig.CategorizeChests)
            return (chest, item) => ModEntry.CategorizeChests.ChestAcceptsItem(chest, item);

        if (ModConfig.StashToExistingStacks)
            return (chest, item) => chest.ContainsItem(item);

        return (_, _) => false;
    }

    private RejectingFunc CreateRejectingFunction()
    {
        if (ModConfig.NeverStashTools)
            return item => item is Tool || InventoryLocksItem(item);
        return InventoryLocksItem;
    }

    private void RefreshConfig()
    {
        IsStashToNearbyActive = ModConfig.StashToNearby;
        IsStashAnyweherActive = ModConfig.StashAnywhere;
    }

    /// <summary>
    /// 将物品堆叠至箱子中。
    /// Stash items to chest(s).
    /// </summary>
    private void StashToNearbyKeyPressed()
    {
        // 未获取到玩家位置信息，结束函数。
        // If cannot get the player's current location, stop.
        if (Game1.player.currentLocation == null)
            return;

        // 打开某个箱子时，将物品堆叠至当前的箱子。
        // While opening a chest, stash items into current chest.
        if (Game1.activeClickableMenu is ItemGrabMenu { context: Chest chest })
            StashToCurrentChest(chest, AcceptingFunc, RejectingFunc);

        // 未打开箱子时，将物品堆叠至附近的箱子。
        // While no chests is opening, stash items into nearby chests.
        else if (IsStashToNearbyActive)
            StashToNearbyChests(ModConfig.StashRadius, AcceptingFunc, RejectingFunc);

        var a = InventoryDataManager.GetInventoryData(PlayerName).LockedItemKinds.ToString();
        Console.WriteLine(a);
    }

    private void StashAnywhereKeyPressed()
    {
        // try to stash to fridge first
        var success = false;
        if (ModConfig.StashAnywhereToFridge && ChestExtension.GetFridge(Game1.player) is { } fridge)
            success |= StashToChest(fridge, AcceptingFunc, RejectingFunc);

        // try to find all chests by location
        if (Game1.player.currentLocation == null)
            return;

        var chests = Utility.GetLocations()
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
    /// 按下堆叠按钮或按键时，将物品堆叠至子里。
    /// When stash button or stash key is pressed, try stash items to nearby chests.
    /// </summary>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        switch (e.Button)
        {
            case var _ when e.Button == ModConfig.StashAnywhereKey:
                ModEntry.ReloadConfig(ModConfig.StashAnywhere, this);
                if (IsStashAnyweherActive) StashAnywhereKeyPressed();
                break;
            case var _ when e.Button == ModConfig.StashToNearbyKey || e.Button == ModConfig.StashButton:
                ModEntry.ReloadConfig(ModConfig.StashToNearby, this);
                StashToNearbyKeyPressed();
                break;
        }
    }
}