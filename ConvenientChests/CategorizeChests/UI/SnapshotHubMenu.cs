using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.CategorizeChests.UI;

internal class SnapshotHubMenu : CategoryMenu<ChestDataSnapshot>
{
    public override ChestDataSnapshot ChestData => _selectedTag!.Snapshot;

    private readonly StackPanel _snapshotContainer;
    private readonly List<SnapshotTag> _snapshotTags;

    private SnapshotTag? _selectedTag;

    public SnapshotHubMenu(int x0, int y0, int width, int height, CategoryChestMenu parent, int padding)
        : base(x0, y0, width, height, parent, padding)
    {
        Components.Remove(TopRow);
        Background = null;
        ParentMenu = parent;

        var x = xPositionOnScreen;
        var textHeight = (int)Game1.smallFont.MeasureString("M").Y;
        var yForContainer = yPositionOnScreen + textHeight + 40;

        // left side
        var containerBound = new Rectangle(x, yPositionOnScreen, 304, height);
        var containerBg = NineSlice.MenuBackground(containerBound);

        var title = new TextLabel(I18n.UI_Snapshot(), Color.Black, Game1.smallFont);
        title.SetInCenterOfTheBounds(containerBound);
        title.OffsetPosition(y: yPositionOnScreen - title.Y + 32);

        _snapshotContainer = new StackPanel(x + 24, yForContainer, 256, height - 96, 64);
        _snapshotContainer.Background = containerBg;

        // right side
        x += 308;
        var previewerBound = new Rectangle(x, yPositionOnScreen, width - 308, height);
        var previewBg = NineSlice.MenuBackground(previewerBound);

        var preview = new TextLabel(I18n.UI_Snapshot_Preview(), Color.Black, Game1.smallFont);
        preview.SetInCenterOfTheBounds(previewerBound);
        preview.OffsetPosition(y: yPositionOnScreen - preview.Y + 32);

        GridMenu = new GridMenu(x + 24, yForContainer, width - 356, height - 160, 64);
        GridMenu.Background = previewBg;

        AddChildren(_snapshotContainer, GridMenu, title, preview);

        // Add snapshots to stack panel
        _snapshotTags = SnapshotManager.GetSnapshots()
            .Select(s => new SnapshotTag(_snapshotContainer.Width - 16, s))
            .ToList();

        foreach (var tag in _snapshotTags)
        {
            tag.OnSelected += SetActiveTag;
            tag.OnUnSelected += ClearActiveTag;
        }

        _snapshotContainer.AddComponents(_snapshotTags);

        return;

        void SetActiveTag(SnapshotTag t)
        {
            if (_selectedTag == t)
                return;

            _selectedTag?.UnSelect();
            _selectedTag = t;

            _snapshotSetter.RemoveAllComponents();
            var labels = t.Snapshot.AcceptedItemKinds
                .OrderBy(k => k)
                .Select(k => new ItemLabel<Item>(k.GetOne()));
            _snapshotSetter.AddComponents(labels);
        }

        void ClearActiveTag(SnapshotTag t) => _snapshotSetter.RemoveAllComponents();
    }

    public override void AfterDraw(SpriteBatch b) { }

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
    public override void receiveKeyPress(Keys key)
    {
        if (SubMenu is not null)
        {
            SubMenu.ReceiveKeyPress(key);
            return;
        }

        if (_selectedTag is { Selected: true })
        {
            _selectedTag.ReceiveKeyPress(key);
            return;
        }

        base.receiveKeyPress(key);
    }
}