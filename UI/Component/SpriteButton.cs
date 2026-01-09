using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Sprite;

namespace UI.Component;

/// <summary>
/// Indicate a button with only a sprite.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SpriteButton : IClickableComponent, IDisposable
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

    public TextureRegion Texture;
    public event Action OnPress;

    public SpriteButton(TextureRegion texture, int x = 0, int y = 0, int width = 16, int height = 16)
    {
        Texture = texture;
        this.SetDestination(x, y, width, height);
    }

    public SpriteButton(TextureRegion texture, Rectangle destination)
    {
        Texture = texture;
        this.SetDestination(destination);
    }

    public virtual void Draw(SpriteBatch b) => b.Draw(Texture, Bounds);

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