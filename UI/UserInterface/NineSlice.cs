using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.UserInterface;

/// <summary>
/// 表示一个九宫格（NineSlice）图形区域，用于在 UI 中灵活拉伸和绘制带有边框的背景。
/// A nine-slice (NineSlice) graphical area used for flexible stretching 
/// and drawing of backgrounds with borders in the UI.
/// </summary>
internal class NineSlice
{
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

    /// <summary>
    /// 按照九宫格方式在指定区域绘制该对象。
    /// Draws this object in the specified area using the nine-slice method.
    /// </summary>
    /// <param name="batch">SpriteBatch 实例。</param>
    /// <param name="bounds">绘制区域。</param>
    public void Draw(SpriteBatch batch, Rectangle bounds)
    {
        // draw background
        batch.Draw(Center,
            bounds.X + Left.Width,
            bounds.Y + Top.Height,
            bounds.Width - Left.Width - Right.Width,
            bounds.Height - Top.Height - Bottom.Height);

        // draw borders
        batch.Draw(Top,
            bounds.X + TopLeft.Width,
            bounds.Y,
            bounds.Width - TopLeft.Width - TopRight.Width,
            Top.Height);
        batch.Draw(Left,
            bounds.X,
            bounds.Y + TopLeft.Height,
            Left.Width,
            bounds.Height - TopLeft.Height - BottomLeft.Height);
        batch.Draw(Right,
            bounds.X + bounds.Width - Right.Width,
            bounds.Y + TopRight.Height,
            Right.Width,
            bounds.Height - TopRight.Height - BottomRight.Height);
        batch.Draw(Bottom,
            bounds.X + BottomLeft.Width,
            bounds.Y + bounds.Height - Bottom.Height,
            bounds.Width - BottomLeft.Width - BottomRight.Width,
            Bottom.Height);

        // draw border joints
        batch.Draw(TopLeft,
            bounds.X,
            bounds.Y,
            TopLeft.Width,
            TopLeft.Height);
        batch.Draw(TopRight,
            bounds.X + bounds.Width - TopRight.Width,
            bounds.Y,
            TopRight.Width,
            TopRight.Height);
        batch.Draw(BottomLeft,
            bounds.X,
            bounds.Y + bounds.Height - BottomLeft.Height,
            BottomLeft.Width,
            BottomLeft.Height);
        batch.Draw(BottomRight,
            bounds.X + bounds.Width - BottomRight.Width,
            bounds.Y + bounds.Height - BottomRight.Height,
            BottomRight.Width,
            BottomRight.Height);
    }
}