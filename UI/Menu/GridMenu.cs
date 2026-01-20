using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Sprite;

namespace UI.Menu;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class GridMenu : IClickableMenu, IClickableComponent
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

    public IComponent Background;
    public List<IComponent> Components = new();

    /// <summary>
    /// Maximum number of columns that can be displayed in the grid.
    /// Items will wrap to a new row when this column limit is reached.
    /// </summary>
    /// <remarks>
    /// If the value of this value plus <see cref="ItemSize"/> exceeds
    /// the width of the grid menu, the value will be adjusted to fit
    /// in the ctor.
    /// </remarks>
    public int MaxItemColumns
    {
        get => IsAndroid && IsScrollingEnabled && ScrollingDirection == ScrollDirection.Vertical
            ? _maxItemColumns - 1
            : _maxItemColumns;
        set => _maxItemColumns = value;
    }

    private int _maxItemColumns;

    /// <summary>
    /// Maximum number of rows that can be displayed in the grid.
    /// If the total number of items exceeds this limit, scrolling will
    /// be enabled.
    /// </summary>
    /// <remarks>
    /// If the value of this value plus <see cref="ItemSize"/> exceeds
    /// the height of the grid menu, the value will be adjusted to fit
    /// in the ctor.
    /// </remarks>
    public int MaxItemRows
    {
        get => IsAndroid && IsScrollingEnabled && ScrollingDirection == ScrollDirection.Horizontal
            ? _maxItemRows - 1
            : _maxItemRows;
        set => _maxItemRows = value;
    }

    private int _maxItemRows;

    /// <summary>
    /// The size of each item displayed in the grid.
    /// </summary>
    public int ItemSize;

    public ScrollDirection ScrollingDirection;

    /// <summary>
    /// The index of the first item to display. Used when drawing or
    /// scrolling the grid.
    /// </summary>
    private int _firstItemIndex;

    public int MaxVisibleItems => MaxItemColumns * MaxItemRows;
    public bool IsScrollingEnabled => Components.Count > _maxItems;

    private readonly int _maxItems;

    # region Android Exclusive

    private static bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;

    /// <summary>
    /// Only used on Android. Should be manually managed.
    /// Used to simulate scroll-wheel action when the elements in
    /// grid-menu are too many.
    /// </summary>
    private SpriteButton _androidExclusiveNextButton;

    /// <summary>
    /// Only used on Android. Should be manually managed.
    /// Used to simulate scroll-wheel action when the elements in
    /// grid-menu are too many.
    /// </summary>
    private SpriteButton _androidExclusivePrevButton;

    #endregion

    /// <summary>
    /// Construct a grid menu.
    /// </summary>
    /// <param name="x">The x-position of this grid-menu.</param>
    /// <param name="y">The y-position of this grid-menu.</param>
    /// <param name="width">The width of this grid-menu.</param>
    /// <param name="height">The height of this grid-menu.</param>
    /// <param name="itemSize">The size of each item displayed in the grid in pixel.</param>
    /// <param name="maxItemColumns">The maximum number of columns that can be displayed in the grid.</param>
    /// <param name="maxItemRows">The maximum number of rows that can be displayed in the grid.</param>
    /// <param name="scrollDirection">The scrolling direction of this grid-menu.</param>
    public GridMenu(int x, int y, int width, int height, int itemSize, int maxItemColumns = 99, int maxItemRows = 99,
        ScrollDirection scrollDirection = ScrollDirection.Vertical)
    {
        this.SetDestination(x, y, width, height);
        MaxItemColumns = maxItemColumns;
        MaxItemRows = maxItemRows;
        ItemSize = itemSize;

        if (itemSize * maxItemColumns > width)
            MaxItemColumns = width / itemSize;
        if (itemSize * maxItemRows > height)
            MaxItemRows = height / itemSize;

        _maxItems = MaxItemColumns * MaxItemRows;
        ScrollingDirection = scrollDirection;

        if (!IsAndroid) return;

        var anchor = this.RightBottomPosition();

        switch (scrollDirection)
        {
            case ScrollDirection.Horizontal:
                _androidExclusiveNextButton = new SpriteButton(TextureRegion.RightArrow());
                _androidExclusivePrevButton = new SpriteButton(TextureRegion.LeftArrow());
                _androidExclusiveNextButton.SetDestination(anchor.X - 50, anchor.Y - 50, 48, 48);
                _androidExclusivePrevButton.SetDestination(anchor.X - 100, anchor.Y - 50, 48, 48);
                break;
            case ScrollDirection.Vertical:
            default:
                _androidExclusiveNextButton = new SpriteButton(TextureRegion.DownArrow());
                _androidExclusivePrevButton = new SpriteButton(TextureRegion.UpArrow());
                _androidExclusiveNextButton.SetDestination(anchor.X - 50, anchor.Y - 50, 48, 48);
                _androidExclusivePrevButton.SetDestination(anchor.X - 50, anchor.Y - 100, 48, 48);
                break;
        }

        _androidExclusiveNextButton.OnPress += () => ReceiveScrollWheelAction(-1);
        _androidExclusivePrevButton.OnPress += () => ReceiveScrollWheelAction(1);
    }

    /// <summary>
    /// Add a component to the grid, then automatically calculates
    /// its initial position. The component will be placed in the
    /// center of its bounds by default.
    /// </summary>
    public void AddComponents(IEnumerable<IComponent> components)
    {
        var index = 0;

        Components.AddRange(components);
        switch (ScrollingDirection)
        {
            case ScrollDirection.Horizontal:
                var maxItemRows = MaxItemRows;
                foreach (var component in Components)
                {
                    var row = index % maxItemRows;
                    var column = index / maxItemRows;
                    var bound = new Rectangle(X + column * ItemSize, Y + row * ItemSize, ItemSize, ItemSize);
                    component.SetInCenterOfTheBounds(bound);
                    index++;
                }

                return;

            case ScrollDirection.Vertical:
            default:
                var maxItemColumns = MaxItemColumns;
                foreach (var component in Components)
                {
                    var column = index % maxItemColumns;
                    var row = index / maxItemColumns;
                    var bound = new Rectangle(X + column * ItemSize, Y + row * ItemSize, ItemSize, ItemSize);
                    component.SetInCenterOfTheBounds(bound);
                    index++;
                }

                return;
        }
    }

    public void RemoveAllComponents()
    {
        Components.Clear();
        _firstItemIndex = 0;
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch b)
    {
        Background?.Draw(b);
        var lastItemIndex = Math.Min(_firstItemIndex + MaxVisibleItems, Components.Count);
        for (var i = _firstItemIndex; i < lastItemIndex; i++)
        {
            var component = Components[i];
            if (Bounds.Contains(component.Bounds))
                component.Draw(b);
        }

        if (IsAndroid && IsScrollingEnabled)
        {
            _androidExclusiveNextButton.Draw(b);
            _androidExclusivePrevButton.Draw(b);
        }
    }

    /// <inheritdoc cref="BaseMenu.ReceiveLeftClick"/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        var lastItemIndex = Math.Min(_firstItemIndex + MaxVisibleItems, Components.Count);
        for (var i = _firstItemIndex; i < lastItemIndex; i++)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            var handled = c.ReceiveLeftClick(x, y);
            if (handled) return true;
        }

        if (IsAndroid && IsScrollingEnabled)
        {
            var handled =
                _androidExclusiveNextButton.ReceiveLeftClick(x, y) ||
                _androidExclusivePrevButton.ReceiveLeftClick(x, y);
            if (handled) return true;
        }

        return false;
    }

    /// <inheritdoc cref="BaseMenu.ReceiveCursorHover"/>
    public virtual bool ReceiveCursorHover(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        var lastItemIndex = Math.Min(_firstItemIndex + MaxVisibleItems, Components.Count);
        for (var i = _firstItemIndex; i < lastItemIndex; i++)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            var handled = c.ReceiveCursorHover(x, y);
            if (handled) return true;
        }

        if (IsAndroid && IsScrollingEnabled)
        {
            var handled =
                _androidExclusiveNextButton.ReceiveCursorHover(x, y) ||
                _androidExclusivePrevButton.ReceiveCursorHover(x, y);
            if (handled) return true;
        }

        return false;
    }

    public virtual bool ReceiveScrollWheelAction(int amount)
    {
        var cursorPos = Game1.getMousePosition();
        if (!Bounds.Contains(cursorPos))
            return false;

        if (Components.Count <= MaxVisibleItems)
            return true;

        int newIndex;
        int maxIndex;
        switch (ScrollingDirection)
        {
            case ScrollDirection.Horizontal:
                newIndex = _firstItemIndex - amount * MaxItemRows;
                maxIndex = ((Components.Count - 1) / MaxItemRows - MaxItemColumns + 1) * MaxItemRows;

                if (newIndex > maxIndex)
                {
                    _firstItemIndex = maxIndex;
                    return true;
                }

                if (newIndex < 0)
                {
                    _firstItemIndex = 0;
                    return true;
                }

                _firstItemIndex = newIndex;
                foreach (var component in Components)
                {
                    var oldX = component.X;
                    var oldY = component.Y;
                    component.SetPosition(oldX + amount * ItemSize, oldY);
                }

                return true;
            case ScrollDirection.Vertical:
            default:
                newIndex = _firstItemIndex - amount * MaxItemColumns;
                maxIndex = ((Components.Count - 1) / MaxItemColumns - MaxItemRows + 1) * MaxItemColumns;

                if (newIndex > maxIndex)
                {
                    _firstItemIndex = maxIndex;
                    return true;
                }

                if (newIndex < 0)
                {
                    _firstItemIndex = 0;
                    return true;
                }

                _firstItemIndex = newIndex;
                foreach (var component in Components)
                {
                    var oldX = component.X;
                    var oldY = component.Y;
                    component.SetPosition(oldX, oldY + amount * ItemSize);
                }

                return true;
        }
    }

    public void Dispose()
    {
        foreach (var component in Components)
            if (component is IDisposable d)
                d.Dispose();

        Components.Clear();

        if (IsAndroid)
        {
            _androidExclusiveNextButton.Dispose();
            _androidExclusivePrevButton.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    public enum ScrollDirection
    {
        Vertical,
        Horizontal
    }
}