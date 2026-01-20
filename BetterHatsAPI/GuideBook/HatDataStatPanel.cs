using BetterHatsAPI.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Menu;
using UI.Sprite;

namespace BetterHatsAPI.GuideBook;

/// <summary>
/// A panel component that displays cumulative buff stats for hats.
/// </summary>
public class HatDataStatPanel : IClickableMenu, IClickableComponent
{
    private const int RowHeight = StatDisplay.IconSize + StatDisplay.Padding;

    private const int ItemsPerColumn = 4;
    private const int ItemsPerColumnWhenExpanded = 9;
    private const int TotalNumbersOfElements = 18;

    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X { get; set; }

    /// <inheritdoc/>
    public int Y { get; set; }

    /// <inheritdoc/>
    public int Width { get; set; }

    /// <summary>
    /// The height of the panel is dynamic calculated based on
    /// the number of visible stats. So it's not settable.
    /// </summary>
    public int Height
    {
        get => CurrentVisibleCount * RowHeight;
        set { }
    }

    private readonly TextureRegion _backGround = new(Texture.MenuBackground, 161, 39, 32, 32);
    private readonly List<StatDisplay> _stats = new();
    private readonly List<SpriteButton> _buttons = new();

    private readonly SpriteButton _expandPrevButton;
    private readonly SpriteButton _expandNextButton;
    private int _firstVisibleIndex;
    private int _maxFirstVisibleIndex;
    private bool _isExpanded;

    private bool Scrollable => _stats.Count > CurrentVisibleCount;

    private int CurrentVisibleCount =>
        Math.Min(_isExpanded ? ItemsPerColumnWhenExpanded : ItemsPerColumn, _stats.Count);

    public HatDataStatPanel(int x, int y, int width)
    {
        var texture = new TextureRegion(SpriteHelper.CursorSheet, 412, 495, 5, 4, true);
        _expandPrevButton = new SpriteButton(texture, x + 360, y + 64, texture.Width, texture.Height);
        _expandNextButton = new SpriteButton(texture, x + 360, y + 100, texture.Width, texture.Height);

        _expandPrevButton.OnPress += () => ReceiveScrollWheelAction(1);
        _expandNextButton.OnPress += () => ReceiveScrollWheelAction(-1);

        _buttons.Add(_expandPrevButton);
        _buttons.Add(_expandNextButton);

        this.SetDestination(x, y, width, 0);
    }

    /// <summary>
    /// Update the panel with cumulative stats from multiple HatData objects.
    /// </summary>
    public void UpdateStats(HatData data)
    {
        // Update status
        _stats.Clear();
        _firstVisibleIndex = 0;
        _isExpanded = false;

        if (data is null) return;

        // initialize auxiliary variables
        var count = 0;
        for (var i = 0; i < TotalNumbersOfElements; i++)
        {
            // get and filter empty data
            var statValue = data.GetValueByIndex(i);
            if (statValue.ApproximatelyEquals(0))
                continue;

            // determine display text color
            var attrText = data.GetTranslationByIndex(i);
            var color = statValue >= 0 ? Color.DarkGreen : Color.DarkRed;

            // calculate position and position element
            var y = Y + count * RowHeight;
            var display = new StatDisplay(i, attrText, color, X, y);
            _stats.Add(display);

            // count
            count++;
        }

        _maxFirstVisibleIndex = Math.Max(0, _stats.Count - ItemsPerColumnWhenExpanded);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch b)
    {
        // draw background
        if (_isExpanded)
            b.Draw(_backGround, Bounds, Color.LemonChiffon);

        // draw first 4 stats
        var visibleCount = Math.Min(CurrentVisibleCount, _stats.Count - _firstVisibleIndex);
        for (var i = 0; i < visibleCount; i++)
        {
            var statIndex = _firstVisibleIndex + i;
            _stats[statIndex].Draw(b);
        }

        // draw buttons when the number of stats is greater than 4
        if (_stats.Count <= ItemsPerColumn) return;

        // always draw the prev button when expanded
        if (_isExpanded)
            _expandPrevButton.Draw(b);

        // draw the next button when there are more stats to show
        if ((!_isExpanded && Scrollable) || _firstVisibleIndex != _maxFirstVisibleIndex)
            _expandNextButton.DrawFlipVertical(b);
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        if (_stats.Count <= ItemsPerColumn)
            return true;

        foreach (var button in _buttons)
            if (button.ReceiveLeftClick(x, y))
                return true;

        return true;
    }

    public bool ReceiveCursorHover(int x, int y) => Bounds.Contains(x, y);

    public bool ReceiveScrollWheelAction(int amount)
    {
        var cursorPos = Game1.getMousePosition();
        if (!Bounds.Contains(cursorPos))
            return false;

        switch (amount)
        {
            //scroll up when expanded and first visible index is 0, we should collapse the panel
            case > 0 when _isExpanded && _firstVisibleIndex == 0:
                _isExpanded = false;
                return true;
            case < 0 when !_isExpanded && Scrollable:
                _isExpanded = true;
                return true;
        }

        // clamp first visible index
        var newIndex = _firstVisibleIndex - amount;
        _firstVisibleIndex = Math.Clamp(newIndex, 0, _maxFirstVisibleIndex);

        // return directly if the new index is out of range
        if (newIndex > _maxFirstVisibleIndex || newIndex < 0)
            return true;

        // move the position of each stat
        foreach (var stat in _stats)
        {
            var oldX = stat.X;
            var oldY = stat.Y;
            stat.SetPosition(oldX, oldY + amount * RowHeight);
        }
        return true;
    }

    public void Dispose()
    {
        foreach (var button in _buttons)
            button.Dispose();

        _stats.Clear();
        _buttons.Clear();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Auxiliary class for displaying attribute icons and text.
/// </summary>
internal class StatDisplay : IComponent
{
    public const int IconSize = 40;
    public const int Padding = 8;

    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X
    {
        get => _x;
        set
        {
            _label.X += value - _x;
            _x = value;
        }
    }

    private int _x;

    /// <inheritdoc/>
    public int Y
    {
        get => _y;
        set
        {
            _label.Y += value - _y;
            _y = value;
        }
    }

    private int _y;

    /// <inheritdoc/>
    public int Width { get; set; }

    /// <inheritdoc/>
    public int Height { get; set; }

    private readonly TextureRegion _icon;
    private readonly TextLabel _label;

    public StatDisplay(int index, string attr, Color color, int x = 0, int y = 0)
    {
        _x = x;
        _y = y;
        _icon = Texture.GetAttrIconByIndex(index);
        _label = new TextLabel(attr, color, Game1.smallFont);
        Width = IconSize + Padding + _label.Width;
        Height = IconSize + Padding;
        _label.SetPosition(X + IconSize + Padding, Y + (Height - _label.Height) / 2);
    }

    public void Draw(SpriteBatch b)
    {
        b.Draw(_icon, new Rectangle(X, Y + Padding / 2, IconSize, IconSize));
        _label.Draw(b);
    }
}

internal static class SpriteButtonExtension
{
    public static void DrawFlipVertical(this SpriteButton button, SpriteBatch b)
        => b.Draw(button.Texture, button.Bounds, SpriteEffects.FlipVertically);
}