using ConvenientChests.CategorizeChests.UI.SubMenus;
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.CategorizeChests.UI;

internal class SnapshotHubMenu : CategoryMenu<ChestDataSnapshot>
{
    /// <summary>
    /// Whether the menu is in edit mode.
    /// </summary>
    public bool EditMode
    {
        get => _editMode;
        set
        {
            if (_editMode == value)
                return;

            if (value)
            {
                TopRow.CategorySelector.SelectByValue(ChestData.PotentialMostRelevantCategory());
                RecreateItemToggles();
            }
            else
                RecreateItemPreviewToggles(_selectedTag);

            _editMode = value;
        }
    }

    private bool _editMode;

    /// <inheritdoc />
    public override ChestDataSnapshot ChestData => _selectedTag!.Snapshot;

    /// <summary>
    /// The stack panel that contains the snapshot tags.
    /// </summary>
    public StackPanel StackPanel;

    private readonly TextLabel _editLabel;
    private readonly TextLabel _previewLabel;

    private readonly ToggleButton _editOrPreviewButton;
    private readonly IComponent[] _availableWhenTagSelected;

    private SnapshotTag? _selectedTag;

    public SnapshotHubMenu(int x0, int y0, int width, int height, CategoryChestMenu parent, int padding)
        : base(x0, y0, width, height, parent, padding)
    {
        Background = null;
        ParentMenu = parent;

        var x = xPositionOnScreen;
        var textHeight = (int)Game1.smallFont.MeasureString("M").Y;
        var y = yPositionOnScreen + textHeight + 40;

        // stack panel
        var containerBound = new Rectangle(x, yPositionOnScreen, 304, height);
        var containerBg = NineSlice.MenuBackground(containerBound);

        var stackPanelTitle = new TextLabel(I18n.UI_Snapshot(), Color.Black, Game1.smallFont);
        stackPanelTitle.SetInCenterOfTheBounds(containerBound);
        stackPanelTitle.OffsetPosition(y: yPositionOnScreen - stackPanelTitle.Y + 32);

        StackPanel = new StackPanel(x + 24, y, 256, height - 160, 64);
        StackPanel.Background = containerBg;

        AddChildren(StackPanel, stackPanelTitle);

        // grid menu
        x += 308;
        var previewerBound = new Rectangle(x, yPositionOnScreen, width - 308, height);
        var previewBg = NineSlice.MenuBackground(previewerBound);

        _editLabel = new TextLabel(I18n.UI_Snapshot_Edit(), Color.Black, Game1.smallFont);
        _editLabel.SetInCenterOfTheBounds(previewerBound);
        _editLabel.OffsetPosition(y: yPositionOnScreen - _editLabel.Y + 32);

        _previewLabel = new TextLabel(I18n.UI_Snapshot_Preview(), Color.Black, Game1.smallFont);
        _previewLabel.SetInCenterOfTheBounds(previewerBound);
        _previewLabel.OffsetPosition(y: yPositionOnScreen - _previewLabel.Y + 32);

        GridMenu = new GridMenu(x + 24, y, width - 356, height - 160, 64);
        GridMenu.Background = previewBg;

        AddChildren(GridMenu, _editLabel, _previewLabel);

        // create new snapshot button
        x = StackPanel.X + StackPanel.Width / 2 - 64;
        y = StackPanel.Y + StackPanel.Height;
        var createTexture = UIHelper.LightButtonBackground();
        var createButton = new Button(createTexture, I18n.UI_Snapshot_Create(), Color.Black, Game1.smallFont);
        createButton.SetDestination(x, y, 128, 60);
        createButton.OnPress += NewSnapshot;
        AddChild(createButton);

        // edit/preview button
        x = GridMenu.X + GridMenu.Width / 2 - 170;
        var epTexture = UIHelper.YellowButtonBackground();
        _editOrPreviewButton = new ToggleButton(epTexture, I18n.UI_Snapshot_Preview(), I18n.UI_Snapshot_Edit(),
            Color.Black, Game1.smallFont);
        _editOrPreviewButton.SetDestination(x, y, 108, 60);
        _editOrPreviewButton.OnToggle += s => EditMode = s;

        // apply button
        x += 116;
        var applyTexture = UIHelper.YellowButtonBackground();
        var applyButton = new Button(applyTexture, I18n.UI_Snapshot_Apply(), Color.Black, Game1.smallFont);
        applyButton.SetDestination(x, y, 108, 60);
        applyButton.Tooltip = new Tooltip(desc: I18n.UI_Snapshot_Apply_Desc());
        applyButton.SoundCue = "money";
        applyButton.OnPress += ApplySnapshot;

        // delete button
        x += 116;
        var deleteTexture = UIHelper.RedButtonBackground();
        var deleteButton = new Button(deleteTexture, I18n.UI_Snapshot_Delete(), Color.Black, Game1.smallFont);
        deleteButton.SetDestination(x, y, 108, 60);
        deleteButton.Tooltip = new Tooltip(desc: I18n.UI_Snapshot_Delete_Desc());
        deleteButton.OnPress += DeleteSnapshot;

        AddChildren(_editOrPreviewButton, applyButton, deleteButton);
        _availableWhenTagSelected = new IComponent[] { _editOrPreviewButton, applyButton, deleteButton };

        // Add snapshots to stack panel
        var snapshotTags = SnapshotManager.GetSnapshots()
            .Select(s => new SnapshotTag(StackPanel.Width - 16, s))
            .ToList();

        foreach (var tag in snapshotTags)
        {
            tag.OnSelected += SetActiveTag;
            tag.OnUnSelected += ClearActiveTag;
        }

        StackPanel.AddComponents(snapshotTags);
        AddChildren(TopRow);
    }

    /// <summary>
    /// High customized draw method.
    /// </summary>
    public override void DrawComponents(SpriteBatch b)
    {
        foreach (var component in Components)
        {
            if (EditMode && component == _previewLabel)
                continue;

            if (!EditMode && (component == _editLabel || component == TopRow))
                continue;

            if (_selectedTag is null && _availableWhenTagSelected.Contains(component))
                continue;

            component.Draw(b);
        }
    }

    /// <summary>
    /// High customized ReceiveLeftClick method.
    /// </summary>
    public override bool ReceiveLeftClick(int x, int y)
    {
        // let the submenu handle the click first
        if (SubMenu is not null)
        {
            SubMenu.ReceiveLeftClick(x, y);
            return true;
        }

        // then check if the top row is clicked
        if (EditMode && TopRow.ReceiveLeftClick(x, y))
            return true;

        // travel from back to front
        for (var i = Components.Count - 1; i >= 0; i--)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            if (_selectedTag is null && _availableWhenTagSelected.Contains(component))
                continue;

            if (c.ReceiveLeftClick(x, y))
                return true;
        }

        return false;
    }

    /// <summary>
    /// High customized ReceiveCursorHover method.
    /// </summary>
    public override bool ReceiveCursorHover(int x, int y)
    {
        Tooltip = null;
        HoveredItem = null;

        if (SubMenu is not null)
            return true;

        // check if the top row is available
        if (EditMode && TopRow.ReceiveCursorHover(x, y))
            return true;

        // travel from back to front
        for (var i = Components.Count - 1; i >= 0; i--)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            if (_selectedTag is null && _availableWhenTagSelected.Contains(component))
                continue;

            if (c.ReceiveCursorHover(x, y))
                return true;
        }

        return false;
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

    public override void OnExit()
    {
        SnapshotManager.Save();
        base.OnExit();
    }

    /// <summary>
    /// Recreate item toggles for preview mode.
    /// </summary>
    private void RecreateItemPreviewToggles(SnapshotTag? t)
    {
        GridMenu.RemoveAllComponents();
        if (t is null) return;
        var labels = t.Snapshot.AcceptedItems
            .Select(k => k.ConvertToItem())
            .Where(i => i.Name != Item.ErrorItemName)
            .OrderBy(i => i)
            .Select(i => new ItemLabel<Item>(i));
        GridMenu.AddComponents(labels);
    }

    /// <summary>
    /// Action to execute when select a new snapshot tag.
    /// </summary>
    /// <param name="t">The new selected tag.</param>
    private void SetActiveTag(SnapshotTag t)
    {
        if (_selectedTag == t)
            return;

        _selectedTag?.UnSelect();
        _selectedTag = t;

        RecreateItemPreviewToggles(t);
    }

    /// <summary>
    /// Action to execute when deselect a new snapshot tag.
    /// </summary>
    /// <param name="t">The just deselected tag.</param>
    private void ClearActiveTag(SnapshotTag t)
    {
        GridMenu.RemoveAllComponents();
        _editOrPreviewButton.Active = false;
        _selectedTag = null;

        EditMode = false;
    }

    /// <summary>
    /// Create a new snapshot.
    /// </summary>
    private void NewSnapshot()
    {
        var snapshot = SnapshotManager.CreateNewSnapshot(I18n.UI_Unnamed());
        var tag = new SnapshotTag(StackPanel.Width - 16, snapshot);

        SnapshotManager.Add(snapshot);

        StackPanel.AddComponents(tag);
        StackPanel.ScrollToBottom();

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
        exitThisMenuNoSound();
    }

    /// <summary>
    /// Unlink the snapshot of this chest, use <see cref="SubMenus.DoubleConfirmSubMenu"/>
    /// to double-confirm this operation.
    /// </summary>
    private void DeleteSnapshot()
    {
        SubMenu = new DoubleConfirmSubMenu(this, I18n.UI_Snapshot_Delete_Confirm());
        SubMenu.OnOk += DeleteSnapshotExecute;

        return;

        void DeleteSnapshotExecute(SubMenu sender)
        {
            if (_selectedTag is null)
                throw new InvalidOperationException("No snapshot selected");

            SnapshotManager.Remove(_selectedTag.Snapshot.UniqueID);
            StackPanel.Remove(_selectedTag);
            GridMenu.RemoveAllComponents();
            _selectedTag = null;
        }
    }
}