using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Sprite;

namespace UI.Component;

/// <summary>
/// Indicate a button with only a sprite.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SpriteButton : IClickableComponent, IHaveTooltip, IDisposable
{
    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X { get; set; }

    /// <inheritdoc/>
    public int Y { get; set; }

    /// <inheritdoc/>
    public int Width { get; set; }

    /// <inheritdoc/>
    public int Height { get; set; }

    public Tooltip? Tooltip { get; set; }

    public float Scale = 1;
    public string SoundCue = "drumkit6";
    public TextureRegion Texture;
    public event Action? OnPress;
    public event Action? OnHover;

    public SpriteButton(TextureRegion texture, int x = 0, int y = 0, int width = 64, int height = 64)
    {
        Texture = texture;
        this.SetDestination(x, y, width, height);
    }

    public SpriteButton(TextureRegion texture, Rectangle destination)
    {
        Texture = texture;
        this.SetDestination(destination);
    }

    public virtual void Draw(SpriteBatch b) => Texture.Draw(b, Bounds);

    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnPress?.Invoke();
        if (!string.IsNullOrEmpty(SoundCue)) Game1.playSound(SoundCue);
        return true;
    }

    public virtual bool ReceiveCursorHover(int x, int y)
    {
        if (!Bounds.Contains(x, y))
        {
            Scale = Math.Max(Scale - 0.04f, 1f);
            return false;
        }

        Scale = Math.Min(Scale + 0.04f, 1.125f);
        OnHover?.Invoke();
        return true;
    }

    public virtual void Dispose()
    {
        OnPress = null;
        OnHover = null;
        GC.SuppressFinalize(this);
    }
}