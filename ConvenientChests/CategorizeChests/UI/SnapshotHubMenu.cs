using ConvenientChests.CategorizeChests.UI.SubMenus;
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.CategorizeChests.UI;

internal class SnapshotHubMenu : CategoryMenu<ChestDataSnapshot>
{
    public bool EditMode
    {
        get => _editMode;
        set
        {
            if (_editMode == value)
                return;

            if (value)
            {
                _gridMenuTitle = _edit;
                Components.Add(TopRow);
                RecreateItemToggles();
            }
            else
            {
                _gridMenuTitle = _preview;
                Components.Remove(TopRow);
                RecreateItemPreviewToggles(_selectedTag);
            }

            _editMode = value;
        }
    }

    private bool _editMode;

    public override ChestDataSnapshot ChestData => _selectedTag!.Snapshot;

    private TextLabel _gridMenuTitle;
    private readonly StackPanel _stackPanel;
    private readonly ToggleButton _epButton;
    private readonly Button _applyButton;
    private readonly Button _deleteButton;

    private readonly TextLabel _edit;
    private readonly TextLabel _preview;

    private SnapshotTag? _selectedTag;

    public SnapshotHubMenu(int x0, int y0, int width, int height, CategoryChestMenu parent, int padding)
        : base(x0, y0, width, height, parent, padding)
    {
        Background = null;
        ParentMenu = parent;

        var x = xPositionOnScreen;
        var textHeight = (int)Game1.smallFont.MeasureString("M").Y;
        var y = yPositionOnScreen + textHeight + 40;

        // left side
        var containerBound = new Rectangle(x, yPositionOnScreen, 304, height);
        var containerBg = NineSlice.MenuBackground(containerBound);

        var stackPanelTitle = new TextLabel(I18n.UI_Snapshot(), Color.Black, Game1.smallFont);
        stackPanelTitle.SetInCenterOfTheBounds(containerBound);
        stackPanelTitle.OffsetPosition(y: yPositionOnScreen - stackPanelTitle.Y + 32);

        _stackPanel = new StackPanel(x + 24, y, 256, height - 160, 64);
        _stackPanel.Background = containerBg;

        // right side
        x += 308;
        var previewerBound = new Rectangle(x, yPositionOnScreen, width - 308, height);
        var previewBg = NineSlice.MenuBackground(previewerBound);

        _edit = new TextLabel(I18n.UI_Snapshot_Edit(), Color.Black, Game1.smallFont);
        _edit.SetInCenterOfTheBounds(previewerBound);
        _edit.OffsetPosition(y: yPositionOnScreen - _edit.Y + 32);

        _preview = new TextLabel(I18n.UI_Snapshot_Preview(), Color.Black, Game1.smallFont);
        _preview.SetInCenterOfTheBounds(previewerBound);
        _preview.OffsetPosition(y: yPositionOnScreen - _preview.Y + 32);

        _gridMenuTitle = _preview;

        GridMenu = new GridMenu(x + 24, y, width - 356, height - 160, 64);
        GridMenu.Background = previewBg;

        AddChildren(_stackPanel, GridMenu, stackPanelTitle, _gridMenuTitle);

        x = _stackPanel.X + _stackPanel.Width / 2 - 64;
        y = _stackPanel.Y + _stackPanel.Height;
        var createTexture = UIHelper.LightButtonBackground();
        var createButton = new Button(createTexture, I18n.UI_Snapshot_Create(), Color.Black, Game1.smallFont);
        createButton.SetDestination(x, y, 128, 60);
        createButton.OnPress += NewSnapshot;
        AddChild(createButton);

        x = GridMenu.X + GridMenu.Width / 2 - 170;
        var epTexture = UIHelper.YellowButtonBackground();
        _epButton = new ToggleButton(epTexture, I18n.UI_Snapshot_Preview(), I18n.UI_Snapshot_Edit(),
            Color.Black, Game1.smallFont);
        _epButton.SetDestination(x, y, 108, 60);
        _epButton.OnToggle += ChangeStatus;

        x += 116;
        var applyTexture = UIHelper.YellowButtonBackground();
        _applyButton = new Button(applyTexture, I18n.UI_Snapshot_Apply(), Color.Black, Game1.smallFont);
        _applyButton.SetDestination(x, y, 108, 60);
        _applyButton.Tooltip = new Tooltip(desc: I18n.UI_Snapshot_Apply_Desc());
        _applyButton.SoundCue = "money";
        _applyButton.OnPress += ApplySnapshot;

        x += 116;
        var deleteTexture = UIHelper.RedButtonBackground();
        _deleteButton = new Button(deleteTexture, I18n.UI_Snapshot_Delete(), Color.Black, Game1.smallFont);
        _deleteButton.SetDestination(x, y, 108, 60);
        _deleteButton.Tooltip = new Tooltip(desc: I18n.UI_Snapshot_Delete_Desc());
        _deleteButton.OnPress += DeleteSnapshot;

        // Add snapshots to stack panel
        var snapshotTags = SnapshotManager.GetSnapshots()
            .Select(s => new SnapshotTag(_stackPanel.Width - 16, s))
            .ToList();

        foreach (var tag in snapshotTags)
        {
            tag.OnSelected += SetActiveTag;
            tag.OnUnSelected += ClearActiveTag;
        }

        _stackPanel.AddComponents(snapshotTags);
    }

    /// <inheritdoc/>
    public override bool ReceiveKeyPress(Keys key)
    {
        if (SubMenu is not null)
        {
            var handled = SubMenu.ReceiveKeyPress(key);
            if (handled) return true;
        }

        if (_selectedTag is { Selected: true })
        {
            if (key == Keys.Delete)
            {
                DeleteSnapshot();
                return true;
            }

            var handled = _selectedTag.ReceiveKeyPress(key);
            if (handled) return true;
        }

        return false;
    }

    private void ChangeStatus(bool editMode)
    {
        if (editMode)
        {
            _gridMenuTitle = _edit;
            Components.Add(TopRow);
            RecreateItemToggles();
        }
        else
        {
            _gridMenuTitle = _preview;
            Components.Remove(TopRow);
            RecreateItemPreviewToggles(_selectedTag);
        }
    }

    private void RecreateItemPreviewToggles(SnapshotTag? t)
    {
        GridMenu.RemoveAllComponents();
        if (t is null) return;
        var labels = t.Snapshot.AcceptedItemKinds
            .Select(k => k.GetOne())
            .Where(i => i.Name != Item.ErrorItemName)
            .OrderBy(i => i)
            .Select(i => new ItemLabel<Item>(i));
        GridMenu.AddComponents(labels);
    }

    private void SetActiveTag(SnapshotTag t)
    {
        if (_selectedTag == t)
            return;

        _selectedTag?.UnSelect();
        _selectedTag = t;

        RecreateItemPreviewToggles(t);

        Components.Add(_epButton);
        Components.Add(_applyButton);
        Components.Add(_deleteButton);
    }

    private void ClearActiveTag(SnapshotTag t)
    {
        GridMenu.RemoveAllComponents();
        _epButton.Active = false;
        _selectedTag = null;

        EditMode = false;

        Components.Remove(_epButton);
        Components.Remove(_applyButton);
        Components.Remove(_deleteButton);
    }

    /// <summary>
    /// Create a new snapshot.
    /// </summary>
    private void NewSnapshot()
    {
        var snapshot = SnapshotManager.CreateNewSnapshot(I18n.UI_Unnamed());
        var tag = new SnapshotTag(_stackPanel.Width - 16, snapshot);

        SnapshotManager.Add(snapshot);

        _stackPanel.AddComponents(tag);
        _stackPanel.ScrollToBottom();

        tag.OnSelected += SetActiveTag;
        tag.OnUnSelected += ClearActiveTag;
        _selectedTag?.UnSelect();
        _selectedTag = tag;
        _selectedTag.Select(true);
    }

    /// <summary>
    /// Apply the chosen snapshot to this chest.
    /// </summary>
    private void ApplySnapshot()
    {
        if (ParentMenu is not CategoryChestMenu menu)
            return;

        menu.SetSnapshot(ChestData);
        _selectedTag?.UnSelect();
        receiveKeyPress(Keys.Escape);
    }

    /// <summary>
    /// Unlink the snapshot of this chest, use <see cref="SubMenus.DoubleConfirmSubMenu"/>
    /// to double-confirm this operation.
    /// </summary>
    private void DeleteSnapshot()
    {
        SubMenu = new DoubleConfirmSubMenu(this, I18n.UI_Snapshot_Delete_Comfirm());
        SubMenu.OnOk += DeleteSnapshotExecute;

        return;

        void DeleteSnapshotExecute(SubMenu sender)
        {
            if (_selectedTag is null)
                throw new InvalidOperationException("No snapshot selected");

            SnapshotManager.Remove(_selectedTag.Snapshot.UniqueID);
            _stackPanel.Remove(_selectedTag);
            _selectedTag = null;
        }
    }
}