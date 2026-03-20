using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace UI.Component;

public class TextBox : IClickableComponent, IKeyboardSubscriber, IDisposable
{
    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X { get; set; }

    /// <inheritdoc/>
    public int Y { get; set; }

    /// <summary>
    /// The width of the label, which should be automatically
    /// calculated and avoid to be changed manually.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the label, which should be automatically
    /// calculated and avoid to be changed manually.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Whether the text box is selected.
    /// </summary>
    public bool Selected { get; set; }

    /// <summary>
    /// The inner text of the text box.
    /// </summary>
    public string Text { get; private set; }

    public event Action<TextBox> OnEnterPressed;

    private readonly IComponent _background;
    private readonly SpriteFont _font;

    public TextBox(int x, int y, int width, int height, string text, SpriteFont font, IComponent background)
    {
        Text = text;
        this.SetDestination(x, y, width, height);
        _background = background;
        _background.SetDestination(x, y, width, height);
        _font = font;
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        Selected = Bounds.Contains(x, y);
        return Selected;
    }

    public bool ReceiveCursorHover(int x, int y) => Bounds.Contains(x, y);

    public void Draw(SpriteBatch b)
    {
        _background.Draw(b);
        var caretVisible = Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 >= 500.0;

        var toDraw = Text ?? "";
        var size = _font.MeasureString(toDraw);
        while (size.X > Width - 32)
        {
            toDraw = toDraw[1..];
            size = _font.MeasureString(toDraw);
        }

        if (caretVisible && Selected)
            b.Draw(Game1.staminaRect, new Rectangle(X + 16 + (int)size.X + 2, Y + 16, 4, 32), Color.Black);
        b.DrawString(_font, toDraw, new Vector2(X + 16, Y + 16), Color.Black);
    }

    public void RecieveTextInput(char inputChar)
    {
        if (!Selected) return;
        Text += inputChar;
    }

    public void RecieveTextInput(string text)
    {
        if (!Selected) return;
        Text += text;
    }

    public void RecieveCommandInput(char command)
    {
        if (!Selected) return;

        switch (command)
        {
            case '\b':
                if (Text.Length <= 0)
                    return;

                Text = Text[..^1];
                return;
            case '\r':
                OnEnterPressed?.Invoke(this);
                return;
        }
    }

    public void RecieveSpecialInput(Keys key) { }

    public void Dispose()
    {
        OnEnterPressed = null;
        Game1.keyboardDispatcher.Subscriber = null;
        GC.SuppressFinalize(this);
    }
}