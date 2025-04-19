using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests.StashToChests;

public class StashToNearbyChestsModule : Module 
{
    public StackLogic.AcceptingFunction AcceptingFunction { get; private set; }

    public StashToNearbyChestsModule(ModEntry modEntry) : base(modEntry) {}

    /// <summary>
    /// 启用模组时的操作。
    /// What to do when the mod is activated.
    /// </summary>
    public override void Activate() 
    {
        IsActive = true;
        // 初始化堆叠逻辑函数并绑定按键事件
        // Init the stack logic function and button press event.
        AcceptingFunction = CreateAcceptingFunction();
        Events.Input.ButtonPressed += OnButtonPressed;
    }
    
    /// <summary>
    /// 禁用模组时的操作。
    /// What to do when the mod is deactivated.
    /// </summary>
    public override void Deactivate() 
    {
        IsActive = false;
        // 注销堆叠逻辑函数与按键事件
        // Cancel the stack logic function and button press event.
        AcceptingFunction = (_, _) => false;
        Events.Input.ButtonPressed -= OnButtonPressed;
    }

    /// <summary>
    /// 根据配置选项设置将物品堆叠至箱子时的判断逻辑函数。
    /// Set the stack logic function based on modconfig.
    /// </summary>
    /// <returns>判断逻辑函数</returns>
    private StackLogic.AcceptingFunction CreateAcceptingFunction() 
    {
        if (ModConfig.CategorizeChests && ModConfig.StashToExistingStacks)
            return (chest, item) => ModEntry.CategorizeChests.ChestAcceptsItem(chest, item) || chest.ContainsItem(item);

        if (ModConfig.CategorizeChests)
            return (chest, item) => ModEntry.CategorizeChests.ChestAcceptsItem(chest, item);

        if (ModConfig.StashToExistingStacks)
            return (chest, item) => chest.ContainsItem(item);

        return (_, _) => false;
    }

    /// <summary>
    /// 将物品堆叠至箱子中。
    /// Stash items to chest(s).
    /// </summary>
    private void TryStashNearby() 
    {
        // 未获取到玩家位置信息，结束函数。
        // If cannot get the player's current location, stop.
        if (Game1.player.currentLocation == null)
            return;

        // 打开某个箱子时，将物品堆叠至当前的箱子。
        // While opening a chest, stash items into current chest.
        if (Game1.activeClickableMenu is ItemGrabMenu { context: Chest c }) 
        {
            ModEntry.Log("Stash to current chest");
            StackLogic.StashToChest(c, AcceptingFunction);
        }

        // 未打开箱子时，将物品堆叠至附近的箱子。
        // While no chests is opening, stash items into nearby chests.
        else 
        {
            ModEntry.Log("Stash to nearby chests");
            StackLogic.StashToNearbyChests(ModConfig.StashRadius, AcceptingFunction);
        }
    }

    /// <summary>
    /// 按下堆叠按钮或按键时，将物品堆叠至附近的箱子里。
    /// When stash button or stash key is pressed, try stash items to nearby chests.
    /// </summary>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e) 
    {
        if (e.Button != ModConfig.StashKey && e.Button != ModConfig.StashButton) return;
        ModEntry.ReloadConfig(ModConfig.StashToNearbyChests, this);
        TryStashNearby();
    }
}