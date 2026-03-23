using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ToggleButton : IClickableComponent, IDisposable
{
    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X
    {
        get => _x;
        set
        {
            Label1.X += value - _x;
            Label2.X += value - _x;
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
            Label1.Y += value - _y;
            Label2.Y += value - _y;
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
            Label1.X = _x + (value - Label1.Width) / 2;
            Label2.X = _x + (value - Label2.Width) / 2;
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
            Label1.Y = _y + (value - Label1.Height) / 2;
            Label2.Y = _y + (value - Label2.Height) / 2;
            Background.Height = value;
            _height = value;
        }
    }

    private int _height;

    public string SoundCue = "drumkit6";
    public IComponent Background;

    public TextLabel Label1;
    public TextLabel Label2;

    private int _padding;

    public bool Active;
    public event Action<bool>? OnToggle;
    public event Action<bool>? OnHover;

    /// <summary>
    /// Construct a button. 
    /// </summary>
    /// <param name="background">The background sprite of this button.</param>
    /// <param name="text1">The first (also initial) text of this button.</param>
    /// <param name="text2">The second text of this button.</param>
    /// <param name="color">The color of text.</param>
    /// <param name="font">The font of text.</param>
    /// <param name="x">The x-position of background.</param>
    /// <param name="y">The y-position of background.</param>
    /// <param name="width">The width of the background.</param>
    /// <param name="height">The height of the background.</param>
    /// <param name="padding">The padding between the text and the background.</param>
    /// <param name="drawShadow">Whether to draw the text label with shadow.</param>
    public ToggleButton(IComponent background, string text1, string text2, Color color, SpriteFont font,
        int x = 0, int y = 0, int? width = null, int? height = null, int padding = Game1.pixelZoom * 2,
        bool drawShadow = false)
    {
        Background = background;
        Label1 = new TextLabel(text1, color, font, drawShadow: drawShadow);
        Label2 = new TextLabel(text2, color, font, drawShadow: drawShadow);
        _padding = padding;

        // Use the width and height of the label as the width and height of the button.
        _width = Background.Width = width ?? Math.Max(Label1.Width, Label2.Width) + padding * 2;
        _height = Background.Height = height ?? Math.Max(Label1.Height, Label2.Height) + padding * 2;
        _x = Background.X = x;
        _y = Background.Y = y;
        Label1.SetInCenterOfTheBounds(Bounds);
        Label2.SetInCenterOfTheBounds(Bounds);
    }

    public virtual void Draw(SpriteBatch b)
    {
        Background.Draw(b);
        if (Active) Label2.Draw(b);
        else Label1.Draw(b);
    }

    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        Active = !Active;
        OnToggle?.Invoke(Active);
        if (!string.IsNullOrEmpty(SoundCue)) Game1.playSound(SoundCue);
        return true;
    }

    public virtual bool ReceiveCursorHover(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnHover?.Invoke(Active);
        return true;
    }

    public virtual void Dispose()
    {
        OnToggle = null;
        OnHover = null;
        GC.SuppressFinalize(this);
    }
}