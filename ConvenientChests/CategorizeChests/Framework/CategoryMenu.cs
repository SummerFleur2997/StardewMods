using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.ItemService;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Menu;
using UI.Sprite;

namespace ConvenientChests.CategorizeChests.Framework;

internal class CategoryMenu : BaseMenu
{
    private const int TopRowHeight = 60;

    /// <summary>
    /// Used to compatible with LookupAnything.
    /// </summary>
    [UsedImplicitly] public Item HoveredItem;

    private readonly ChestData _chestData;
    private TopRow _topRow;
    private GridMenu _gridMenu;
    private Tooltip _tooltip;

    private ItemCategoryName ActiveCategory => _topRow.CategorySelector.SelectedValue;
    private static bool IsAndroid => ModEntry.IsAndroid;

    public CategoryMenu(int x, int y, int width, int height, ChestData chestData, int padding = 0)
        : base(x, y, width, height)
    {
        _chestData = chestData;

        BuildWidgets(padding);
        BuildCategories();
        RecreateItemToggles();
    }

    private void BuildWidgets(int padding)
    {
        var maxColumns = IsAndroid ? 99 : 12;
        var delta = IsAndroid ? 60 : 0;
        _topRow = new TopRow(
            xPositionOnScreen,
            yPositionOnScreen - TopRowHeight,
            width, TopRowHeight, height);
        _gridMenu = new GridMenu(
            xPositionOnScreen + padding,
            yPositionOnScreen + padding,
            width - padding * 2 - delta,
            height - padding * 2,
            66, maxColumns);

        _topRow.NextButton.OnPress += SelectNext;
        _topRow.PrevButton.OnPress += SelectPrev;
        _topRow.SelectAllButton.OnToggle += OnToggleSelectAll;
        _topRow.CategorySelector.OnSelectionChanged += OnSelectChanged;

        // The click handler logic in BaseMenu will traverse the children in
        // reverse order, so we need to add the grid menu in front of the top
        // row to ensure that the drop-down in the top row is handled first.
        AddChild(_gridMenu);
        AddChild(_topRow);
    }

    private void BuildCategories()
    {
        var categories = CategoryDataManager.ItemCategories;

        // 根据配置文件决定列表排序方式
        // Determine list sorting method based on configuration settings
        if (ModEntry.Config.EnableSort)
        {
            // 按字母顺序排序
            // Sort in alphabetical order
            categories = categories
                .OrderBy(c => c.CategoryDisplayName)
                .ToList();
        }
        else
        {
            // 定义自定义排序顺序的基准名称列表
            // Define custom sorting order using base names
            var customOrder = new List<string>
            {
                "Vegetable", "Fruit", "Flower", "Animal Product", "Artisan Goods", "Seed", "Fertilizer", "Fish",
                "Bait", "Fishing Tackle", "Forage", "Artifact", "Resource", "Mineral", "Monster Loot", "Crafting",
                "Machine", "BigCrafts", "Cooking", "Consumable", "Book", "Skill Book", "Tool", "Weapons", "Ring",
                "Trinket", "Hats", "Shirts", "Pants", "Footwear", "Mannequin", "Decor", "Wallpaper", "Flooring",
                "Trash", "Miscellaneous"
            };

            // 创建基准名称到排序索引的字典
            // Create lookup dictionary: base name -> predefined sorting index
            var orderDictionary = customOrder
                .Select((name, index) => new { name, index })
                .ToDictionary(item => item.name, item => item.index);

            // 根据自定义顺序排序
            // Sort in custom rules
            categories = categories
                .OrderBy(c => orderDictionary.GetValueOrDefault(c.CategoryBaseName, int.MaxValue))
                .ToList();
        }

        foreach (var category in categories)
            _topRow.CategorySelector.AddOption(category.CategoryDisplayName, category);
    }

    private void RecreateItemToggles()
    {
        _gridMenu.RemoveAllComponents();

        var entries = CategoryDataManager.Categories[ActiveCategory]
            .Select(itemKey => new ItemEntry(itemKey))
            .OrderBy(itemEntry => itemEntry)
            .ToList();

        var toggles = new List<ItemToggle<Item>>();
        foreach (var entry in entries)
        {
            var toggle = new ItemToggle<Item>(entry.Item, _chestData.Accepts(entry.ItemKey));
            toggle.OnToggle += () => ToggleItem(entry.ItemKey);
            toggle.OnHover += () =>
            {
                _tooltip = toggle.Tooltip;
                HoveredItem = toggle.Item;
            };
            toggles.Add(toggle);
        }

        _gridMenu.AddComponents(toggles);
    }

    private void OnToggleSelectAll(bool on)
    {
        if (on) SelectNone();
        else SelectAll();
    }

    private void SelectAll()
    {
        var allItems = CategoryDataManager.Categories[ActiveCategory];
        foreach (var itemKey in allItems)
            if (!_chestData.Accepts(itemKey))
                _chestData.Toggle(itemKey);

        RecreateItemToggles();
    }

    private void SelectNone()
    {
        var allItems = CategoryDataManager.Categories[ActiveCategory];
        foreach (var itemKey in allItems)
            if (_chestData.Accepts(itemKey))
                _chestData.Toggle(itemKey);

        RecreateItemToggles();
    }

    private void SelectNext()
    {
        _topRow.CategorySelector.SelectNext();
        RecreateItemToggles();
        _topRow.SelectAllButton.Checked = AreAllSelected();
    }

    private void SelectPrev()
    {
        _topRow.CategorySelector.SelectPrev();
        RecreateItemToggles();
        _topRow.SelectAllButton.Checked = AreAllSelected();
    }

    private void OnSelectChanged(ItemCategoryName category) => RecreateItemToggles();

    private void ToggleItem(ItemKey itemKey)
    {
        _chestData.Toggle(itemKey);
        _topRow.SelectAllButton.Checked = AreAllSelected();
    }

    private bool AreAllSelected() => _gridMenu.Components.OfType<ItemToggle<Item>>().All(t => t.Active);

    public override void Draw(SpriteBatch b) => _tooltip?.Draw(b);

    public override bool ReceiveCursorHover(int x, int y)
    {
        _tooltip = null;
        HoveredItem = null;
        return base.ReceiveCursorHover(x, y);
    }
}

/// <summary>
/// Customized menu for <see cref="CategoryMenu"/>.
/// </summary>
internal sealed class TopRow : IClickableMenu, IClickableComponent
{
    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X { get; set; }

    /// <inheritdoc/>
    public int Y { get; set; }

    /// <inheritdoc/>
    public int Width { get; set; }

    /// <inheritdoc/>
    public int Height { get; set; }

    public readonly NineSlice SelectAllButtonBackground;
    public readonly LabeledCheckBox SelectAllButton;
    public readonly SpriteButton PrevButton;
    public readonly SpriteButton NextButton;
    public readonly DropDownMenu<ItemCategoryName> CategorySelector;

    private List<IClickableComponent> _components = new();

    public TopRow(int x, int y, int width, int height, int parentMenuHeight)
    {
        SelectAllButtonBackground = NineSlice.CommonMenu();
        SelectAllButton = new LabeledCheckBox(I18n.Categorize_All(), Color.Black);
        PrevButton = new SpriteButton(TextureRegion.UpArrow());
        NextButton = new SpriteButton(TextureRegion.DownArrow());
        CategorySelector = new DropDownMenu<ItemCategoryName>(parentMenuHeight);

        var actualHeight = Math.Max(SelectAllButton.Height, PrevButton.Height);
        y -= Math.Max(0, actualHeight - height); // offset to the top
        this.SetDestination(x, y, width, height);
        SelectAllButtonBackground.SetDestination(x, y, SelectAllButton.Width + Game1.pixelZoom * 10, height);
        SelectAllButton.SetAtLeftCenterWithEqualMargins(SelectAllButtonBackground.Bounds);

        x += SelectAllButtonBackground.Width + Game1.pixelZoom * 4;
        PrevButton.SetDestination(x, y, height, height);
        x += height - Game1.pixelZoom * 2;
        NextButton.SetDestination(x, y, height, height);
        x += height + Game1.pixelZoom * 4;
        CategorySelector.SetPosition(x, y);

        _components.Add(SelectAllButton);
        _components.Add(PrevButton);
        _components.Add(NextButton);
    }

    public void Draw(SpriteBatch b)
    {
        SelectAllButtonBackground.Draw(b);
        SelectAllButton.Draw(b);
        PrevButton.Draw(b);
        NextButton.Draw(b);
        CategorySelector.Draw(b);
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        var handled = CategorySelector.ReceiveLeftClick(x, y);
        if (handled) return true;

        foreach (var component in _components)
            if (component.Contains(x, y))
                return component.ReceiveLeftClick(x, y);

        return false;
    }

    public bool ReceiveCursorHover(int x, int y) =>
        CategorySelector.Expanded && CategorySelector.ReceiveCursorHover(x, y);

    public bool ReceiveScrollWheelAction(int amount) =>
        CategorySelector.Expanded && CategorySelector.ReceiveScrollWheelAction(amount);

    public void Dispose()
    {
        CategorySelector.Dispose();
        _components.Clear();
        _components = null;
    }
}