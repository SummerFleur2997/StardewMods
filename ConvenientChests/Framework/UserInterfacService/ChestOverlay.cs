using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.CategorizeChests;
using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.StashToChests;
using ConvenientChests.StashToChests.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests.Framework.UserInterfacService;

internal class ChestOverlay : Widget
{
    private ItemGrabMenu ItemGrabMenu { get; }
    private CategorizeChestsModule CategorizeModule { get; }
    private StashToChestsModule StashModule { get; }
    private Chest Chest { get; }
    private TooltipManager TooltipManager { get; }

    private readonly InventoryMenu _inventoryMenu;
    private readonly InventoryMenu.highlightThisItem _defaultChestHighlighter;
    private readonly InventoryMenu.highlightThisItem _defaultInventoryHighlighter;

    private TextButton CategorizeButton { get; set; }
    private TextButton StashButton { get; set; }
    private CategoryMenu CategoryMenu { get; set; }

    private bool ShouldAddCategoryKey { get; }

    /// <summary>
    /// 构造函数，初始化 ChestOverlay 类。
    /// Constructor to initialize the ChestOverlay class.
    /// </summary>
    public ChestOverlay(CategorizeChestsModule categorizeModule, StashToChestsModule stashModule, Chest chest,
        ItemGrabMenu menu, bool shouldAddCategoryKey)
    {
        CategorizeModule = categorizeModule;
        StashModule = stashModule;

        Chest = chest;
        ItemGrabMenu = menu;
        _inventoryMenu = menu.ItemsToGrabMenu;
        TooltipManager = new TooltipManager();

        _defaultChestHighlighter = ItemGrabMenu.inventory.highlightMethod;
        _defaultInventoryHighlighter = _inventoryMenu.highlightMethod;

        ShouldAddCategoryKey = shouldAddCategoryKey;
        if (Chest.SpecialChestType == Chest.SpecialChestTypes.Enricher) return;
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
        if (ShouldAddCategoryKey) AddChild(CategorizeButton);

        StashButton = new TextButton(ChooseStashButtonLabel(), Sprites.LeftProtrudingTab);
        StashButton.OnPress += StashItems;
        AddChild(StashButton);

        PositionButtons();
    }

    /// <summary>
    /// 确定分类按钮和堆叠按钮的位置，使它们在箱子界面左侧对齐。
    /// Determine the position of the categorize and stash buttons to
    /// align them on the left side of the chest interface.
    /// </summary>
    private void PositionButtons()
    {
        var delta = Chest.SpecialChestType switch
        {
            Chest.SpecialChestTypes.BigChest => -128,
            Chest.SpecialChestTypes.MiniShippingBin => -34,
            Chest.SpecialChestTypes.JunimoChest => -34,
            _ => Chest.Name switch
            {
                "__Auto_!_Eats__" => -34,
                _ => -112
            }
        };

        StashButton.Width = CategorizeButton.Width = Math.Max(StashButton.Width, CategorizeButton.Width);

        CategorizeButton.Position = new Point(
            ItemGrabMenu.xPositionOnScreen + ItemGrabMenu.width / 2 - CategorizeButton.Width + delta * Game1.pixelZoom,
            ItemGrabMenu.yPositionOnScreen + 22 * Game1.pixelZoom);

        StashButton.Position = new Point(
            CategorizeButton.Position.X + CategorizeButton.Width - StashButton.Width,
            CategorizeButton.Position.Y + CategorizeButton.Height - 0);
    }

    /// <summary>
    /// 选择堆叠按钮的标签文本。
    /// Choose the label text for the stash button.
    /// </summary>
    private string ChooseStashButtonLabel()
    {
        return CategorizeModule.ModConfig.StashToNearbyKey == SButton.None || StashModule.ModConfig.StashToNearby
            ? I18n.Button_Stash()
            : I18n.Button_Stash() + $" ({CategorizeModule.ModConfig.StashToNearbyKey})";
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
        var chestData = CategorizeModule.ChestManager.GetChestData(Chest);
        CategoryMenu = new CategoryMenu(chestData, CategorizeModule.CategoryDataManager, TooltipManager,
            ItemGrabMenu.width - 24);
        CategoryMenu.Position = new Point(
            ItemGrabMenu.xPositionOnScreen - GlobalBounds.X - 12,
            ItemGrabMenu.yPositionOnScreen - GlobalBounds.Y - 60);

        // 根据配置文件决定列表排序方式
        // Determine list sorting method based on configuration settings
        if (CategorizeModule.ModConfig.EnableSort)
        {
            // 按字母顺序排序
            // Sort in alphabetical order
            CategoryMenu.Categories = CategoryMenu.Categories
                .OrderBy(c => c.CategoryBaseName)
                .ToList();
        }
        else
        {
            // 定义自定义排序顺序的基准名称列表
            // Define custom sorting order using base names
            var customOrder = new List<string>
            {
                "Vegetable", "Fruit", "Flower", "Animal Product", "Artisan Goods", "Seed", "Fertilizer",
                "Fish", "Bait", "Fishing Tackle", "Crafting", "Machine", "Book", "Skill Book", "Trash",
                "Tool", "Weapons", "Ring", "Hats", "Shirts", "Pants", "Footwear", "Mannequin", "Decor",
                "Wallpaper", "Flooring", "Consumable", "Cooking", "Miscellaneous", "Trinket", "Monster Loot",
                "Artifact", "Mineral", "Resource", "Forage"
            };

            // 创建基准名称到排序索引的字典
            // Create lookup dictionary: base name -> predefined sorting index
            var orderDictionary = customOrder
                .Select((name, index) => new { name, index })
                .ToDictionary(item => item.name, item => item.index);

            // 根据自定义顺序排序
            // Sort in custom rules
            CategoryMenu.Categories = CategoryMenu.Categories
                .OrderBy(c => orderDictionary.GetValueOrDefault(c.CategoryBaseName, int.MaxValue))
                .ToList();
        }

        // 刷新类别列表以应用更改
        // Refresh category menu to apply changes
        CategoryMenu.RefreshMenu();

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
        StashLogic.StashToCurrentChest(Chest, StashModule.AcceptingFunc, StashModule.RejectingFunc);
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
            ItemGrabMenu.inventory.highlightMethod = _defaultChestHighlighter;
            _inventoryMenu.highlightMethod = _defaultInventoryHighlighter;
        }
        else
        {
            ItemGrabMenu.inventory.highlightMethod = i => false;
            _inventoryMenu.highlightMethod = i => false;
        }
    }
}