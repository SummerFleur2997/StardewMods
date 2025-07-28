using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace UI.UserInterface;

/// <summary>
/// 从游戏 .xnb 文件中获取到的各个 UI 元素的精灵图
/// UI elements' sprites extracted from the game's .xnb files
/// </summary>
internal static class Sprites
{
    public static readonly Texture2D CursorSheet = Game1.mouseCursors;
    public static readonly Texture2D MenuSheet = Game1.menuTexture;

    public static readonly NineSlice TabBackground = new()
    {
        Left        = new TextureRegion(CursorSheet, 0,  388, 3, 1, true),
        Right       = new TextureRegion(CursorSheet, 13, 388, 3, 1, true),
        Bottom      = new TextureRegion(CursorSheet, 4,  397, 1, 3, true),
        TopLeft     = new TextureRegion(CursorSheet, 0,  384, 5, 5, true),
        TopRight    = new TextureRegion(CursorSheet, 11, 384, 5, 5, true),
        BottomLeft  = new TextureRegion(CursorSheet, 0,  395, 5, 5, true),
        BottomRight = new TextureRegion(CursorSheet, 11, 395, 5, 5, true),
        Center      = new TextureRegion(CursorSheet, 5,  387, 1, 1, true)
    };

    /// <summary>
    /// 菜单的九宫格精灵图
    /// Nine-slice sprite of a menu
    /// </summary>
    public static readonly NineSlice MenuBackground = new()
    {
        Top         = new TextureRegion(MenuSheet, 40,  12,  1,  24),
        Left        = new TextureRegion(MenuSheet, 12,  36,  24, 1),
        Right       = new TextureRegion(MenuSheet, 220, 40,  24, 1),
        Bottom      = new TextureRegion(MenuSheet, 36,  220, 1,  24),
        TopLeft     = new TextureRegion(MenuSheet, 12,  12,  24, 24),
        TopRight    = new TextureRegion(MenuSheet, 220, 12,  24, 24),
        BottomLeft  = new TextureRegion(MenuSheet, 12,  220, 24, 24),
        BottomRight = new TextureRegion(MenuSheet, 220, 220, 24, 24),
        Center      = new TextureRegion(MenuSheet, 64,  128, 64, 64)
    };

    /// <summary>
    /// Tooltip 的九宫格精灵图
    /// Nine-slice sprite of a tooltip
    /// </summary>
    public static readonly NineSlice TooltipBackground = new()
    {
        Top         = new TextureRegion(CursorSheet, 297, 360, 16, 4,  true),
        Left        = new TextureRegion(CursorSheet, 293, 364, 4,  16, true),
        Right       = new TextureRegion(CursorSheet, 313, 364, 4,  16, true),
        Bottom      = new TextureRegion(CursorSheet, 297, 380, 16, 4,  true),
        TopLeft     = new TextureRegion(CursorSheet, 293, 360, 4,  4,  true),
        TopRight    = new TextureRegion(CursorSheet, 313, 360, 4,  4,  true),
        BottomLeft  = new TextureRegion(CursorSheet, 293, 380, 4,  4,  true),
        BottomRight = new TextureRegion(CursorSheet, 313, 380, 4,  4,  true),
        Center      = new TextureRegion(CursorSheet, 297, 364, 16, 16, true)
    };

    /// <summary>
    /// 左悬浮按钮的九宫格精灵图
    /// Nine-slice sprite of a left-protruding tab button
    /// </summary>
    public static readonly NineSlice LeftProtrudingTab = new()
    {
        Top         = new TextureRegion(CursorSheet, 661, 64, 1, 4, true),
        Left        = new TextureRegion(CursorSheet, 656, 69, 5, 1, true),
        Right       = new TextureRegion(CursorSheet, 670, 68, 2, 1, true),
        Bottom      = new TextureRegion(CursorSheet, 661, 76, 1, 4, true),
        TopLeft     = new TextureRegion(CursorSheet, 656, 64, 5, 5, true),
        TopRight    = new TextureRegion(CursorSheet, 670, 64, 2, 5, true),
        BottomLeft  = new TextureRegion(CursorSheet, 656, 75, 5, 5, true),
        BottomRight = new TextureRegion(CursorSheet, 670, 75, 2, 5, true),
        Center      = new TextureRegion(CursorSheet, 661, 68, 1, 1, true)
    };

    /// <summary>左箭头按钮 Left arrow shaped button</summary>
    public static readonly TextureRegion LeftArrow  = new(CursorSheet, 8,  268, 44, 40);

    /// <summary>右箭头按钮R ight arrow shaped button</summary>
    public static readonly TextureRegion RightArrow = new(CursorSheet, 12, 204, 44, 40);

    /// <summary>上箭头按钮 Up arrow shaped button</summary>
    public static readonly TextureRegion UpArrow    = new(CursorSheet, 64, 64, 64, 64);

    /// <summary>下箭头按钮 Down arrow shaped button</summary>
    public static readonly TextureRegion DownArrow  = new(CursorSheet, 0,  64, 64, 64);

    public static readonly TextureRegion EmptyChkbox  = new(CursorSheet, 227, 425, 9,  9,  true);
    public static readonly TextureRegion FilledChkbox = new(CursorSheet, 236, 425, 9,  9,  true);
    public static readonly TextureRegion ExitButton   = new(CursorSheet, 337, 494, 12, 12, true);

    public static readonly TextureRegion HoverBackground = 
        new(CursorSheet, 161, 340, 4, 4, true);
    public static readonly TextureRegion ActiveBackground = 
        new(CursorSheet, 258, 258, 4, 4, true);
    public static readonly TextureRegion InactiveBackground = 
        new(CursorSheet, 269, 258, 4, 4, true);

    public static void Draw(this SpriteBatch batch, Texture2D sheet, Rectangle sprite, int x, int y, 
        int width, int height, Color? color = null)
        => batch.Draw(sheet, new Rectangle(x, y, width, height), sprite, color ?? Color.White);

    public static void Draw(this SpriteBatch batch, TextureRegion texture, int x, int y, 
        int width, int height, Color? color = null)
        => batch.Draw(texture.Texture, texture.Region, x, y, width, height, color);

    public static void Draw(this SpriteBatch batch, TextureRegion texture, Rectangle destination, Color? color = null)
        => batch.Draw(texture.Texture, destination, texture.Region, color ?? Color.White);
}