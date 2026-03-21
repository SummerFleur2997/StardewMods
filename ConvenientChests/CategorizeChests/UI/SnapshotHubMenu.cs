#nullable enable
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.CategorizeChests.UI;

internal class SnapshotHubMenu : BaseMenu, IHaveSubMenu
{
    private readonly StackPanel _snapshotContainer;
    private readonly GridMenu _snapshotSetter;
    private readonly List<SnapshotTag> _snapshotTags;

    private SnapshotTag? _selectedTag;

    public SubMenu? SubMenu { get; set; }

    public SnapshotHubMenu(int x, int y, int width, int height, CategoryMenu parent)
        : base(x, y, width, height)
    {
        Background = null;
        ParentMenu = parent;

        var x0 = xPositionOnScreen;
        var textHeight = (int)Game1.smallFont.MeasureString("M").Y;
        var yForContainer = yPositionOnScreen + textHeight + 40;

        // left side
        var containerBound = new Rectangle(x0, yPositionOnScreen, 304, height);
        var containerBg = NineSlice.MenuBackground(containerBound);

        var title = new TextLabel(I18n.UI_Snapshot(), Color.Black, Game1.smallFont);
        title.SetInCenterOfTheBounds(containerBound);
        title.OffsetPosition(y: yPositionOnScreen - title.Y + 32);

        _snapshotContainer = new StackPanel(x0 + 24, yForContainer, 256, height - 96, 64);
        _snapshotContainer.Background = containerBg;

        // right side
        x0 += 308;
        var previewerBound = new Rectangle(x0, yPositionOnScreen, width - 308, height);
        var previewBg = NineSlice.MenuBackground(previewerBound);

        var preview = new TextLabel(I18n.UI_Snapshot_Preview(), Color.Black, Game1.smallFont);
        preview.SetInCenterOfTheBounds(previewerBound);
        preview.OffsetPosition(y: yPositionOnScreen - preview.Y + 32);

        _snapshotSetter = new GridMenu(x0 + 24, yForContainer, width - 356, height - 160, 64);
        _snapshotSetter.Background = previewBg;

        AddChildren(_snapshotContainer, _snapshotSetter, title, preview);

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

    private sealed class SnapshotTag : IClickableComponent, IDisposable
    {
        /// <inheritdoc/>
        public Rectangle Bounds => new(X, Y, Width, Height);

        /// <inheritdoc/>
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                _selectedBg.X = _unselectedBg.X = _x + (Width - Background.Width) / 2;
                TextBox.X = _x + (Width - TextBox.Width) / 2;
            }
        }

        private int _x;

        /// <inheritdoc/>
        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                _selectedBg.Y = _unselectedBg.Y = _y + (Height - Background.Height) / 2;
                TextBox.Y = _y + (Height - TextBox.Height) / 2;
            }
        }

        private int _y;

        /// <inheritdoc/>
        public int Width { get; set; }

        /// <inheritdoc/>
        public int Height { get; set; }

        public event Action<SnapshotTag>? OnSelected;
        public event Action<SnapshotTag>? OnUnSelected;

        public bool Selected;
        public readonly TextBox TextBox;
        public readonly ChestDataSnapshot Snapshot;

        private NineSlice Background => Selected ? _selectedBg : _unselectedBg;

        private readonly NineSlice _selectedBg;
        private readonly NineSlice _unselectedBg;

        public SnapshotTag(int width, ChestDataSnapshot snapshot)
        {
            this.SetSize(width, 56);
            _selectedBg = UIHelper.OrangeButtonBackground(Bounds);
            _unselectedBg = UIHelper.LightButtonBackground(Bounds);
            TextBox = new TextBox(0, 0, width, 64, snapshot.Alias);
            TextBox.SetInCenterOfTheBounds(Bounds);
            TextBox.OnEnterPressed += OnRename;
            Snapshot = snapshot;
        }

        public void Select()
        {
            Selected = true;
            OnSelected?.Invoke(this);
        }

        public void UnSelect()
        {
            if (!Selected && !TextBox.Selected)
                return;

            Selected = false;
            TextBox.Selected = false;
            Game1.keyboardDispatcher.Subscriber = null;
            OnUnSelected?.Invoke(this);
        }

        /// <summary>
        /// Rename the snapshot held by this tag.
        /// </summary>
        /// <param name="textBox"></param>
        private void OnRename(TextBox textBox)
        {
            // snapshot alias has a customized setter to check duplicated name
            Snapshot.Alias = textBox.Text;
            textBox.Text = Snapshot.Alias;
        }

        /// <inheritdoc/>
        public void Draw(SpriteBatch b)
        {
            Background.Draw(b);
            TextBox.Draw(b);
        }

        /// <inheritdoc/>
        public bool ReceiveLeftClick(int x, int y)
        {
            if (!Bounds.Contains(x, y))
            {
                UnSelect();
                return false;
            }

            if (!Selected)
            {
                Select();
                Game1.playSound("drumkit6");
                return true;
            }

            if (!TextBox.Selected)
            {
                TextBox.Selected = true;
                Game1.keyboardDispatcher.Subscriber = TextBox;
                Game1.playSound("drumkit6");
            }

            return true;
        }

        /// <summary>
        /// Receives a key press. This method can only be called
        /// when the <see cref="Selected"/> is true.
        /// </summary>
        public void ReceiveKeyPress(Keys key)
        {
            // if the textbox is not selected, deselect this tag
            if (!TextBox.Selected)
            {
                UnSelect();
                Game1.playSound("bigDeSelect");
                return;
            }

            // else, the textbox is selected, handle some special key press
            switch (key)
            {
                case Keys.Escape:
                    TextBox.Selected = false;
                    Game1.playSound("bigDeSelect");
                    return;
                case Keys.Enter:
                    TextBox.Selected = false;
                    Game1.playSound("money");
                    return;
            }
        }

        /// <inheritdoc/>
        public bool ReceiveCursorHover(int x, int y) => !Bounds.Contains(x, y);

        public void Dispose() => OnSelected = null;
    }
}