/*using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

/// <summary>
/// Indicate a button with a background and a sprite label.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SpritesButton : IClickableComponent, IDisposable
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
            Background.X = _x + (_width - Background.Width) / 2;
            Body.X = _x + (_width - Body.Width) / 2;
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
            Background.Y = value;
            Body.Y = _y + (_height - Body.Height) / 2;
        }
    }

    private int _y;

    /// <inheritdoc/>
    public int Width
    {
        get => _width;
        set
        {
            Background.Width = value;
            Body.X += (value - _width) / 2;
            _width = value;
        }
    }

    private int _width;

    /// <inheritdoc/>
    public int Height
    {
        get => _height;
        set
        {
            Background.Height = value;
            Body.Y += (value - _height) / 2;
            _height = value;
        }
    }

    private int _height;

    public IComponent Background;
    public SpriteLabel Body;

    public string SoundCue = "drumkit6";
    public event Action OnPress;
    public event Action OnHover;

    /// <summary>
    /// The default ctor of this component. Warning: we should initialize the size
    /// of body before constructing this component. After constructed, the size of
    /// body will never change.
    /// </summary>
    public SpritesButton(SpriteLabel body, IComponent bg, int x = 0, int y = 0, int width = 16, int height = 16)
    {
        _width = width;
        _height = height;

        Background = bg;
        Background.SetSize(width, height);

        Body = body;
        Body.SetInCenterOfTheBounds(Bounds);

        this.SetPosition(x, y);
    }

    /// <inheritdoc/>
    public virtual void Draw(SpriteBatch b)
    {
        Background.Draw(b);
        Body.Draw(b);
    }

    /// <inheritdoc/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnPress?.Invoke();
        if (!string.IsNullOrEmpty(SoundCue)) Game1.playSound(SoundCue);
        return true;
    }

    /// <inheritdoc/>
    public virtual bool ReceiveCursorHover(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnHover?.Invoke();
        return true;
    }

    public virtual void Dispose()
    {
        OnPress = null;
        OnHover = null;
        GC.SuppressFinalize(this);
    }
}*/