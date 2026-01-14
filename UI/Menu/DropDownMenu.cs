using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Sprite;

namespace UI.Menu;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class DropDownMenu<T> : IClickableMenu, IClickableComponent
{
    public const int ArrowXOffset = 280;
    public const int ArrowYOffset = 8;
    public const int ItemHeight = 40;

    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X
    {
        get => _x;
        set
        {
            _labelPosition.X += value - _x;
            Background.X = value;
            Arrow.X = value + ArrowXOffset;
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
            _labelPosition.Y += value - _y;
            Background.Y = value;
            Arrow.Y = value + ArrowYOffset;
            _y = value;
        }
    }

    private int _y;

    /// <inheritdoc/>
    public int Width { get; set; } = 320;

    /// <summary>
    /// The height of the drop-down menu. It will be automatically
    /// calculated based on whether it is expanded or not.
    /// </summary>
    public int Height
    {
        get => _height + (Expanded ? Math.Min(MaxVisibleOptions, Options.Count) * ItemHeight : 0);
        set => _height = value;
    }

    private int _height = 60;

    public SpriteFont Font;
    public NineSlice Background;

    /// <summary>
    /// The arrow-shaped label that indicates the drop-down state.
    /// </summary>
    public SpriteLabel Arrow;

    /// <summary>
    /// The background texture for the inactive state.
    /// </summary>
    public TextureRegion InactiveBackground;

    /// <summary>
    /// The background texture for the hovered state.
    /// </summary>
    public TextureRegion HoverBackground;

    /// <summary>
    /// The background texture for the active state.
    /// </summary>
    public TextureRegion ActiveBackground;

    /// <summary>
    /// List of options in the drop-down menu.
    /// </summary>
    public List<DropDownOption<T>> Options = new();

    /// <summary>
    /// The action to perform when the selected option changes.
    /// </summary>
    public event Action<T> OnSelectionChanged;

    /// <summary>
    /// Whether the drop-down menu is expanded or not.
    /// </summary>
    public bool Expanded;

    private int _selectedIndex = -1;
    private int _hoveredOptionIndex = -1;
    private int _firstVisibleIndex;
    private Vector2 _labelPosition;

    public T SelectedValue => _selectedIndex >= 0 ? Options[_selectedIndex].Value : default;
    public string SelectedLabel => _selectedIndex >= 0 ? Options[_selectedIndex].Label : "";

    /// <summary>
    /// Maximum visible options, which is calculated based on the
    /// <see cref="_limitVisibleOptions"/> and the viewport height.
    /// </summary>
    public int MaxVisibleOptions => new[]
        { _limitVisibleOptions, (Game1.viewport.Height - Y - _height) / ItemHeight, Options.Count }.Min();

    /// <summary>
    /// Limited visible options, which is set when constructed.
    /// </summary>
    private readonly int _limitVisibleOptions;

    /// <summary>
    /// Initialize a new instance of the <see cref="DropDownMenu{T}"/> class.
    /// Customize its size is too complex, so we use a default size.
    /// </summary>
    public DropDownMenu(int maxHeight, int x = 0, int y = 0)
    {
        _limitVisibleOptions = maxHeight / ItemHeight - 1;
        Font = Game1.smallFont;
        var measure = Font.MeasureString("你好 World");
        var y0 = Bounds.Y + (Bounds.Height - measure.Y) / 2;
        _labelPosition.Y = y0 + 4;
        _labelPosition.X = y0 - Bounds.Y + Bounds.X;

        var defaultBackgroundBound = new Rectangle(X, Y, Width - 32, Height);
        var defaultArrowBound = new Rectangle(X + ArrowXOffset, Y + ArrowYOffset, 40, 44);
        Background = NineSlice.CommonMenu(defaultBackgroundBound);
        Arrow = new SpriteLabel(TextureRegion.DropDownSideArrow(), defaultArrowBound);
        InactiveBackground = TextureRegion.InactiveBackground();
        HoverBackground = TextureRegion.HoverBackground();
        ActiveBackground = TextureRegion.ActiveBackground();
        this.SetPosition(x, y);
    }

    /// <summary>
    /// Add an option to the drop-down menu. If the selected index
    /// is 0, automatically update it.
    /// </summary>
    public void AddOption(string label, T value)
    {
        Options.Add(new DropDownOption<T>(label, value));
        if (_selectedIndex == -1)
            _selectedIndex = 0;
    }

    /// <summary>
    /// Clear all options from the drop-down menu. Then set the
    /// selected index and the first visible index to default.
    /// </summary>
    public void ClearOptions()
    {
        Options.Clear();
        _selectedIndex = -1;
        _firstVisibleIndex = 0;
    }

    /// <summary>
    /// Select the option by its index. Then update the first visible
    /// index. If the index is out of range, it will be clamped to
    /// the valid range.
    /// </summary>
    public void SelectByIndex(int index)
    {
        // Clamp the index to the valid range
        var count = Options.Count;
        if (count == 0) throw new Exception("How did you get here?");

        // Update the selected value
        index = Math.Clamp(index, 0, count - 1);
        _selectedIndex = index;
        OnSelectionChanged?.Invoke(Options[index].Value);

        // Update the first visible index, floating based on the selected index
        var offset = (_selectedIndex + 1.0f) / count;
        var maxFirstIndex = count - MaxVisibleOptions;
        _firstVisibleIndex = (int)(offset * maxFirstIndex);
    }

    public void SelectByValue(T value)
    {
        var index = Options.FindIndex(o => EqualityComparer<T>.Default.Equals(o.Value, value));
        SelectByIndex(index);
    }

    /// <summary>
    /// Select the option by its offset from the current selected index.
    /// </summary>
    /// <seealso cref="SelectByIndex"/>
    public void SelectByOffset(int offset) => SelectByIndex(_selectedIndex + offset);

    /// <summary>
    /// Select the next option in the drop-down menu. 
    /// </summary>
    /// <seealso cref="SelectByIndex"/>
    public void SelectNext() => SelectByIndex(_selectedIndex + 1);


    /// <summary>
    /// Select the previous option in the drop-down menu.
    /// </summary>
    /// <seealso cref="SelectByIndex"/>
    public void SelectPrev() => SelectByIndex(_selectedIndex - 1);

    /// <inheritdoc/>
    public void Draw(SpriteBatch b)
    {
        // 绘制背景，字符和箭头
        Background.Draw(b);
        b.DrawString(Font, SelectedLabel, _labelPosition, Color.Black);
        Arrow.Draw(b);

        if (!Expanded) return;

        var optionsBounds = new Rectangle(X, Y + _height, Width, Height - _height);
        b.Draw(InactiveBackground, optionsBounds);

        // 绘制可见选项
        var visibleCount = Math.Min(MaxVisibleOptions, Options.Count - _firstVisibleIndex);
        for (var i = 0; i < visibleCount; i++)
        {
            var optionIndex = _firstVisibleIndex + i;
            var option = Options[optionIndex];

            // 计算选项位置
            var optionY = optionsBounds.Y + i * ItemHeight;
            var optionBounds = new Rectangle(optionsBounds.X, optionY, optionsBounds.Width, ItemHeight);

            // 绘制活动项效果
            if (optionIndex == _selectedIndex)
                b.Draw(ActiveBackground.Texture, optionBounds, ActiveBackground.Region, Color.White * 0.5f);

            // 绘制悬停效果
            else if (optionIndex == _hoveredOptionIndex)
                b.Draw(HoverBackground.Texture, optionBounds, HoverBackground.Region, Color.White * 0.5f);

            // 绘制选项文本
            var textPosition = new Vector2(
                optionBounds.X + 10,
                optionBounds.Y + (optionBounds.Height - Font.MeasureString(option.Label).Y) / 2
            );
            b.DrawString(Font, option.Label, textPosition, Color.Black);
        }
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        // Check if clicked on this dropdown
        if (Bounds.Contains(x, y) && !Expanded)
        {
            Expanded = !Expanded;
            Game1.playSound("drumkit6");
            return true;
        }

        // Actions when expanded
        if (Expanded)
        {
            var optionsBounds = new Rectangle(X, Y + _height, Width, Height - _height);

            // Check if clicked on an option
            if (optionsBounds.Contains(x, y))
            {
                var clickedIndex = _firstVisibleIndex + (y - optionsBounds.Y) / ItemHeight;
                SelectByIndex(clickedIndex);
                Expanded = false;
                Game1.playSound("drumkit6");
                return true;
            }

            // Otherwise, collapse the dropdown
            Expanded = false;
            return true;
        }

        return false;
    }

    public bool ReceiveCursorHover(int x, int y)
    {
        var optionsBounds = new Rectangle(X, Y + _height, Width, Height - _height);
        if (Expanded && optionsBounds.Contains(x, y))
        {
            _hoveredOptionIndex = _firstVisibleIndex + (y - optionsBounds.Y) / ItemHeight;
            return true;
        }

        _hoveredOptionIndex = -1;
        return false;
    }

    public bool ReceiveScrollWheelAction(int amount)
    {
        if (!Expanded)
            return false;

        // 计算新的第一个可见选项索引
        var newIndex = _firstVisibleIndex - amount;
        var maxFirstIndex = Math.Max(0, Options.Count - MaxVisibleOptions);
        _firstVisibleIndex = Math.Clamp(newIndex, 0, maxFirstIndex);

        return true;
    }

    public void Dispose()
    {
        OnSelectionChanged = null;
        Options.Clear();
    }
}

/// <summary>
/// Auxiliary class for <see cref="DropDownMenu{T}"/>
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class DropDownOption<T>
{
    public string Label { get; set; }
    public T Value { get; set; }

    public DropDownOption(string label, T value)
    {
        Label = label;
        Value = value;
    }
}