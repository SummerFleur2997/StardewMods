using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Sprite;

namespace UI.Component;

/// <summary>
/// Indicate a component with only a texture.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class SpriteLabel : IComponent
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

    public SpriteLabel(TextureRegion texture, int x = 0, int y = 0, int width = 16, int height = 16)
    {
        Texture = texture;
        this.SetDestination(x, y, width, height);
    }

    public SpriteLabel(TextureRegion texture, Rectangle destination)
    {
        Texture = texture;
        this.SetDestination(destination);
    }

    public void Draw(SpriteBatch b) => b.Draw(Texture, Bounds);
}