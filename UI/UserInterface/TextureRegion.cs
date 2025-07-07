using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace UI.UserInterface;

/// <summary>
/// 用于记录组成 <see cref="NineSlice"/> 组件纹理的区域
/// Represents the region of a texture that makes up a <see cref="NineSlice"/> component
/// </summary>
internal class TextureRegion
{
    /// <summary>
    /// 纹理
    /// The texture
    /// </summary>
    public readonly Texture2D Texture;

    /// <summary>
    /// 该组件在游戏纹理资源文件中的范围，以正方形的形式表示
    /// The region of the component in the game texture resource file, 
    /// represented as a rectangle
    /// </summary>
    public readonly Rectangle Region;

    /// <summary>
    /// 该纹理是否可拉伸
    /// Whether the texture can be zoomed.
    /// </summary>
    private readonly bool _zoomable;

    public TextureRegion(Texture2D texture, Rectangle region, bool zoomable=false)
    {
        Texture = texture;
        Region = region;
        _zoomable = zoomable;
    }

    public int Width => Region.Width * (_zoomable ? Game1.pixelZoom : 1);
    public int Height => Region.Height * (_zoomable ? Game1.pixelZoom : 1);
}