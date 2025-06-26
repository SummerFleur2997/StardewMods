using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ConvenientChests.Framework.UserInterfacService;

/// <summary>
/// 从游戏 .xnb 文件中获取到的各个 UI 元素的精灵图
/// UI elements' sprites extracted from the game's .xnb files
/// </summary>
internal static class Sprites
{
    public static NineSlice TabBackground { get; } = new()
    {
        Left        = new TextureRegion(Game1.mouseCursors, new Rectangle(0,  388, 3, 1), true),
        Right       = new TextureRegion(Game1.mouseCursors, new Rectangle(13, 388, 3, 1), true),
        Bottom      = new TextureRegion(Game1.mouseCursors, new Rectangle(4,  397, 1, 3), true),
        TopLeft     = new TextureRegion(Game1.mouseCursors, new Rectangle(0,  384, 5, 5), true),
        TopRight    = new TextureRegion(Game1.mouseCursors, new Rectangle(11, 384, 5, 5), true),
        BottomLeft  = new TextureRegion(Game1.mouseCursors, new Rectangle(0,  395, 5, 5), true),
        BottomRight = new TextureRegion(Game1.mouseCursors, new Rectangle(11, 395, 5, 5), true),
        Center      = new TextureRegion(Game1.mouseCursors, new Rectangle(5,  387, 1, 1), true)
    };

    /// <summary>
    /// 菜单的九宫格精灵图
    /// Nine-slice sprite of a menu
    /// </summary>
    public static NineSlice MenuBackground { get; }  = new()
    {
        Top         = new TextureRegion(Game1.menuTexture, new Rectangle(40,   12, 1, 24)),
        Left        = new TextureRegion(Game1.menuTexture, new Rectangle(12,   36, 24, 1)),
        Right       = new TextureRegion(Game1.menuTexture, new Rectangle(220,  40, 24, 1)),
        Bottom      = new TextureRegion(Game1.menuTexture, new Rectangle(36,  220, 1, 24)),
        TopLeft     = new TextureRegion(Game1.menuTexture, new Rectangle(12,   12, 24, 24)),
        TopRight    = new TextureRegion(Game1.menuTexture, new Rectangle(220,  12, 24, 24)),
        BottomLeft  = new TextureRegion(Game1.menuTexture, new Rectangle(12,  220, 24, 24)),
        BottomRight = new TextureRegion(Game1.menuTexture, new Rectangle(220, 220, 24, 24)),
        Center      = new TextureRegion(Game1.menuTexture, new Rectangle(64,  128, 64, 64))
    };

    /// <summary>
    /// Tooltip 的九宫格精灵图
    /// Nine-slice sprite of a tooltip
    /// </summary>
    public static NineSlice TooltipBackground { get; } = new()
    {
        Top         = new TextureRegion(Game1.mouseCursors, new Rectangle(297, 360, 16, 4), true),
        Left        = new TextureRegion(Game1.mouseCursors, new Rectangle(293, 364, 4, 16), true),
        Right       = new TextureRegion(Game1.mouseCursors, new Rectangle(313, 364, 4, 16), true),
        Bottom      = new TextureRegion(Game1.mouseCursors, new Rectangle(297, 380, 16, 4), true),
        TopLeft     = new TextureRegion(Game1.mouseCursors, new Rectangle(293, 360, 4,  4), true),
        TopRight    = new TextureRegion(Game1.mouseCursors, new Rectangle(313, 360, 4,  4), true),
        BottomLeft  = new TextureRegion(Game1.mouseCursors, new Rectangle(293, 380, 4,  4), true),
        BottomRight = new TextureRegion(Game1.mouseCursors, new Rectangle(313, 380, 4,  4), true),
        Center      = new TextureRegion(Game1.mouseCursors, new Rectangle(297, 364, 16, 16), true)
    };

    /// <summary>
    /// 左悬浮按钮的九宫格精灵图
    /// Nine-slice sprite of a left-protruding tab button
    /// </summary>
    public static NineSlice LeftProtrudingTab { get; } = new()
    {
        Top         = new TextureRegion(Game1.mouseCursors, new Rectangle(661, 64, 1, 4), true),
        Left        = new TextureRegion(Game1.mouseCursors, new Rectangle(656, 69, 5, 1), true),
        Right       = new TextureRegion(Game1.mouseCursors, new Rectangle(670, 68, 2, 1), true),
        Bottom      = new TextureRegion(Game1.mouseCursors, new Rectangle(661, 76, 1, 4), true),
        TopLeft     = new TextureRegion(Game1.mouseCursors, new Rectangle(656, 64, 5, 5), true),
        TopRight    = new TextureRegion(Game1.mouseCursors, new Rectangle(670, 64, 2, 5), true),
        BottomLeft  = new TextureRegion(Game1.mouseCursors, new Rectangle(656, 75, 5, 5), true),
        BottomRight = new TextureRegion(Game1.mouseCursors, new Rectangle(670, 75, 2, 5), true),
        Center      = new TextureRegion(Game1.mouseCursors, new Rectangle(661, 68, 1, 1), true)
    };

    /// <summary>
    /// 左箭头按钮
    /// Left arrow shaped button
    /// </summary>
    public static TextureRegion LeftArrow   { get; } = new(Game1.mouseCursors, new Rectangle(8, 268, 44, 40));

    /// <summary>
    /// 右箭头按钮
    /// Right arrow shaped button
    /// </summary>
    public static TextureRegion RightArrow  { get; } = new(Game1.mouseCursors, new Rectangle(12, 204, 44, 40));

    /// <summary>
    /// 上箭头按钮
    /// Up arrow shaped button
    /// </summary>
    public static TextureRegion UpArrow     { get; } = new(Game1.mouseCursors, new Rectangle(64, 64, 64, 64));

    /// <summary>
    /// 下箭头按钮
    /// Down arrow shaped button
    /// </summary>
    public static TextureRegion DownArrow   { get; } = new(Game1.mouseCursors, new Rectangle(0, 64, 64, 64));

    public static TextureRegion EmptyChkbox { get; } = new(Game1.mouseCursors, new Rectangle(227, 425, 9, 9), true);
    public static TextureRegion FilledChkbox{ get; } = new(Game1.mouseCursors, new Rectangle(236, 425, 9, 9), true);
    public static TextureRegion ExitButton  { get; } = new(Game1.mouseCursors, new Rectangle(337, 494, 12, 12), true);

    public static void Draw(this SpriteBatch batch, Texture2D sheet, Rectangle sprite, int x, int y, 
        int width, int height, Color? color = null)
        => batch.Draw(sheet, new Rectangle(x, y, width, height), sprite, color ?? Color.White);

    public static void Draw(this SpriteBatch batch, TextureRegion textureRegion, int x, int y, 
        int width, int height, Color? color = null)
        => batch.Draw(textureRegion.Texture, textureRegion.Region, x, y, width, height, color);
}