#nullable enable
using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.CategorizeChests.UI.SubMenus;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using ConvenientChests.Framework.UserInterfaceService;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;
using ClickableMenu = StardewValley.Menus.IClickableMenu;

namespace ConvenientChests.CategorizeChests.UI;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
internal class CategoryMenu : BaseMenu, IHaveSubMenu
{
    private const int TopRowHeight = 60;

    /// <summary>
    /// The submenu of this menu.
    /// </summary>
    public SubMenu? SubMenu { get; set; }

    /// <summary>
    /// Used to compatible with LookupAnything.
    /// </summary>
    public Item? HoveredItem;

    public CategoryTopRow TopRow;
    public GridMenu GridMenu;
    public Tooltip? Tooltip;
    public Button? Warning;

    public ChestData ChestData;

    private ItemCategoryName ActiveCategory => TopRow.CategorySelector.SelectedValue;
    private static bool IsAndroid => ModEntry.IsAndroid;

    public CategoryMenu(int x0, int y0, int width, int height, ChestData data, ClickableMenu parent, int padding)
        : base(x0, y0, width, height)
    {
        ChestData = data;
        ParentMenu = parent;

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

        if (ChestData.Snapshot is not null)
            AddSnapshotWarning();

        // quick set button
        var x = xPositionOnScreen + width + 16;
        var y = yPositionOnScreen + 64;
        var quickSetButton = UIHelper.SideButton(x, y, SideButtonVariant.Set);
        quickSetButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Set);
        quickSetButton.OnPress += QuickSet;
        AddChild(quickSetButton);

        // manage snapshot button
        y += 80;
        var manageSnapshotButton = UIHelper.SideButton(x, y, SideButtonVariant.Manage);
        manageSnapshotButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Manage);
        manageSnapshotButton.OnPress += ManageSnapshots;
        AddChild(manageSnapshotButton);

        // save as snapshot button
        y += 80;
        var saveAsSnapshotButton = UIHelper.SideButton(x, y, SideButtonVariant.Save);
        saveAsSnapshotButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Save);
        // _saveAsSnapshotButton.OnPress += SaveAsSnapshot;
        AddChild(saveAsSnapshotButton);

        // unlink snapshot button
        y += 80;
        var unlinkSnapshotButton = UIHelper.SideButton(x, y, SideButtonVariant.Unlink);
        unlinkSnapshotButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Unlink);
        unlinkSnapshotButton.OnPress += UnlinkSnapshot;
        AddChild(unlinkSnapshotButton);

        // Get all the item categories, then link them with the drop-down.
        var categories = CategoryDataManager.GetCategories();
        foreach (var category in categories)
            TopRow.CategorySelector.AddOption(category.DisplayName, category);

        RecreateItemToggles();
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

    public override void receiveKeyPress(Keys key)
    {
        if (SubMenu is not null)
        {
            SubMenu.ReceiveKeyPress(key);
            return;
        }

        base.receiveKeyPress(key);
    }

    #region # Private Methods
    
    
    /// <summary>
    /// Refresh the status of all the item toggles at this page.
    /// </summary>
    private void RefreshToggleStatus()
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

    /// <summary>
    /// Recreate the item toggles at this page when select a new category.
    /// </summary>
    private void RecreateItemToggles()
    {
        GridMenu.RemoveAllComponents();

        var itemKeys = CategoryDataManager.Categories[ActiveCategory]
            .OrderBy(k => k);

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

    private bool AreAllSelected()
    {
        var res = GridMenu.QueryComponents(c => c.OfType<ItemToggle<Item>>().All(t => t.Active));
        return res;
    }

    /// <summary>
    /// If this chest is using a snapshot, show a warning frame
    /// tells the player that they should edit the snapshot.
    /// </summary>
    private void AddSnapshotWarning()
    {
        var snapshotName = ChestData.Snapshot?.Alias ?? "";
        var text = Context.IsMainPlayer
            ? I18n.UI_Snapshot_Warning_Mainplayer(snapshotName)
            : I18n.UI_Snapshot_Warning_Farmhand();

        var parsedText = Game1.parseText(text, Game1.smallFont, width / 2);
        var background = NineSlice.TooltipBackground();
        Warning = new Button(background, parsedText, Color.Black, Game1.smallFont, padding: 16);
        Warning.SetInCenterOfTheBounds(Bounds);
        AddChild(Warning);
    }

    /// <summary>
    /// Create a new menu to manage snapshots.
    /// </summary>
    private void ManageSnapshots()
    {
        var menu = new SnapshotHubMenu(xPositionOnScreen, yPositionOnScreen, width, height, this);
        AddDependency();
        Game1.activeClickableMenu = menu;
    }

    /// <summary>
    /// Add all the items in this chest to the category list.
    /// </summary>
    private void QuickSet()
    {
        if (ChestData.Snapshot != null)
            return;

        var chest = ChestData.GetChest();
        if (chest is null)
            return;

        var items = chest.Items
            .DistinctBy(i => i.QualifiedItemId)
            .Select(i => i.ToItemKey());

        foreach (var key in items)
        {
            if (!ChestData.Accepts(key))
                ChestData.ToggleItem(key);
        }

        RefreshToggleStatus();
    }

    /// <summary>
    /// Unlink the snapshot of this chest, use <see cref="SubMenus.DoubleConfirmSubMenu"/>
    /// to double-confirm this operation.
    /// </summary>
    private void UnlinkSnapshot()
    {
        if (ChestData.Snapshot is null || !Context.IsMainPlayer)
            return;

        SubMenu = new DoubleConfirmSubMenu(this, I18n.UI_Snapshot_Unlink_Confirm());
        SubMenu.OnOk += UnlinkSnapshotExecute;

        return;

        void UnlinkSnapshotExecute(SubMenu sender)
        {
            ChestData.AcceptedItemKinds = ChestData.Snapshot.AcceptedItemKinds;
            ChestData.Snapshot = null;
            if (Warning is null)
                return;

            RemoveChild(Warning);
            Warning.Dispose();
            Warning = null;
        }
    }

    private void SetSnapshot(ChestDataSnapshot snapshot)
    {
        ChestData.Snapshot = snapshot;
        ChestData.AcceptedItemKinds = snapshot.AcceptedItemKinds;
        AddSnapshotWarning();
    }

    /// <summary>
    /// Hover event for side buttons
    /// </summary>
    private void ShowTooltipForSideButton(SideButtonVariant hint)
    {
        var tooltip = hint switch
        {
            SideButtonVariant.Set => new Tooltip(I18n.UI_QuickSet(), I18n.UI_QuickSet_Desc()),
            SideButtonVariant.Manage => new Tooltip(I18n.UI_Snapshot_Manage(), I18n.UI_Snapshot_Manage_Desc()),
            SideButtonVariant.Save => new Tooltip(I18n.UI_Snapshot_Save(), I18n.UI_Snapshot_Save_Desc()),
            SideButtonVariant.Unlink => new Tooltip(I18n.UI_Snapshot_Unlink(), I18n.UI_Snapshot_Unlink_Desc()),
            _ => null
        };
        Tooltip = tooltip;
    }

    #endregion
}