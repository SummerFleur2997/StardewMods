using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

/// <summary>
/// Indicate a component with only a text label.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class TextLabel : IComponent
{
    public static TextLabel EmptyTextLabel => new("", Color.White, Game1.smallFont);

    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X { get; set; }

    /// <inheritdoc/>
    public int Y { get; set; }

    /// <summary>
    /// The width of the label, which should be automatically
    /// calculated and avoid to be changed manually.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the label, which should be automatically
    /// calculated and avoid to be changed manually.
    /// </summary>
    public int Height { get; set; }

    public Color Color;
    public SpriteFont Font;

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            CalculateDimensions();
        }
    }

    private string _text;

    public TextLabel(string text, Color color, SpriteFont font, int x = 0, int y = 0)
    {
        _text = text;
        Color = color;
        Font = font;
        CalculateDimensions();
        this.SetPosition(x, y);
    }

    public void Draw(SpriteBatch b) => b.DrawString(Font, _text, new Vector2(X, Y), Color);

    private void CalculateDimensions()
    {
        var measure = Font.MeasureString(_text);
        Width = (int)measure.X;
        Height = (int)measure.Y;
    }
}