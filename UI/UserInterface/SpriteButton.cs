using Microsoft.Xna.Framework.Graphics;

namespace UI.UserInterface;

/// <summary>
/// A button that uses a single textureRegion to display itself.
/// </summary>
internal class SpriteButton : Button
{
    private readonly TextureRegion _textureRegion;

    private bool Visible { get; set; } = true;

    public SpriteButton(TextureRegion textureRegion)
    {
        _textureRegion = textureRegion;
        Width = _textureRegion.Width;
        Height = _textureRegion.Height;
    }

    public override void Draw(SpriteBatch batch)
    {
        if (!Visible)
            return;

        batch.Draw(_textureRegion.Texture, _textureRegion.Region, GlobalPosition.X, GlobalPosition.Y,
            _textureRegion.Width, _textureRegion.Height);
    }
}