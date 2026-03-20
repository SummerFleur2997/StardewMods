#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Sprite;

namespace ConvenientChests.CategorizeChests.UI;

public class ChestInfoBubble
{
    private const int TailSize = 20;

    public Item? Item { get; private set; }

    public string? Text
    {
        get => string.IsNullOrEmpty(_text) ? null : _text;
        private set => _text = value ?? "";
    }

    private string _text = "";

    private int _x;
    private int _y;
    private const int Height = 72;

    private readonly int _textYOffset;
    private readonly NineSlice _body;
    private readonly TextureRegion _tail;
    private readonly SpriteFont _font;

    public ChestInfoBubble(SpriteFont font)
    {
        _font = font;
        _body = UIHelper.TextBubble();
        _body.SetSize(Height, Height);
        _tail = new TextureRegion(UIHelper.Cursors_1_6, 251, 506, 5, 5, true);

        var height = (int)_font.MeasureString("M").Y;
        _textYOffset = (Height - height) / 2 + 1;
    }

    public void Draw(SpriteBatch b)
    {
        _body.Draw(b);
        var x = _x + 4;
        if (Item is not null)
        {
            Item.drawInMenu(b, new Vector2(x, _y + 4), 0.75f);
            x += 48;
        }

        x += 16;
        if (Text is not null) b.DrawString(_font, _text, new Vector2(x, _y + _textYOffset), Color.Black);

        b.Draw(_tail, new Rectangle(_x + 20, _y + 68, TailSize, TailSize));
    }

    public void Set(Item? item, string? alias)
    {
        Item = item;
        Text = alias;

        var width = 32;

        if (item is not null) width += 40;

        if (!string.IsNullOrEmpty(alias)) width += (int)_font.MeasureString(alias).X + 8;

        _body.SetSize(width, Height);
    }

    public void UpdatePosition(Vector2 position)
    {
        _x = (int)position.X + 2;
        _y = (int)position.Y - Height - TailSize;
        _body.SetPosition(_x, _y);
    }
}