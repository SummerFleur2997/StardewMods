using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using UI.Sprite;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class LabeledCheckBox : IClickableComponent, IDisposable
{
    private const int CheckboxSize = 36;

    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X
    {
        get => _x;
        set
        {
            _x = value;
            FilledCheckbox.X = EmptyCheckbox.X = value;
            Label.X = value + FilledCheckbox.Width + Game1.pixelZoom;
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
            FilledCheckbox.Y = EmptyCheckbox.Y = value + _checkboxYPadding;
            Label.Y = value + _labelYPadding;
        }
    }

    private int _y;

    /// <summary>
    /// The width of the button, which should be automatically
    /// calculated and avoid to be changed manually.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the button, which should be automatically
    /// calculated and avoid to be changed manually.
    /// </summary>
    public int Height { get; set; }

    public SpriteLabel FilledCheckbox;
    public SpriteLabel EmptyCheckbox;
    public TextLabel Label;

    public bool Checked;

    /// <summary>
    /// The action to be performed when the checkbox is toggled.
    /// </summary>
    /// <seealso cref="OnToggleHandler"/>
    public event OnToggleHandler OnToggle;

    /// <summary>
    /// The action to be performed when the checkbox is toggled.
    /// </summary>
    /// <param name="checkedState">Whether the checkbox is currently
    /// checked (true) or unchecked (false).</param>
    public delegate void OnToggleHandler(bool checkedState);

    private int _checkboxYPadding;
    private int _labelYPadding;

    public LabeledCheckBox(string text, Color color, int x = 0, int y = 0)
    {
        FilledCheckbox = new SpriteLabel(TextureRegion.FilledCheckbox(), x, y, CheckboxSize, CheckboxSize);
        EmptyCheckbox = new SpriteLabel(TextureRegion.EmptyCheckbox(), x, y, CheckboxSize, CheckboxSize);
        Label = new TextLabel(text, color, Game1.smallFont, x, y);

        // Use the width and height of the label as the width and height of the button.
        Width = Label.Width + CheckboxSize;
        Height = Math.Max(Label.Height, CheckboxSize);

        _checkboxYPadding = (Height - CheckboxSize) / 2;
        _labelYPadding = (Height - Label.Height) / 2;
        this.SetPosition(x, y);
    }

    public void Draw(SpriteBatch b)
    {
        var box = Checked ? FilledCheckbox : EmptyCheckbox;
        box.Draw(b);
        Label.Draw(b);
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnToggle?.Invoke(Checked);
        Checked = !Checked;
        Game1.playSound("drumkit6");
        return true;
    }

    public bool ReceiveCursorHover(int x, int y) => false;

    public void Dispose() => OnToggle = null;
}