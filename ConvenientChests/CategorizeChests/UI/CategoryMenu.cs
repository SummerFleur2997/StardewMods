using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.CategorizeChests.UI;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
internal class CategoryMenu<T> : BaseMenu where T : IChestData
{
    private const int TopRowHeight = 60;

    /// <summary>
    /// Used to compatible with LookupAnything.
    /// </summary>
    public Item HoveredItem;

    public Tooltip Tooltip;
    public CategoryTopRow TopRow;
    public GridMenu GridMenu;

    public readonly T ChestData;

    private ItemCategoryName ActiveCategory => TopRow.CategorySelector.SelectedValue;
    protected static bool IsAndroid => ModEntry.IsAndroid;

    protected CategoryMenu(int x, int y, int width, int height, T chestData)
        : base(x, y, width, height) =>
        ChestData = chestData;

    protected void BuildBasicWidgets(int padding)
    {
        var maxColumns = IsAndroid ? 99 : 12;
        var delta = IsAndroid ? 60 : 0;
        TopRow = new CategoryTopRow(
            xPositionOnScreen,
            yPositionOnScreen - TopRowHeight,
            width, TopRowHeight, height);
        GridMenu = new GridMenu(
            xPositionOnScreen + padding,
            yPositionOnScreen + padding,
            width - padding * 2 - delta,
            height - padding * 2,
            66, maxColumns);

        TopRow.NextButton.OnPress += SelectNext;
        TopRow.PrevButton.OnPress += SelectPrev;
        TopRow.SelectAllButton.OnToggle += OnToggleSelectAll;
        TopRow.CategorySelector.OnSelectionChanged += OnSelectChanged;

        // The click handler logic in BaseMenu will traverse the children in
        // reverse order, so we need to add the grid menu in front of the top
        // row to ensure that the drop-down in the top row is handled first.
        AddChildren(GridMenu, TopRow);
    }

    /// <summary>
    /// Get all the item categories, then link them with the drop-down.
    /// </summary>
    protected void BuildCategories()
    {
        var categories = CategoryDataManager.GetCategories();

        foreach (var category in categories)
            TopRow.CategorySelector.AddOption(category.DisplayName, category);

        RecreateItemToggles();
    }

    /// <summary>
    /// Refresh the status of all the item toggles at this page.
    /// </summary>
    protected void RefreshToggleStatus()
    {
        GridMenu.EditComponents(CheckActivate);

        return;

        void CheckActivate(IEnumerable<IComponent> toggles)
        {
            foreach (var toggle in toggles)
            {
                if (toggle is not ItemToggle<Item> itemToggle)
                    continue;

                var key = itemToggle.Item.ToItemKey();
                itemToggle.Active = ChestData.Accepts(key);
            }
        }
    }

    protected void RecreateItemToggles()
    {
        GridMenu.RemoveAllComponents();

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

        GridMenu.AddComponents(toggles);
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
        TopRow.CategorySelector.SelectNext();
        RecreateItemToggles();
        TopRow.SelectAllButton.Checked = AreAllSelected();
    }

    /// <summary>
    /// Select previous category, and then recreate toggles at this page.
    /// </summary>
    private void SelectPrev()
    {
        TopRow.CategorySelector.SelectPrev();
        RecreateItemToggles();
        TopRow.SelectAllButton.Checked = AreAllSelected();
    }

    /// <summary>
    /// A default event used in <see cref="CategoryTopRow"/> to update the item toggles.
    /// </summary>
    /// <param name="category">The new selected category.</param>
    private void OnSelectChanged(ItemCategoryName category) => RecreateItemToggles();

    private void ToggleItem(ItemKey itemKey)
    {
        ChestData.ToggleItem(itemKey);
        TopRow.SelectAllButton.Checked = AreAllSelected();
    }

    private bool AreAllSelected() => GridMenu.QueryComponents(c => c.OfType<ItemToggle<Item>>().All(t => t.Active));

    public override void Draw(SpriteBatch b) => Tooltip?.Draw(b);

    public override bool ReceiveCursorHover(int x, int y)
    {
        Tooltip = null;
        HoveredItem = null;
        return base.ReceiveCursorHover(x, y);
    }
}