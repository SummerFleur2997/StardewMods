using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using static UI.Sprite.SpriteHelper;

namespace UI.Sprite;

/// <summary>
/// 用于记录组成组件纹理的区域
/// Represents the region of a texture.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class TextureRegion
{
    /// <summary>
    /// 游戏纹理资源。
    /// The texture resource in the game.
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// 该组件在游戏纹理资源文件中的范围，以正方形的形式表示
    /// The region of the component in the game texture resource file, 
    /// represented as a rectangle
    /// </summary>
    public Rectangle Region;

    /// <summary>
    /// 该纹理是否可拉伸
    /// Whether the texture can be zoomed.
    /// </summary>
    private bool _zoomable;

    public int Width => Region.Width * (_zoomable ? Game1.pixelZoom : 1);
    public int Height => Region.Height * (_zoomable ? Game1.pixelZoom : 1);

    public TextureRegion(Texture2D texture, int x, int y, int width, int height, bool zoomable = false)
    {
        Texture = texture;
        Region = new Rectangle(x, y, width, height);
        _zoomable = zoomable;
    }

    public static TextureRegion LeftArrow() => new(CursorSheet, 8, 268, 44, 40);
    public static TextureRegion RightArrow() => new(CursorSheet, 12, 204, 44, 40);
    public static TextureRegion UpArrow() => new(CursorSheet, 64, 64, 64, 64);
    public static TextureRegion DownArrow() => new(CursorSheet, 0, 64, 64, 64);
    public static TextureRegion EmptyCheckbox() => new(CursorSheet, 227, 425, 9, 9, true);
    public static TextureRegion FilledCheckbox() => new(CursorSheet, 236, 425, 9, 9, true);
    public static TextureRegion MenuBackGround() => new(MenuSheet, 0, 256, 60, 60);
    public static TextureRegion HoverBackground() => new(CursorSheet, 161, 340, 4, 4, true);
    public static TextureRegion ActiveBackground() => new(CursorSheet, 258, 258, 4, 4, true);
    public static TextureRegion InactiveBackground() => new(CursorSheet, 269, 258, 4, 4, true);
    public static TextureRegion DropDownSideArrow() => new(CursorSheet, 437, 450, 10, 11, true);
}