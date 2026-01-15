using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Sprite;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public static class SpriteHelper
{
    public static readonly Texture2D CursorSheet = Game1.mouseCursors;
    public static readonly Texture2D MenuSheet = Game1.menuTexture;

    public static void Draw(this SpriteBatch b, TextureRegion texture, Rectangle destination, Color? color = null)
        => b.Draw(texture.Texture, destination, texture.Region, color ?? Color.White);

    public static void Draw(this SpriteBatch b, TextureRegion texture, Rectangle destination,
        SpriteEffects effect, Color? color = null)
        => b.Draw(texture.Texture, destination, texture.Region, color ?? Color.White, 0f, Vector2.Zero, effect, 0f);
}