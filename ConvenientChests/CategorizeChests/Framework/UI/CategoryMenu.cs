using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.CategorizeChests.Framework.UI;

internal class CategoryMenu<T> : BaseMenu where T : IChestData
{
    private const int TopRowHeight = 60;

    /// <summary>
    /// Used to compatible with LookupAnything.
    /// </summary>
    [UsedImplicitly] public Item HoveredItem;

    protected readonly T ChestData;
    protected Tooltip Tooltip;
    private CategoryTopRow _topRow;
    private GridMenu _gridMenu;

    private ItemCategoryName ActiveCategory => _topRow.CategorySelector.SelectedValue;
    protected static bool IsAndroid => ModEntry.IsAndroid;

    protected CategoryMenu(int x, int y, int width, int height, T chestData)
        : base(x, y, width, height) =>
        ChestData = chestData;

    protected void BuildBasicWidgets(int padding)
    {
        var maxColumns = IsAndroid ? 99 : 12;
        var delta = IsAndroid ? 60 : 0;
        _topRow = new CategoryTopRow(
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
        AddChildren(_gridMenu, _topRow);
    }

    /// <summary>
    /// Get all the item categories, then link them with the drop-down.
    /// </summary>
    protected void BuildCategories()
    {
        var categories = CategoryDataManager.GetCategories();

        foreach (var category in categories)
            _topRow.CategorySelector.AddOption(category.DisplayName, category);

        RecreateItemToggles();
    }

    /// <summary>
    /// Refresh the status of all the item toggles at this page.
    /// </summary>
    protected void RefreshToggleStatus()
    {
        var toggles = _gridMenu.Components.OfType<ItemToggle<Item>>();
        foreach (var toggle in toggles)
        {
            var key = toggle.Item.ToItemKey();
            toggle.Active = ChestData.Accepts(key);
        }
    }

    private void RecreateItemToggles()
    {
        _gridMenu.RemoveAllComponents();

        var itemKeys = CategoryDataManager.Categories[ActiveCategory]
            .OrderBy(k => k)
            .ToList();

        var toggles = new List<ItemToggle<Item>>();
        foreach (var key in itemKeys)
        {
            var toggle = new ItemToggle<Item>(key.GetOne(), ChestData.Accepts(key));
            toggle.OnToggle += () => ToggleItem(key);
            toggle.OnHover += () =>
            {
                Tooltip = toggle.Tooltip;
                HoveredItem = toggle.Item;
            };
            toggles.Add(toggle);
        }

        _gridMenu.AddComponents(toggles);
    }

    /// <summary>
    /// The action to perform when the select all button is toggled.
    /// </summary>
    /// <param name="on"></param>
    private void OnToggleSelectAll(bool on)
    {
        if (on)
        {
            var allItems = CategoryDataManager.Categories[ActiveCategory];
            foreach (var itemKey in allItems)
                if (!ChestData.Accepts(itemKey))
                    ChestData.ToggleItem(itemKey);
        }
        else
        {
            var allItems = CategoryDataManager.Categories[ActiveCategory];
            foreach (var itemKey in allItems)
                if (ChestData.Accepts(itemKey))
                    ChestData.ToggleItem(itemKey);
        }
        RecreateItemToggles();
    }

    /// <summary>
    /// Select next category, and then recreate toggles at this page.
    /// </summary>
    private void SelectNext()
    {
        _topRow.CategorySelector.SelectNext();
        RecreateItemToggles();
        _topRow.SelectAllButton.Checked = AreAllSelected();
    }

    /// <summary>
    /// Select previous category, and then recreate toggles at this page.
    /// </summary>
    private void SelectPrev()
    {
        _topRow.CategorySelector.SelectPrev();
        RecreateItemToggles();
        _topRow.SelectAllButton.Checked = AreAllSelected();
    }

    /// <summary>
    /// A default event used in <see cref="CategoryTopRow"/> to update the item toggles.
    /// </summary>
    /// <param name="category">The new selected category.</param>
    private void OnSelectChanged(ItemCategoryName category) => RecreateItemToggles();

    private void ToggleItem(ItemKey itemKey)
    {
        ChestData.ToggleItem(itemKey);
        _topRow.SelectAllButton.Checked = AreAllSelected();
    }

    private bool AreAllSelected() => _gridMenu.Components.OfType<ItemToggle<Item>>().All(t => t.Active);

    public override void Draw(SpriteBatch b) => Tooltip?.Draw(b);

    public override bool ReceiveCursorHover(int x, int y)
    {
        Tooltip = null;
        HoveredItem = null;
        return base.ReceiveCursorHover(x, y);
    }
}