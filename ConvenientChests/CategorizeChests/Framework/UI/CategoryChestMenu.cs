using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;

namespace ConvenientChests.CategorizeChests.Framework.UI;

internal class CategoryChestMenu : CategoryMenu<ChestData>, IHaveSubMenu
{
    private Button _warning;
    private SpritesButton _editNoteButton;
    private SpritesButton _quickSetButton;
    private SpritesButton _manageSnapshotButton;
    private SpritesButton _saveAsSnapshotButton;
    private SpritesButton _unlinkSnapshotButton;

    public SubMenu SubMenu { get; set; }

    public CategoryChestMenu(int x, int y, int width, int height, ChestData chestData, int padding = 0)
        : base(x, y, width, height, chestData)
    {
        BuildWidgets(padding);
        BuildCategories();
    }

    public override void Draw(SpriteBatch b)
    {
        SubMenu?.Draw(b);
        base.Draw(b);
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
    public override void receiveKeyPress(Keys key)
    {
        if (SubMenu is not null)
        {
            SubMenu.ReceiveKeyPress(key);
            return;
        }

        base.receiveKeyPress(key);
    }

    private void BuildWidgets(int padding)
    {
        BuildBasicWidgets(padding);

        if (ChestData.Snapshot is not null) AddSnapshotWarning();

        // edit note button
        var x = xPositionOnScreen + width + 16;
        var y = yPositionOnScreen + 40;
        _editNoteButton = SpritesButton.CreateButton(x, y, SideButtonVariant.Edit);
        _editNoteButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Edit);

        // quick set button
        y += 80;
        _quickSetButton = SpritesButton.CreateButton(x, y, SideButtonVariant.Set);
        _quickSetButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Set);
        _quickSetButton.OnPress += QuickSet;

        // manage snapshot button
        y += 80;
        _manageSnapshotButton = SpritesButton.CreateButton(x, y, SideButtonVariant.Manage);
        _manageSnapshotButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Manage);

        // save as snapshot button
        y += 80;
        _saveAsSnapshotButton = SpritesButton.CreateButton(x, y, SideButtonVariant.Save);
        _saveAsSnapshotButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Save);

        // unlink snapshot button
        y += 80;
        _unlinkSnapshotButton = SpritesButton.CreateButton(x, y, SideButtonVariant.Unlink);
        _unlinkSnapshotButton.OnHover += () => ShowTooltipForSideButton(SideButtonVariant.Unlink);
        _unlinkSnapshotButton.OnPress += UnlinkSnapshot;

        AddChildren(
            _editNoteButton, _quickSetButton, _manageSnapshotButton, _saveAsSnapshotButton, _unlinkSnapshotButton);
    }

    /// <summary>
    /// If this chest is using a snapshot, show a warning frame
    /// tells the player that they should edit the snapshot.
    /// </summary>
    private void AddSnapshotWarning()
    {
        var snapshotName = ChestData.Snapshot.Note;
        var text = Context.IsMainPlayer
            ? I18n.UI_Snapshot_Warning_Mainplayer(snapshotName)
            : I18n.UI_Snapshot_Warning_Farmhand();

        var parsedText = Game1.parseText(text, Game1.smallFont, width / 2);
        var background = NineSlice.TooltipBackground();
        _warning = new Button(background, parsedText, Color.Black, Game1.smallFont, padding: 16);
        _warning.SetInCenterOfTheBounds(Bounds);
        AddChild(_warning);
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
            if (!ChestData.Accepts(key))
                ChestData.ToggleItem(key);

        RefreshToggleStatus();
    }

    /// <summary>
    /// Unlink the snapshot of this chest, use <see cref="UI.SubMenu"/>
    /// to double-confirm this operation.
    /// </summary>
    private void UnlinkSnapshot()
    {
        if (ChestData.Snapshot is null || !Context.IsMainPlayer)
            return;

        var confirm = Game1.parseText(I18n.UI_Snapshot_Unlink_Confirm(), Game1.smallFont, 300);
        var confirmLabel = new TextLabel(confirm, Color.Black, Game1.smallFont);
        SubMenu = new SubMenu(320, 128 + confirmLabel.Height, this);
        SubMenu.OkButton.Background = UIHelper.RedButtonBackground(SubMenu.OkButton.Bounds);

        confirmLabel.SetInCenterOfTheBounds(Bounds);
        confirmLabel.OffsetPosition(y: SubMenu.Y - confirmLabel.Y + 32);
        SubMenu.OnOk += UnlinkSnapshotExecute;
        SubMenu.Components.Add(confirmLabel);

        return;

        void UnlinkSnapshotExecute()
        {
            ChestData.AcceptedItemKinds = ChestData.Snapshot.AcceptedItemKinds;
            ChestData.Snapshot = null;
            RemoveChild(_warning);
            _warning.Dispose();
            _warning = null;
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
    /// <param name="hint"></param>
    private void ShowTooltipForSideButton(SideButtonVariant hint) =>
        Tooltip = hint switch
        {
            SideButtonVariant.Edit => new Tooltip(I18n.UI_ChestNote(), I18n.UI_ChestNote_Desc()),
            SideButtonVariant.Set => new Tooltip(I18n.UI_QuickSet(), I18n.UI_QuickSet_Desc()),
            SideButtonVariant.Manage => new Tooltip(I18n.UI_Snapshot_Manage(), I18n.UI_Snapshot_Manage_Desc()),
            SideButtonVariant.Save => new Tooltip(I18n.UI_Snapshot_Save(), I18n.UI_Snapshot_Save_Desc()),
            SideButtonVariant.Unlink => new Tooltip(I18n.UI_Snapshot_Unlink(), I18n.UI_Snapshot_Unlink_Desc()),
            _ => null
        };
}