using System.Reflection.Emit;
using Common;
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using ConvenientChests.Framework.IntegrationService;
using ConvenientChests.Framework.UserInterfaceService;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using static ConvenientChests.StashToChests.Framework.StashLogic;

namespace ConvenientChests.StashToChests;

internal class StashToChestsModule : IModule
{
    /// <summary>
    /// Static singleton.
    /// </summary>
    public static readonly StashToChestsModule Instance = new();

    /// <inheritdoc />
    public bool IsActive { get; private set; }

    /// <summary>
    /// 判断箱子接受物品的函数。
    /// The function to determine whether the chest accepts the item.
    /// </summary>
    public AcceptingFunc AcceptingFunc { get; private set; } = (_, _) => false;

    /// <summary>
    /// 判断箱子拒绝物品的函数。
    /// The function to determine whether the chest rejects the item.
    /// </summary>
    public RejectingFunc RejectingFunc { get; private set; } = _ => false;

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

    private StashToChestsModule() { }

    /// <inheritdoc />
    public void Activate()
    {
        IsActive = true;
        RefreshConfig();
        // 初始化存储逻辑函数
        // Init the stack logic function.
        CreateJudgementFunction();
        ModEntry.ModHelper.Events.Input.ButtonPressed += OnButtonPressed;
        ModEntry.ModHelper.Events.GameLoop.TimeChanged += OnTimeChanged;
        ModEntry.ModHelper.Events.Player.InventoryChanged += OnInventoryChanged;
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        IsActive = false;
        RefreshConfig();
        AcceptingFunc = (_, _) => false;
        RejectingFunc = _ => true;
        ModEntry.ModHelper.Events.Input.ButtonPressed -= OnButtonPressed;
        ModEntry.ModHelper.Events.GameLoop.TimeChanged -= OnTimeChanged;
        ModEntry.ModHelper.Events.Player.InventoryChanged -= OnInventoryChanged;
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
    /// Harmony register for <see cref="Patch_ItemGrabMenu_FillOutStacks"/>
    /// </summary>
    public static void RegisterHarmonyPatch()
    {
        try
        {
            var harmony = new Harmony(ModEntry.Manifest.UniqueID);
            var original = AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.FillOutStacks));
            var transpiler = AccessTools.Method(
                typeof(StashToChestsModule), nameof(Patch_ItemGrabMenu_FillOutStacks));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched ItemGrabMenu.FillOutStacks successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for lock items: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Patch to <see cref="ItemGrabMenu.FillOutStacks"/> to make it
    /// possible to lock items when using vanilla stash button.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_ItemGrabMenu_FillOutStacks(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Callvirt),
            new(i => i.opcode == OpCodes.Stloc_S && i.operand is LocalBuilder { LocalIndex: 5 }),
            new(OpCodes.Ldloc_S),
            new(OpCodes.Brfalse) // if false: continue, else: process
        };
        matcher.MatchStartForward(target).Advance(3);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new CodeInstruction(OpCodes.Call,
            AccessTools.Method(typeof(StashToChestsModule), nameof(ShouldDealThisItem)));
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }

    /// <summary>
    /// Whether the given item should be dealt in advance.
    /// </summary>
    /// <returns>True if the item should be stashed, false otherwise.</returns>
    /// <remarks>The original il code below this method is BrFalse.</remarks>
    private static bool ShouldDealThisItem(Item? item)
    {
        // return false means to deal next item
        if (item is null)
            return false;

        if (!Instance.IsActive)
            return true;

        return !InventoryLocksItem(item);
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
        var locked = item.LockedInInventory();

        if (ConvenientInventoryIntegration.CIApi is null)
            return locked;

        var itemIndex = Game1.player.Items.IndexOf(item);
        return locked || ConvenientInventoryIntegration.CIApi.IsFavouriteItem(itemIndex);
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
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (MenuManager.ScreenWidgetHost.Value is
            MenuHost<ItemGrabMenu> { Overlay: ChestOverlay { AliasMenu: not null } })
            return;

        if (ModEntry.Config.StashAnywhereKey.JustPressed() && IsStashAnywhereActive)
            StashAnywhere();

        if (ModEntry.Config.StashToNearbyKey.JustPressed())
            StashToNearby();
    }

    /// <summary>
    /// Auto stash in the mine or skull cave every 30 minutes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
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

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        var id = ModEntry.Manifest.UniqueID;
        foreach (var item in e.Removed)
        {
            item.modData.Remove(id);
        }
    }
}