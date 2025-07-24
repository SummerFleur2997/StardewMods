using System;
using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.StashToChests.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using UI.UserInterface;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class ChestOverlay : Widget
{
    private readonly Chest _chest;
    private readonly ItemGrabMenu _itemGrabMenu;
    private TextButton CategorizeButton { get; set; }
    private TextButton StashButton { get; set; }
    private CategoryMenu CategoryMenu { get; set; }
    private TooltipManager TooltipManager { get; }

    private readonly InventoryMenu _inventoryMenu;
    private readonly InventoryMenu.highlightThisItem _defaultChestHighlighter;
    private readonly InventoryMenu.highlightThisItem _defaultInventoryHighlighter;

    /// <summary>
    /// 构造函数，初始化 ChestOverlay 类。
    /// Constructor to initialize the ChestOverlay class.
    /// </summary>
    public ChestOverlay(ItemGrabMenu menu, Chest chest)
    {
        _chest = chest;
        _itemGrabMenu = menu;
        _inventoryMenu = menu.ItemsToGrabMenu;
        TooltipManager = new TooltipManager();

        _defaultChestHighlighter = _itemGrabMenu.inventory.highlightMethod;
        _defaultInventoryHighlighter = _inventoryMenu.highlightMethod;

        if (_chest.SpecialChestType == Chest.SpecialChestTypes.Enricher) return;
        AddButtons();
    }

    /// <summary>
    /// 当父组件发生变化时调用。
    /// Called when the parent widget changes.
    /// </summary>
    protected override void OnParent(Widget parent)
    {
        base.OnParent(parent);

        if (parent == null) return;
        Width = parent.Width;
        Height = parent.Height;
    }

    /// <summary>
    /// 绘制界面元素。
    /// Draw the UI elements.
    /// </summary>
    public override void Draw(SpriteBatch batch)
    {
        base.Draw(batch);
        TooltipManager.Draw(batch);
    }

    /// <summary>
    /// 添加分类和堆叠按钮。
    /// Add categorize and stash buttons.
    /// </summary>
    private void AddButtons()
    {
        CategorizeButton = new TextButton(I18n.Button_Categorize(), Sprites.LeftProtrudingTab);
        CategorizeButton.OnPress += ToggleMenu;
        if (ModEntry.CategorizeModule.IsActive) AddChild(CategorizeButton);

        StashButton = new TextButton(I18n.Button_Stash(), Sprites.LeftProtrudingTab);
        StashButton.OnPress += StashItems;
        AddChild(StashButton);

        PositionButtons();
    }

    /// <summary>
    /// 计算分类按钮和堆叠按钮的偏移量。
    /// Calculate the offset of the categorize and stash buttons
    /// </summary>
    private int GetOffset()
    {
        return ModEntry.IsAndroid 
            ? 100 + ModEntry.Config.MobileOffset
            : _chest.SpecialChestType switch 
            {
                Chest.SpecialChestTypes.BigChest => 128,
                Chest.SpecialChestTypes.MiniShippingBin => 34,
                Chest.SpecialChestTypes.JunimoChest => 34,
                _ => _chest.Name switch
                {
                    "__Auto_!_Eats__" => 34,
                    _ => 112
                }
            };
    }

    /// <summary>
    /// 确定分类按钮和堆叠按钮的位置，使它们在箱子界面左侧对齐。
    /// Determine the position of the categorize and stash buttons to
    /// align them on the left side of the chest interface.
    /// </summary>
    private void PositionButtons()
    {
        var delta = GetOffset();
        StashButton.Width = CategorizeButton.Width = Math.Max(StashButton.Width, CategorizeButton.Width);

        CategorizeButton.Position = new Point(
            _itemGrabMenu.xPositionOnScreen + _itemGrabMenu.width / 2 - CategorizeButton.Width - delta * Game1.pixelZoom,
            _itemGrabMenu.yPositionOnScreen + 22 * Game1.pixelZoom);

        StashButton.Position = new Point(
            CategorizeButton.Position.X + CategorizeButton.Width - StashButton.Width,
            CategorizeButton.Position.Y + CategorizeButton.Height - 0);
    }

    /// <summary>
    /// 切换分类菜单的显示状态。
    /// Toggle the visibility of the category menu.
    /// </summary>
    private void ToggleMenu()
    {
        if (CategoryMenu == null)
            OpenCategoryMenu();

        else
            CloseCategoryMenu();
    }

    /// <summary>
    /// 打开分类菜单，同时按照配置文件决定分类列表排序方式。
    /// Open the category menu and sort the list of categories based on configuration settings.
    /// </summary>
    private void OpenCategoryMenu()
    {
        var chestData = _chest.GetChestData();
        CategoryMenu = new CategoryMenu(chestData, TooltipManager, _itemGrabMenu.width - 24);
        CategoryMenu.Position = new Point(
            _itemGrabMenu.xPositionOnScreen - GlobalBounds.X - 12,
            _itemGrabMenu.yPositionOnScreen - GlobalBounds.Y - 60);

        CategoryMenu.OnClose += CloseCategoryMenu;
        AddChild(CategoryMenu);

        SetItemsClickable(false);
    }

    /// <summary>
    /// 关闭分类菜单。
    /// Close the category menu.
    /// </summary>
    private void CloseCategoryMenu()
    {
        RemoveChild(CategoryMenu);
        CategoryMenu = null;

        SetItemsClickable(true);
    }

    /// <summary>
    /// 将物品堆叠到当前箱子中。
    /// Stash items into the current chest.
    /// </summary>
    private void StashItems()
    {
        var stashModule = ModEntry.StashModule;
        StashLogic.StashToCurrentChest(_chest, stashModule.AcceptingFunc, stashModule.RejectingFunc);
    }

    /// <summary>
    /// 处理鼠标左键点击事件。
    /// Handle left mouse click events.
    /// </summary>
    public override bool ReceiveLeftClick(Point point)
    {
        var hit = PropagateLeftClick(point);
        if (!hit && CategoryMenu != null)
            // 如果点击菜单外部，尝试关闭菜单。
            // If clicking outside the menu, try to close it.
            CloseCategoryMenu();
        return hit;
    }

    /// <summary>
    /// 设置物品是否可点击。
    /// Set whether items are clickable.
    /// </summary>
    private void SetItemsClickable(bool clickable)
    {
        if (clickable)
        {
            _itemGrabMenu.inventory.highlightMethod = _defaultChestHighlighter;
            _inventoryMenu.highlightMethod = _defaultInventoryHighlighter;
        }
        else
        {
            _itemGrabMenu.inventory.highlightMethod = _ => false;
            _inventoryMenu.highlightMethod = _ => false;
        }
    }
}