using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

/// <summary>
/// Indicate a button with both sprite background and text.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class TextButton : IClickableComponent, IDisposable
{
    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X
    {
        get => _x;
        set
        {
            Label.X += value - _x;
            Background.X = value;
            _x = value;
        }
    }

    private int _x;


    /// <inheritdoc/>
    public int Y
    {
        get => _y;
        set
        {
            Label.Y += value - _y;
            Background.Y = value;
            _y = value;
        }
    }

    private int _y;

    /// <summary>
    /// The width of the button, which should be automatically
    /// calculated and avoid to be changed manually.
    /// </summary>
    public int Width
    {
        get => _width;
        set
        {
            Label.X = _x + (value - Label.Width) / 2;
            Background.Width = value;
            _width = value;
        }
    }

    private int _width;

    /// <summary>
    /// The height of the button, which should be automatically
    /// calculated and avoid to be changed manually.
    /// </summary>
    public int Height
    {
        get => _height;
        set
        {
            Label.Y = _y + (value - Label.Height) / 2;
            Background.Height = value;
            _height = value;
        }
    }

    private int _height;

    public IComponent Background;
    public TextLabel Label;

    private int _padding;

    public event Action OnPress;

    /// <summary>
    /// Construct a text button. 
    /// </summary>
    /// <param name="background">The background sprite of this button.</param>
    /// <param name="text">The text of this button.</param>
    /// <param name="color">The color of text.</param>
    /// <param name="font">The font of text.</param>
    /// <param name="x">The x-position of background.</param>
    /// <param name="y">The y-position of background.</param>
    /// <param name="width">The width of the background.</param>
    /// <param name="height">The height of the background.</param>
    /// <param name="padding">The padding between the text and the background.</param>
    public TextButton(IComponent background, string text, Color color, SpriteFont font,
        int x = 0, int y = 0, int? width = null, int? height = null, int padding = Game1.pixelZoom * 2)
    {
        Background = background;
        Label = new TextLabel(text, color, font);
        _padding = padding;

        // Use the width and height of the label as the width and height of the button.
        _width = Background.Width = width ?? Label.Width + padding * 2;
        _height = Background.Height = height ?? Label.Height + padding * 2;
        _x = Background.X = x;
        _y = Background.Y = y;
        Label.SetInCenterOfTheBounds(Bounds);
    }

    public virtual void Draw(SpriteBatch b)
    {
        Background.Draw(b);
        Label.Draw(b);
    }

    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnPress?.Invoke();
        Game1.playSound("drumkit6");
        return true;
    }

    public virtual bool ReceiveCursorHover(int x, int y) => false;

    public virtual void Dispose()
    {
        OnPress = null;
        GC.SuppressFinalize(this);
    }
}