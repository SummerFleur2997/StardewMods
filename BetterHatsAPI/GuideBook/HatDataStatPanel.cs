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
    public const int RowHeight = StatDisplay.IconSize + StatDisplay.Padding;
    public const int ItemsPerColumn = 4;

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

    private readonly List<StatDisplay> _stats = new();
    private int _firstVisibleIndex;

    public HatDataStatPanel(int x, int y, int width, int height) => this.SetDestination(x, y, width, height);

    /// <summary>
    /// Update the panel with cumulative stats from multiple HatData objects.
    /// </summary>
    public void UpdateStats(HatData data)
    {
        // Update cached data
        _stats.Clear();
        if (data is null) return;

        // Recalculate stats.
        for (var i = 0; i < 18; i++)
        {
            var statValue = data.GetValueByIndex(i);
            if (statValue.ApproximatelyEquals(0))
                continue;
            var attrText = data.GetTranslationByIndex(i);
            var color = statValue >= 0 ? Color.DarkGreen : Color.DarkRed;
            var display = new StatDisplay(i, attrText, color);
            _stats.Add(display);
        }
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch b)
    {
        // Draw all stats
        var visibleCount = Math.Min(ItemsPerColumn, _stats.Count - _firstVisibleIndex);
        for (var i = 0; i < visibleCount; i++)
        {
            var statIndex = _firstVisibleIndex + i;
            var stat = _stats[statIndex];

            stat.SetPosition(X, Y + i * RowHeight);
            stat.Draw(b);
        }
    }

    public bool ReceiveLeftClick(int x, int y) => Bounds.Contains(x, y);

    public bool ReceiveCursorHover(int x, int y) => true;

    public bool ReceiveScrollWheelAction(int amount)
    {
        var newIndex = _firstVisibleIndex - amount;
        var maxFirstIndex = Math.Max(0, _stats.Count - ItemsPerColumn);
        _firstVisibleIndex = Math.Clamp(newIndex, 0, maxFirstIndex);

        return true;
    }

    public void Dispose() => _stats.Clear();
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