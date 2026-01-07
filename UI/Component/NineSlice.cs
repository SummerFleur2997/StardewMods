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
    public TextureRegion Top;

    /// <summary>
    /// 左边框区域
    /// The left border of the nine-slice.
    /// </summary>
    public TextureRegion Left;

    /// <summary>
    /// 右边框区域
    /// The right border of the nine-slice.
    /// </summary>
    public TextureRegion Right;

    /// <summary>
    /// 下边框区域
    /// The bottom border of the nine-slice.
    /// </summary>
    public TextureRegion Bottom;

    /// <summary>
    /// 左上角区域
    /// The top-left corner of the nine-slice.
    /// </summary>
    public TextureRegion TopLeft;

    /// <summary>
    /// 右上角区域
    /// The top-right corner of the nine-slice.
    /// </summary>
    public TextureRegion TopRight;

    /// <summary>
    /// 左下角区域
    /// The bottom-left corner of the nine-slice.
    /// </summary>
    public TextureRegion BottomLeft;

    /// <summary>
    /// 右下角区域
    /// The bottom-right corner of the nine-slice.
    /// </summary>
    public TextureRegion BottomRight;

    /// <summary>
    /// 上边框的厚度
    /// The thickness of the top border. 
    /// </summary>
    public int TopBorderThickness => Top.Height;

    /// <summary>
    /// 左边框的厚度
    /// The thickness of the left border.
    /// </summary>
    public int LeftBorderThickness => Left.Width;

    /// <summary>
    /// 右边框的厚度
    /// The thickness of the right border.
    /// </summary>
    public int RightBorderThickness => Right.Width;

    /// <summary>
    /// 下边框的厚度
    /// The thickness of the bottom border. 
    /// </summary>
    public int BottomBorderThickness => Bottom.Height;

    public NineSlice(TextureRegion top, TextureRegion left, TextureRegion right, TextureRegion bottom,
        TextureRegion topLeft, TextureRegion topRight, TextureRegion bottomLeft, TextureRegion bottomRight,
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
        b.Draw(Center, new Rectangle(
            X + Left.Width,
            Y + Top.Height,
            Width - Left.Width - Right.Width,
            Height - Top.Height - Bottom.Height));

        // draw borders
        b.Draw(Top, new Rectangle(
            X + TopLeft.Width,
            Y,
            Width - TopLeft.Width - TopRight.Width,
            Top.Height));
        b.Draw(Left, new Rectangle(
            X,
            Y + TopLeft.Height,
            Left.Width,
            Height - TopLeft.Height - BottomLeft.Height));
        b.Draw(Right, new Rectangle(
            X + Width - Right.Width,
            Y + TopRight.Height,
            Right.Width,
            Height - TopRight.Height - BottomRight.Height));
        b.Draw(Bottom, new Rectangle(
            X + BottomLeft.Width,
            Y + Height - Bottom.Height,
            Width - BottomLeft.Width - BottomRight.Width,
            Bottom.Height));

        // draw border joints 
        b.Draw(TopLeft, new Rectangle(
            X,
            Y,
            TopLeft.Width,
            TopLeft.Height));
        b.Draw(TopRight, new Rectangle(
            X + Width - TopRight.Width,
            Y,
            TopRight.Width,
            TopRight.Height));
        b.Draw(BottomLeft, new Rectangle(
            X,
            Y + Height - BottomLeft.Height,
            BottomLeft.Width,
            BottomLeft.Height));
        b.Draw(BottomRight, new Rectangle(
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
    /// Tooltip 的九宫格精灵图
    /// Nine-slice sprite of a tooltip
    /// </summary>
    public static NineSlice TooltipBackground(Rectangle bounds = new()) => new(
        new TextureRegion(CursorSheet, 297, 360, 16, 4, true),
        new TextureRegion(CursorSheet, 293, 364, 4, 16, true),
        new TextureRegion(CursorSheet, 313, 364, 4, 16, true),
        new TextureRegion(CursorSheet, 297, 380, 16, 4, true),
        new TextureRegion(CursorSheet, 293, 360, 4, 4, true),
        new TextureRegion(CursorSheet, 313, 360, 4, 4, true),
        new TextureRegion(CursorSheet, 293, 380, 4, 4, true),
        new TextureRegion(CursorSheet, 313, 380, 4, 4, true),
        new TextureRegion(CursorSheet, 297, 364, 16, 16, true),
        bounds
    );

    /// <summary>
    /// 左悬浮标签的九宫格精灵图
    /// Nine-slice sprite of a left-protruding tab.
    /// </summary>
    public static NineSlice LeftProtrudingTab(Rectangle bounds = new()) => new(
        new TextureRegion(CursorSheet, 661, 64, 1, 4, true),
        new TextureRegion(CursorSheet, 656, 69, 5, 1, true),
        new TextureRegion(CursorSheet, 670, 68, 2, 1, true),
        new TextureRegion(CursorSheet, 661, 76, 1, 4, true),
        new TextureRegion(CursorSheet, 656, 64, 5, 5, true),
        new TextureRegion(CursorSheet, 670, 64, 2, 5, true),
        new TextureRegion(CursorSheet, 656, 75, 5, 5, true),
        new TextureRegion(CursorSheet, 670, 75, 2, 5, true),
        new TextureRegion(CursorSheet, 661, 68, 1, 1, true),
        bounds
    );

    /// <summary>
    /// 普通菜单的九宫格精灵图。
    /// Nine-slice sprite of a common menu.
    /// </summary>
    public static NineSlice CommonMenu(Rectangle bounds = new()) => new(
        new TextureRegion(MenuSheet, 16, 256, 28, 16),
        new TextureRegion(MenuSheet, 0, 272, 16, 28),
        new TextureRegion(MenuSheet, 48, 272, 16, 28),
        new TextureRegion(MenuSheet, 16, 300, 28, 16),
        new TextureRegion(MenuSheet, 0, 256, 16, 16),
        new TextureRegion(MenuSheet, 48, 256, 16, 16),
        new TextureRegion(MenuSheet, 0, 300, 16, 16),
        new TextureRegion(MenuSheet, 48, 300, 16, 16),
        new TextureRegion(MenuSheet, 16, 272, 28, 28),
        bounds
    );
}