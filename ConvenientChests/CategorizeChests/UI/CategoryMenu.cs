using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using ConvenientChests.Framework.UserInterfaceService;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;
using ClickableMenu = StardewValley.Menus.IClickableMenu;

namespace ConvenientChests.CategorizeChests.UI;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
internal abstract class CategoryMenu<T> : BaseMenu, IHaveSubMenu where T : IChestData
{
    private const int TopRowHeight = 60;

    public abstract T ChestData { get; }

    /// <summary>
    /// The submenu of this menu.
    /// </summary>
    public SubMenu? SubMenu { get; set; }

    public CategoryTopRow TopRow;
    public GridMenu GridMenu;

    private ItemCategoryName ActiveCategory => TopRow.CategorySelector.SelectedValue;

    protected CategoryMenu(int x, int y, int width, int height, ClickableMenu parent, int padding)
        : base(x, y, width, height)
    {
        ParentMenu = parent;

        // var maxColumns = IsAndroid ? 99 : 12;
        // var delta = IsAndroid ? 60 : 0;
        TopRow = new CategoryTopRow(
            xPositionOnScreen,
            yPositionOnScreen - TopRowHeight,
            width, TopRowHeight, height);
        GridMenu = new GridMenu(
            xPositionOnScreen + padding,
            yPositionOnScreen + padding,
            width - padding * 2, // - delta
            height - padding * 2,
            66, 12);

        TopRow.NextButton.OnPress += SelectNext;
        TopRow.PrevButton.OnPress += SelectPrev;
        TopRow.SelectAllButton.OnToggle += OnToggleSelectAll;
        TopRow.CategorySelector.OnSelectionChanged += OnSelectChanged;

        // Get all the item categories, then link them with the drop-down.
        var categories = CategoryDataManager.GetCategories();
        foreach (var category in categories)
            TopRow.CategorySelector.AddOption(category.DisplayName, category);
    }

    /// <inheritdoc />
    public override void AfterDraw(SpriteBatch b)
    {
        SubMenu?.Draw(b);
        Tooltip?.Draw(b);
    }

    /// <inheritdoc/>
    public override bool ReceiveLeftClick(int x, int y)
    {
        if (SubMenu is not null)
        {
            SubMenu.ReceiveLeftClick(x, y);
            return true;
        }

        return base.ReceiveLeftClick(x, y);
    }

    /// <inheritdoc/>
    public override bool ReceiveCursorHover(int x, int y)
    {
        Tooltip = null;
        HoveredItem = null;

        if (SubMenu is not null)
        {
            SubMenu.ReceiveCursorHover(x, y);
            return true;
        }

        return base.ReceiveCursorHover(x, y);
    }

    /// <inheritdoc/>
    public override bool ReceiveScrollWheelAction(int amount)
    {
        if (SubMenu is not null)
        {
            SubMenu.ReceiveScrollWheelAction(amount);
            return true;
        }

        return base.ReceiveScrollWheelAction(amount);
    }

    /// <inheritdoc/>
    public override bool ReceiveKeyPress(Keys key)
    {
        if (SubMenu is not null)
        {
            SubMenu.ReceiveKeyPress(key);
            return true;
        }

        return false;
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

                var key = itemToggle.Item?.ToItemKey();
                if (key is not null)
                    itemToggle.Active = ChestData.Accepts(key);
            }
        }
    }

    /// <summary>
    /// Recreate the item toggles at this page when select a new category.
    /// </summary>
    protected void RecreateItemToggles(bool showEmpty = false)
    {
        GridMenu.RemoveAllComponents();
        if (showEmpty) return;

        var itemKeys = CategoryDataManager.Categories[ActiveCategory]
            .OrderBy(k => k);

        var toggles = new List<ItemToggle<Item>>();
        foreach (var key in itemKeys)
        {
            var toggle = new ItemToggle<Item>(key.GetOne(), ChestData.Accepts(key));
            toggle.OnToggle += () => ToggleItem(key);
            toggles.Add(toggle);
        }

        GridMenu.AddComponents(toggles);
    }

    /// <summary>
    /// The action to perform when the select all button is toggled.
    /// </summary>
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

    private bool AreAllSelected()
    {
        var res = GridMenu.QueryComponents(c => c.OfType<ItemToggle<Item>>().All(t => t.Active));
        return res;
    }
}