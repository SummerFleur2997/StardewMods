using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Sprite;

namespace ConvenientChests.CategorizeChests.Framework.UI;

/// <summary>
/// Customized <see cref="SpriteButton"/> in this mod.
/// </summary>
internal sealed class SpritesButton : IClickableComponent, IDisposable
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

    private readonly NineSlice _background;
    private readonly SpriteLabel _body;
    public event Action OnPress;
    public event Action OnHover;

    private SpritesButton(NineSlice background, SpriteLabel body, int x, int y)
    {
        _background = background;
        _body = body;

        this.SetDestination(x, y, 64, 64);
        _background.SetDestination(Bounds);
        _body.SetInCenterOfTheBounds(Bounds);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch b)
    {
        _background.Draw(b);
        _body.Draw(b);
    }

    /// <inheritdoc/>
    public bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnPress?.Invoke();
        Game1.playSound("drumkit6");
        return true;
    }

    /// <inheritdoc/>
    public bool ReceiveCursorHover(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnHover?.Invoke();
        return true;
    }

    public void Dispose()
    {
        OnPress = null;
        OnHover = null;
    }

    /// <summary>
    /// Create a button with a sprite.
    /// </summary>
    /// <param name="x">The left position of the component in pixels.</param>
    /// <param name="y">The top position of the component in pixels.</param>
    /// <param name="variant">The variant sprite of this button.</param>
    public static SpritesButton CreateButton(int x, int y, SideButtonVariant variant)
    {
        var xOffset = (int)variant * 16;
        var tr = new TextureRegion(UIHelper.Texture, xOffset, 0, 16, 16);
        var background = UIHelper.OrangeButtonBackground();
        var body = new SpriteLabel(tr, width: 48, height: 48);
        return new SpritesButton(background, body, x, y);
    }
}

internal enum SideButtonVariant
{
    Edit = 0,
    Set = 1,
    Manage = 2,
    Save = 3,
    Unlink = 4
}