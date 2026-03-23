using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Sprite;
using static UI.Sprite.SpriteHelper;

namespace UI.Component;

/// <summary>
/// 表示一个九宫格（NineSlice）图形区域，用于在 UI 中灵活拉伸和绘制带有边框的背景。
/// A nine-slice (NineSlice) graphical area used for flexible stretching 
/// and drawing of backgrounds with borders in the UI.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class NineSlice : IComponent
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

    /// <summary>
    /// 中心区域
    /// The center region of the nine-slice.
    /// </summary>
    public TextureRegion Center;

    /// <summary>
    /// 上边框区域
    /// The top border of the nine-slice.
    /// </summary>
    public TextureRegion? Top;

    /// <summary>
    /// 左边框区域
    /// The left border of the nine-slice.
    /// </summary>
    public TextureRegion? Left;

    /// <summary>
    /// 右边框区域
    /// The right border of the nine-slice.
    /// </summary>
    public TextureRegion? Right;

    /// <summary>
    /// 下边框区域
    /// The bottom border of the nine-slice.
    /// </summary>
    public TextureRegion? Bottom;

    /// <summary>
    /// 左上角区域
    /// The top-left corner of the nine-slice.
    /// </summary>
    public TextureRegion? TopLeft;

    /// <summary>
    /// 右上角区域
    /// The top-right corner of the nine-slice.
    /// </summary>
    public TextureRegion? TopRight;

    /// <summary>
    /// 左下角区域
    /// The bottom-left corner of the nine-slice.
    /// </summary>
    public TextureRegion? BottomLeft;

    /// <summary>
    /// 右下角区域
    /// The bottom-right corner of the nine-slice.
    /// </summary>
    public TextureRegion? BottomRight;

    /// <summary>
    /// 上边框的厚度
    /// The thickness of the top border. 
    /// </summary>
    public int TopBorderThickness => Top?.Height ?? 0;

    /// <summary>
    /// 左边框的厚度
    /// The thickness of the left border.
    /// </summary>
    public int LeftBorderThickness => Left?.Width ?? 0;

    /// <summary>
    /// 右边框的厚度
    /// The thickness of the right border.
    /// </summary>
    public int RightBorderThickness => Right?.Width ?? 0;

    /// <summary>
    /// 下边框的厚度
    /// The thickness of the bottom border. 
    /// </summary>
    public int BottomBorderThickness => Bottom?.Height ?? 0;

    public NineSlice(TextureRegion? top, TextureRegion? left, TextureRegion? right, TextureRegion? bottom,
        TextureRegion? topLeft, TextureRegion? topRight, TextureRegion? bottomLeft, TextureRegion? bottomRight,
        TextureRegion center, Rectangle bounds)
    {
        Top = top;
        Left = left;
        Right = right;
        Bottom = bottom;
        TopLeft = topLeft;
        TopRight = topRight;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;
        Center = center;
        this.SetDestination(bounds);
    }

    /// <summary>
    /// 按照九宫格方式在指定区域绘制该对象。
    /// Draws this object in the center of the boundary.
    /// </summary>
    public void Draw(SpriteBatch b)
    {
        // draw background
        Center.Draw(b, new Rectangle(
            X + LeftBorderThickness,
            Y + TopBorderThickness,
            Width - LeftBorderThickness - RightBorderThickness,
            Height - TopBorderThickness - BottomBorderThickness));

        // draw borders
        Top?.Draw(b, new Rectangle(
            X + (TopLeft?.Width ?? 0),
            Y,
            Width - (TopLeft?.Width ?? 0) - (TopRight?.Width ?? 0),
            TopBorderThickness));

        Left?.Draw(b, new Rectangle(
            X,
            Y + (TopLeft?.Height ?? 0),
            LeftBorderThickness,
            Height - (TopLeft?.Height ?? 0) - (BottomLeft?.Height ?? 0)));

        Right?.Draw(b, new Rectangle(
            X + Width - RightBorderThickness,
            Y + (TopRight?.Height ?? 0),
            RightBorderThickness,
            Height - (TopRight?.Height ?? 0) - (BottomRight?.Height ?? 0)));

        Bottom?.Draw(b, new Rectangle(
            X + (BottomLeft?.Width ?? 0),
            Y + Height - BottomBorderThickness,
            Width - (BottomLeft?.Width ?? 0) - (BottomRight?.Width ?? 0),
            BottomBorderThickness));

        // draw border joints 
        TopLeft?.Draw(b, new Rectangle(
            X,
            Y,
            TopLeft.Width,
            TopLeft.Height));

        TopRight?.Draw(b, new Rectangle(
            X + Width - TopRight.Width,
            Y,
            TopRight.Width,
            TopRight.Height));

        BottomLeft?.Draw(b, new Rectangle(
            X,
            Y + Height - BottomLeft.Height,
            BottomLeft.Width,
            BottomLeft.Height));

        BottomRight?.Draw(b, new Rectangle(
            X + Width - BottomRight.Width,
            Y + Height - BottomRight.Height,
            BottomRight.Width,
            BottomRight.Height));
    }

    /// <summary>
    /// 菜单的九宫格精灵图
    /// Nine-slice sprite of a menu
    /// </summary>
    public static NineSlice MenuBackground(Rectangle bounds = new()) => new(
        new TextureRegion(MenuSheet, 40, 12, 1, 24),
        new TextureRegion(MenuSheet, 12, 36, 24, 1),
        new TextureRegion(MenuSheet, 220, 40, 24, 1),
        new TextureRegion(MenuSheet, 36, 220, 1, 24),
        new TextureRegion(MenuSheet, 12, 12, 24, 24),
        new TextureRegion(MenuSheet, 220, 12, 24, 24),
        new TextureRegion(MenuSheet, 12, 220, 24, 24),
        new TextureRegion(MenuSheet, 220, 220, 24, 24),
        new TextureRegion(MenuSheet, 64, 128, 64, 64),
        bounds
    );

    /// <summary>
    /// 普通菜单的九宫格精灵图。
    /// Nine-slice sprite of a common menu.
    /// </summary>
    public static NineSlice SmallMenuBackground(Rectangle bounds = new()) => new(
        new TextureRegion(MenuSheet, 16, 256, 28, 16),
        new TextureRegion(MenuSheet, 0, 272, 16, 28),
        new TextureRegion(MenuSheet, 44, 272, 16, 28),
        new TextureRegion(MenuSheet, 16, 300, 28, 16),
        new TextureRegion(MenuSheet, 0, 256, 16, 16),
        new TextureRegion(MenuSheet, 44, 256, 16, 16),
        new TextureRegion(MenuSheet, 0, 300, 16, 16),
        new TextureRegion(MenuSheet, 44, 300, 16, 16),
        new TextureRegion(MenuSheet, 16, 272, 28, 28),
        bounds
    );
}