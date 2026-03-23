using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;

namespace ConvenientChests.CategorizeChests.UI;

/// <summary>
/// A customized component to display in the <see cref="SnapshotHubMenu"/>..
/// </summary>
internal sealed class SnapshotTag : IClickableComponent, IDisposable
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