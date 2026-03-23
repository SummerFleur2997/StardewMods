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

    /// <summary>
    /// The color of text.
    /// </summary>
    public Color Color;

    /// <summary>
    /// The font of text.
    /// </summary>
    public SpriteFont Font;

    /// <summary>
    /// Whether to draw the text label with shadow.
    /// </summary>
    public bool DrawWithShadow;

    /// <summary>
    /// The inner text of this label.
    /// </summary>
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

    /// <summary>
    /// Construct a text label. 
    /// </summary>
    /// <param name="text">The inner text of this label.</param>
    /// <param name="color">The color of text.</param>
    /// <param name="font">The font of text.</param>
    /// <param name="x">The x-position of label.</param>
    /// <param name="y">The y-position of label.</param>
    /// <param name="drawShadow">Whether to draw the text label with shadow.</param>
    public TextLabel(string text, Color color, SpriteFont font, int x = 0, int y = 0, bool drawShadow = false)
    {
        _text = text;
        Color = color;
        Font = font;
        DrawWithShadow = drawShadow;
        CalculateDimensions();
        this.SetPosition(x, y);
    }

    /// <inheritdoc />
    public void Draw(SpriteBatch b)
    {
        if (DrawWithShadow)
        {
            Utility.drawTextWithShadow(b, _text, Font, new Vector2(X, Y), Color.Black);
        }
        else
        {
            b.DrawString(Font, _text, new Vector2(X, Y), Color);
        }
    }

    private void CalculateDimensions()
    {
        var measure = Font.MeasureString(_text);
        Width = (int)measure.X;
        Height = (int)measure.Y;
    }
}