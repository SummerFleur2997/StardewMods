using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Sprite;

namespace UI.Menu;

/// <summary>
/// A search box that updates the results in real time as the player types.
/// The results are listed below the input box, and can be picked by mouse,
/// keyboard or the scroll wheel.
/// </summary>
/// <typeparam name="T">The type of the search results.</typeparam>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class SearchBox<T> : IClickableMenu, IClickableComponent
{
    public const int InputHeight = 64;
    public const int ItemHeight = 40;
    private const int RowPaddingX = 10;

    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X
    {
        get => _x;
        set
        {
            InputBox.X = value;
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
            InputBox.Y = value;
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
        get => _height + (Expanded ? Math.Min(MaxVisibleOptions, Results.Count) * ItemHeight : 0);
        set => _height = value;
    }

    private int _height = 60;

    public SpriteFont Font = Game1.smallFont;

    /// <summary>
    /// The input text box of the search box.
    /// </summary>
    public TextBox InputBox;

    /// <summary>
    /// The hint text shown when the input box is empty, forwarded to
    /// the inner <see cref="InputBox"/>.
    /// </summary>
    public string? Placeholder
    {
        get => InputBox.Placeholder;
        set => InputBox.Placeholder = value;
    }

    /// <summary>
    /// The background texture of the result panel.
    /// </summary>
    public TextureRegion ResultsBackground;

    /// <summary>
    /// The background texture for the hovered result row.
    /// </summary>
    public TextureRegion HoverBackground;

    /// <summary>
    /// The background texture for the selected result row.
    /// </summary>
    public TextureRegion ActiveBackground;

    /// <summary>
    /// The search results, which are updated in real time by the
    /// <see cref="SearchProvider"/> when the keyword changes.
    /// </summary>
    public List<SearchResult<T>> Results = new();

    /// <summary>
    /// The search provider, which turns a keyword into a list of results.
    /// The user is expected to provide this method. TODO
    /// </summary>
    public Func<string, IReadOnlyList<SearchResult<T>>>? SearchProvider;

    /// <summary>
    /// The action to perform when a result is selected.
    /// </summary>
    public event Action<T>? OnSelectionChanged;

    /// <summary>
    /// Whether the result panel is expanded.
    /// </summary>
    public bool Expanded;

    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _firstVisibleIndex;

    /// <summary>
    /// The value of the currently selected result, or <c>default</c> if none.
    /// </summary>
    public T? SelectedValue => _selectedIndex >= 0 ? Results[_selectedIndex].Value : default;

    /// <summary>
    /// The label of the currently selected result, or an empty string if none.
    /// </summary>
    public string SelectedLabel => _selectedIndex >= 0 ? Results[_selectedIndex].Label : "";

    /// <summary>
    /// The maximum number of visible results, limited by the viewport height.
    /// </summary>
    public int MaxVisibleOptions => new[]
        { _limitVisibleOptions, (Game1.viewport.Height - Y - _height) / ItemHeight, Results.Count }.Min();

    /// <summary>
    /// Limited visible options, which is set when constructed.
    /// </summary>
    private readonly int _limitVisibleOptions;

    /// <summary>
    /// Initialize a new instance of the <see cref="SearchBox{T}"/> class.
    /// </summary>
    public SearchBox(int maxHeight, int x = 0, int y = 0)
    {
        _limitVisibleOptions = maxHeight / ItemHeight - 1;

        var background = NineSlice.SmallMenuBackground();
        background.SetDestination(x, y, Width, InputHeight);
        InputBox = new TextBox(x, y, Width, InputHeight, "", background);
        InputBox.OnTextChanged += OnInputTextChanged;
        InputBox.OnEnterPressed += OnInputEnterPressed;

        ResultsBackground = TextureRegion.InactiveBackground();
        HoverBackground = TextureRegion.HoverBackground();
        ActiveBackground = TextureRegion.ActiveBackground();
        this.SetPosition(x, y);
    }

    /// <summary>
    /// Search for the given keyword immediately and refresh the results.
    /// </summary>
    public void Refresh(string keyword)
    {
        Results.Clear();
        if (SearchProvider is not null && keyword.Length > 0)
        {
            foreach (var result in SearchProvider(keyword))
                Results.Add(result);
        }

        Expanded = Results.Count > 0;
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _firstVisibleIndex = 0;
    }

    /// <summary>
    /// Collapse the result panel.
    /// </summary>
    public void Collapse()
    {
        Expanded = false;
        _hoveredIndex = -1;
    }

    /// <summary>
    /// Select the result by its index. If the index is out of range,
    /// it will be clamped to the valid range.
    /// </summary>
    public void SelectByIndex(int index)
    {
        if (Results.Count == 0)
            return;

        index = Math.Clamp(index, 0, Results.Count - 1);
        _selectedIndex = index;
        OnSelectionChanged?.Invoke(Results[index].Value);
    }

    /// <summary>
    /// Select the next result.
    /// </summary>
    public void SelectNext() => SelectByIndex(_selectedIndex + 1);

    /// <summary>
    /// Select the previous result.
    /// </summary>
    public void SelectPrev() => SelectByIndex(_selectedIndex - 1);

    /// <summary>
    /// Raised when the text of the input box changes. Performs a real-time search.
    /// </summary>
    private void OnInputTextChanged(TextBox box) => Refresh(box.Text.Trim());

    /// <summary>
    /// Raised when the player presses Enter in the input box.
    /// Selects the current (or the first) result and collapses the panel.
    /// </summary>
    private void OnInputEnterPressed(TextBox box)
    {
        if (!Expanded || Results.Count == 0)
            return;

        SelectByIndex(_selectedIndex >= 0 ? _selectedIndex : 0);
        Collapse();
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch b)
    {
        InputBox.Draw(b);

        if (!Expanded || Results.Count == 0)
            return;

        var resultsBounds = new Rectangle(X, Y + InputHeight, Width, Height - InputHeight);
        ResultsBackground.Draw(b, resultsBounds);

        var visibleCount = Math.Min(MaxVisibleOptions, Results.Count - _firstVisibleIndex);
        for (var i = 0; i < visibleCount; i++)
        {
            var resultIndex = _firstVisibleIndex + i;
            var result = Results[resultIndex];

            var resultY = resultsBounds.Y + i * ItemHeight;
            var resultBounds = new Rectangle(resultsBounds.X, resultY, resultsBounds.Width, ItemHeight);

            // 高亮当前选中 / 悬停的结果行
            if (resultIndex == _selectedIndex)
                b.Draw(ActiveBackground.Texture, resultBounds, ActiveBackground.Region, Color.White * 0.5f);
            else if (resultIndex == _hoveredIndex)
                b.Draw(HoverBackground.Texture, resultBounds, HoverBackground.Region, Color.White * 0.5f);

            // 绘制物品图标（可选）与文本
            var textX = resultBounds.X + RowPaddingX;
            if (result.Item is not null)
            {
                result.Item.drawInMenu(b, new Vector2(textX, resultBounds.Y + 4), 0.5f, 1, 1, StackDrawType.Hide);
                textX += 36;
            }

            var textPosition = new Vector2(
                textX,
                resultBounds.Y + (resultBounds.Height - Font.MeasureString(result.Label).Y) / 2
            );
            b.DrawString(Font, result.Label, textPosition, Color.Black);
        }
    }

    /// <inheritdoc cref="IClickableComponent.ReceiveLeftClick"/>
    public bool ReceiveLeftClick(int x, int y)
    {
        // 点击输入框 -> 聚焦并接收键盘输入
        if (InputBox.Bounds.Contains(x, y))
        {
            InputBox.ReceiveLeftClick(x, y);
            if (InputBox.Selected)
                Game1.keyboardDispatcher.Subscriber = InputBox;
            return true;
        }

        // 展开时：点击结果行 -> 选中并收起；点击其它地方 -> 收起
        if (Expanded)
        {
            var resultsBounds = new Rectangle(X, Y + InputHeight, Width, Height - InputHeight);
            if (resultsBounds.Contains(x, y))
            {
                var clickedIndex = _firstVisibleIndex + (y - resultsBounds.Y) / ItemHeight;
                if (clickedIndex >= 0 && clickedIndex < Results.Count)
                {
                    SelectByIndex(clickedIndex);
                    Collapse();
                    Game1.playSound("drumkit6");
                }

                return true;
            }

            Collapse();
            return true;
        }

        // 未展开且点击外部 -> 取消输入框焦点
        if (InputBox.Selected)
        {
            InputBox.Selected = false;
            Game1.keyboardDispatcher.Subscriber = null;
        }

        return false;
    }

    /// <inheritdoc cref="IClickableComponent.ReceiveCursorHover"/>
    public bool ReceiveCursorHover(int x, int y)
    {
        _hoveredIndex = -1;

        if (!Expanded)
            return false;

        var resultsBounds = new Rectangle(X, Y + InputHeight, Width, Height - InputHeight);
        if (!resultsBounds.Contains(x, y))
            return false;

        var hoveredIndex = _firstVisibleIndex + (y - resultsBounds.Y) / ItemHeight;
        if (hoveredIndex >= 0 && hoveredIndex < Results.Count)
            _hoveredIndex = hoveredIndex;

        return true;
    }

    /// <inheritdoc/>
    public bool ReceiveScrollWheelAction(int amount)
    {
        if (!Expanded)
            return false;

        var newIndex = _firstVisibleIndex - amount;
        var maxFirstIndex = Math.Max(0, Results.Count - MaxVisibleOptions);
        _firstVisibleIndex = Math.Clamp(newIndex, 0, maxFirstIndex);
        return true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The caller should forward key presses to this method while the search
    /// box is focused, e.g. in the menu's <c>ReceiveKeyPress</c>.
    /// </remarks>
    public bool ReceiveKeyPress(Keys key)
    {
        if (!InputBox.Selected)
            return false;

        switch (key)
        {
            case Keys.Down:
                if (Expanded) SelectNext();
                return true;
            case Keys.Up:
                if (Expanded) SelectPrev();
                return true;
            case Keys.Escape:
                if (Expanded)
                {
                    Collapse();
                    Game1.playSound("bigDeSelect");
                    return true;
                }

                InputBox.Selected = false;
                Game1.keyboardDispatcher.Subscriber = null;
                return true;
            default:
                return false;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        InputBox.Dispose();
        Results.Clear();
        SearchProvider = null;
        OnSelectionChanged = null;
    }
}

/// <summary>
/// Auxiliary class for <see cref="SearchBox{T}"/>.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class SearchResult<T>
{
    /// <summary>
    /// The display label of the result.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// The underlying value of the result.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// An optional item used to draw the item icon of the result row.
    /// </summary>
    public Item? Item { get; set; }

    public SearchResult(string label, T value, Item? item = null)
    {
        Label = label;
        Value = value;
        Item = item;
    }
}