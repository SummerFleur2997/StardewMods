using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Sprite;

namespace ConvenientChests.CategorizeChests.Framework.UI;

public static class UIHelper
{
    private static Texture2D Cursors => Game1.mouseCursors;

    public static readonly Texture2D Texture =
        ModEntry.ModHelper.ModContent.Load<Texture2D>(Path.Combine("assets", "texture.png"));

    public static NineSlice YellowButtonBackground(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors, 269, 256, 1, 2, true),
        new TextureRegion(Cursors, 267, 258, 2, 1, true),
        new TextureRegion(Cursors, 275, 263, 2, 1, true),
        new TextureRegion(Cursors, 274, 264, 1, 2, true),
        new TextureRegion(Cursors, 267, 256, 2, 2, true),
        new TextureRegion(Cursors, 275, 256, 2, 2, true),
        new TextureRegion(Cursors, 267, 264, 2, 2, true),
        new TextureRegion(Cursors, 275, 264, 2, 2, true),
        new TextureRegion(Cursors, 269, 258, 1, 1, true),
        bounds
    );

    public static NineSlice OrangeButtonBackground(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors, 258, 256, 1, 2, true),
        new TextureRegion(Cursors, 256, 258, 2, 1, true),
        new TextureRegion(Cursors, 264, 263, 2, 1, true),
        new TextureRegion(Cursors, 263, 264, 1, 2, true),
        new TextureRegion(Cursors, 256, 256, 2, 2, true),
        new TextureRegion(Cursors, 264, 256, 2, 2, true),
        new TextureRegion(Cursors, 256, 264, 2, 2, true),
        new TextureRegion(Cursors, 264, 264, 2, 2, true),
        new TextureRegion(Cursors, 258, 258, 1, 1, true),
        bounds
    );

    public static NineSlice RedButtonBackground(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors, 139, 338, 1, 2, true),
        new TextureRegion(Cursors, 137, 340, 2, 1, true),
        new TextureRegion(Cursors, 142, 344, 2, 1, true),
        new TextureRegion(Cursors, 141, 345, 1, 2, true),
        new TextureRegion(Cursors, 137, 338, 2, 2, true),
        new TextureRegion(Cursors, 142, 338, 2, 2, true),
        new TextureRegion(Cursors, 137, 345, 2, 2, true),
        new TextureRegion(Cursors, 142, 345, 2, 2, true),
        new TextureRegion(Cursors, 139, 340, 1, 1, true),
        bounds
    );
}