using System;
using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.Framework.UserInterfaceService;
using ConvenientChests.StashToChests.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using UI.Component;

namespace ConvenientChests.Framework.ChestService;

internal class ChestSideTab : IOverlay<ItemGrabMenu>
{
    private readonly Chest _chest;
    public ItemGrabMenu RootMenu { get; }
    private TextButton CategorizeButton { get; set; }
    private TextButton StashButton { get; set; }

    /// <summary>
    /// 构造函数，初始化 ChestOverlay 类。
    /// Constructor to initialize the ChestOverlay class.
    /// </summary>
    public ChestSideTab(ItemGrabMenu menu, Chest chest)
    {
        _chest = chest;
        RootMenu = menu;

        if (_chest.SpecialChestType == Chest.SpecialChestTypes.Enricher) return;
        AddAndPositionButtons();
    }

    /// <summary>
    /// 绘制界面元素。
    /// Draw the UI elements.
    /// </summary>
    public void Draw(SpriteBatch b)
    {
        if (ModEntry.CategorizeModule.IsActive)
            CategorizeButton?.Draw(b);
        StashButton?.Draw(b);
        RootMenu.drawMouse(b);
    }

    /// <summary>
    /// 添加分类和存储按钮，然后确定它们的位置，使它们在箱子界面左侧对齐。
    /// Add categorize and stash buttons. Then determine their position to
    /// align them on the left side of the chest interface.
    /// </summary>
    private void AddAndPositionButtons()
    {
        var padding = NineSlice.LeftProtrudingTab().TopBorderThickness;

        CategorizeButton = new TextButton(NineSlice.LeftProtrudingTab(), I18n.Button_Categorize(),
            Color.Black, Game1.smallFont, padding: padding);
        CategorizeButton.OnPress += OpenCategoryMenu;

        StashButton = new TextButton(NineSlice.LeftProtrudingTab(), I18n.Button_Stash(),
            Color.Black, Game1.smallFont, padding: padding);
        StashButton.OnPress += StashItems;

        // Calculate the offset based on the chest size.
        var delta = ModEntry.IsAndroid
            // For android, use a fixed offset.
            ? 100 + ModEntry.Config.MobileOffset
            // Otherwise, use a dynamic offset based on the chest size.
            : _chest.GetActualCapacity() switch
            {
                // Big chests, >=70 to compatible with unlimited storage
                >= 70 => 128,
                // Junimo chests / Shipping bin 
                9 => 34,
                // Common chests
                _ => 112
            };

        StashButton.Width = CategorizeButton.Width = Math.Max(StashButton.Width, CategorizeButton.Width);

        CategorizeButton.SetPosition(
            RootMenu.xPositionOnScreen + RootMenu.width / 2 - CategorizeButton.Width - delta * Game1.pixelZoom,
            RootMenu.yPositionOnScreen + 22 * Game1.pixelZoom);

        StashButton.SetPosition(
            CategorizeButton.X,
            CategorizeButton.Y + CategorizeButton.Height + 4 * Game1.pixelZoom);

        CategorizeButton.Label.OffsetPosition(Game1.pixelZoom * 2);
        StashButton.Label.OffsetPosition(Game1.pixelZoom * 2);
    }

    /// <summary>
    /// 打开分类菜单，同时按照配置文件决定分类列表排序方式。
    /// Open the category menu and sort the list of categories based on configuration settings.
    /// </summary>
    private void OpenCategoryMenu()
    {
        var data = _chest.GetChestData();
        var delta = ModEntry.IsAndroid ? 70 : 0;
        var menu = new CategoryMenu(RootMenu.xPositionOnScreen, RootMenu.yPositionOnScreen + delta,
            RootMenu.width, RootMenu.height - delta, data, IClickableMenu.borderWidth);
        menu.exitFunction = ExitFunction;
        Game1.activeClickableMenu = menu;
        return;

        void ExitFunction() => Game1.activeClickableMenu = RootMenu;
    }

    /// <summary>
    /// 将物品存储到当前箱子中。
    /// Stash items into the current chest.
    /// </summary>
    private void StashItems()
    {
        var stashModule = ModEntry.StashModule;
        StashLogic.StashToCurrentChest(_chest, stashModule.AcceptingFunc, stashModule.RejectingFunc);
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        if (ModEntry.CategorizeModule.IsActive && CategorizeButton.Contains(x, y))
            return CategorizeButton.ReceiveLeftClick(x, y);
        if (StashButton.Contains(x, y))
            return StashButton.ReceiveLeftClick(x, y);
        return false;
    }

    public bool ReceiveCursorHover(int x, int y) => false;
}