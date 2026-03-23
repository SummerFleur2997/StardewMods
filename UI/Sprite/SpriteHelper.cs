using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Sprite;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public static class SpriteHelper
{
    public static readonly Texture2D CursorSheet = Game1.mouseCursors;
    public static readonly Texture2D MenuSheet = Game1.menuTexture;

    /// <summary>
    /// Submit a sprite for drawing in the current batch.
    /// </summary>
    /// <param name="b">The spriteBatch.</param>
    /// <param name="texture">The <see cref="TextureRegion"/> instance.</param>
    /// <param name="destination">The destination to draw on screen.</param>
    /// <param name="color">A color mask.</param>
    /// <param name="rotation">A rotation of this sprite.</param>
    /// <param name="origin">Center of the rotation. 0,0 by default.</param>
    /// <param name="effects">Modificators for drawing. Can be combined.</param>
    /// <param name="depth">A depth of the layer of this sprite.</param>
    public static void Draw(this TextureRegion texture, SpriteBatch b, Rectangle destination, Color? color = null,
        float rotation = 0f, Vector2? origin = null, SpriteEffects effects = SpriteEffects.None, float depth = 0.1f)
    {
        var ori = origin ?? Vector2.Zero;
        b.Draw(texture.Texture, destination, texture.Region, Color.White, rotation, ori, effects, depth);
    }
}