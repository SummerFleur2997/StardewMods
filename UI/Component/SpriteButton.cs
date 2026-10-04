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

    /// <summary>
    /// How much bigger the button gets while hovered, e.g. 0.0625 for 6.25% like the game's own
    /// buttons. 0 keeps it at its size.
    /// </summary>
    public float HoverGrowth;

    public virtual void Draw(SpriteBatch b) => Texture.Draw(b, HoverGrowth > 0 ? GrownBounds() : Bounds);

    /// <summary>
    /// The bounds scaled by <see cref="Scale"/>, grown around the center.
    /// </summary>
    private Rectangle GrownBounds()
    {
        var width = (int)(Width * Scale);
        var height = (int)(Height * Scale);
        return new Rectangle(X - (width - Width) / 2, Y - (height - Height) / 2, width, height);
    }

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
        // with HoverGrowth set, grow at the game's pace: 0.04 per frame at its scale of 4
        var step = HoverGrowth > 0 ? 0.01f : 0.04f;
        var max = HoverGrowth > 0 ? 1 + HoverGrowth : 1.125f;

        if (!Bounds.Contains(x, y))
        {
            Scale = Math.Max(Scale - step, 1f);
            return false;
        }

        Scale = Math.Min(Scale + step, max);
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