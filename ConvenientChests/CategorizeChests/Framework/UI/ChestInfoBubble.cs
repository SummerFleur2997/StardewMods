using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Sprite;

namespace ConvenientChests.CategorizeChests.Framework.UI;

public class ChestInfoBubble
{
    private const int TailSize = 20;

    public Item Item;

    private int _x;
    private int _y;
    private const int Height = 72;

    private readonly TextLabel _label;
    private readonly NineSlice _body;
    private readonly TextureRegion _tail;

    public ChestInfoBubble()
    {
        _label = new TextLabel("", Color.Black, Game1.smallFont);

        var cursor = Game1.mouseCursors_1_6;
        _body = new NineSlice(
            new TextureRegion(cursor, 244, 503, 3, 3, true),
            new TextureRegion(cursor, 241, 506, 3, 3, true),
            new TextureRegion(cursor, 247, 506, 3, 3, true),
            new TextureRegion(cursor, 244, 509, 3, 3, true),
            new TextureRegion(cursor, 241, 503, 3, 3, true),
            new TextureRegion(cursor, 247, 503, 3, 3, true),
            new TextureRegion(cursor, 241, 509, 3, 3, true),
            new TextureRegion(cursor, 247, 509, 3, 3, true),
            new TextureRegion(cursor, 244, 506, 3, 3, true),
            new Rectangle()
        );
        _body.SetSize(Height, Height);
        _tail = new TextureRegion(cursor, 251, 506, 5, 5, true);
    }

    public void Draw(SpriteBatch b)
    {
        _body.Draw(b);
        _label.Draw(b);
        Item?.drawInMenu(b, new Vector2(_x + 4, _y + 4), 0.75f);
        b.Draw(_tail, new Rectangle(_x + 20, _y + 68, TailSize, TailSize));
    }

    public void SetText(string text)
    {
        _label.Text = text;
        _body.Width = _label.Width + 80;
    }

    public void UpdatePosition(Vector2 position)
    {
        _x = (int)position.X + 2;
        _y = (int)position.Y - Height - TailSize;
        _body.SetPosition(_x, _y);
        _label.SetAtLeftCenterWithEqualMargins(_body.Bounds);
        _label.OffsetPosition(48);
    }
}