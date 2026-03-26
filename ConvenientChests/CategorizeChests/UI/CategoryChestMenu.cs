using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.CategorizeChests.UI.SubMenus;
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using UI.Component;
using ClickableMenu = StardewValley.Menus.IClickableMenu;

namespace ConvenientChests.CategorizeChests.UI;

internal class CategoryChestMenu : CategoryMenu<ChestData>
{
    /// <inheritdoc />
    public override ChestData ChestData { get; }

    /// <summary>
    /// A warning label that appears when the chest is using a snapshot.
    /// </summary>
    private TextLabel? _warning;

    public CategoryChestMenu(int x0, int y0, int width, int height, ChestData data, ClickableMenu root, int padding)
        : base(x0, y0, width, height, root, padding)
    {
        ChestData = data;

        // quick set button
        var x = xPositionOnScreen + width + 16;
        var y = yPositionOnScreen + 64;
        var quickSetButton = UIHelper.SideButton(x, y, SideButtonVariant.Set);
        quickSetButton.OnPress += QuickSet;
        AddChild(quickSetButton);

        // manage snapshot button
        y += 80;
        var manageSnapshotButton = UIHelper.SideButton(x, y, SideButtonVariant.Manage);
        manageSnapshotButton.OnPress += ManageSnapshots;
        AddChild(manageSnapshotButton);

        // save as snapshot button
        y += 80;
        var saveAsSnapshotButton = UIHelper.SideButton(x, y, SideButtonVariant.Save);
        saveAsSnapshotButton.OnPress += SaveAsSnapshot;
        AddChild(saveAsSnapshotButton);

        // unlink snapshot button
        y += 80;
        var unlinkSnapshotButton = UIHelper.SideButton(x, y, SideButtonVariant.Unlink);
        unlinkSnapshotButton.OnPress += UnlinkSnapshot;
        AddChild(unlinkSnapshotButton);

        // Get all the item categories, then link them with the drop-down.
        var categories = CategoryDataManager.GetCategories();
        foreach (var category in categories)
            TopRow.CategorySelector.AddOption(category.DisplayName, category);

        // add the grid menu in front of the top row to ensure
        // that the drop-down in the top row is handled first.
        AddChildren(GridMenu, TopRow);

        if (data.Snapshot is not null)
        {
            AddSnapshotWarning();
        }
        else
        {
            RecreateItemToggles();
            TopRow.CategorySelector.SelectByValue(data.PotentialMostRelevantCategory());
        }
    }

    /// <summary>
    /// Set a snapshot for this chest.
    /// </summary>
    public void SetSnapshot(ChestDataSnapshot snapshot)
    {
        ChestData.Snapshot = snapshot;
        ChestData.AcceptedItems = snapshot.AcceptedItems;
        AddSnapshotWarning();
    }

    /// <summary>
    /// If this chest is using a snapshot, show a warning frame
    /// tells the player that they should edit the snapshot.
    /// </summary>
    private void AddSnapshotWarning()
    {
        RecreateItemToggles(true);
        Components.Remove(TopRow);
        var snapshotName = ChestData.Snapshot?.Alias ?? "";
        var text = Context.IsMainPlayer
            ? I18n.UI_Snapshot_Warning_Mainplayer(snapshotName)
            : I18n.UI_Snapshot_Warning_Farmhand();

        var parsedText = Game1.parseText(text, Game1.smallFont, width / 2);
        if (_warning is not null) return;
        _warning = new TextLabel(parsedText, Color.Black, Game1.smallFont, drawShadow: true);
        _warning.SetInCenterOfTheBounds(Bounds);
        AddChild(_warning);
    }

    /// <summary>
    /// Create a new menu to manage snapshots.
    /// </summary>
    private void ManageSnapshots()
    {
        var menu = new SnapshotHubMenu(xPositionOnScreen, yPositionOnScreen, width, height, this, borderWidth);
        AddDependency();
        Game1.activeClickableMenu = menu;
        ChildMenu = menu;
    }

    /// <summary>
    /// Add all the items in this chest to the category list.
    /// </summary>
    private void QuickSet()
    {
        if (ChestData.Snapshot != null)
            return;

        var items = ChestData.ChestRef.Items
            .Select(i => i.QualifiedItemId)
            .Distinct();

        foreach (var key in items)
        {
            if (!ChestData.Accepts(key))
                ChestData.Toggle(key);
        }

        RefreshToggleStatus();
    }

    /// <summary>
    /// Save the accepted item of this chest as a new snapshot.
    /// </summary>
    private void SaveAsSnapshot()
    {
        SubMenu = new NewSnapshotSubMenu(this);
        SubMenu.OnOk += SaveAsSnapshotExecute;

        return;

        void SaveAsSnapshotExecute(SubMenu sender)
        {
            if (sender is not NewSnapshotSubMenu menu)
                return;

            var snapshot = SnapshotManager.CreateNewSnapshot(menu.TextBox.Text, ChestData.AcceptedItems);
            SnapshotManager.Add(snapshot);
        }
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
            ChestData.AcceptedItems = ChestData.Snapshot.AcceptedItems;
            ChestData.Snapshot = null;
            if (_warning is null)
                return;

            RemoveChild(_warning);
            _warning = null;
            RecreateItemToggles();
            AddChild(TopRow);
        }
    }
}