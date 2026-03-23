using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;

namespace UI.Menu;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class StackPanel : IClickableMenu, IClickableComponent
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

    public IComponent? Background;
    private List<IComponent> _components = new();

    /// <summary>
    /// The height of each item displayed in the stack.
    /// </summary>
    public int ItemHeight;

    /// <summary>
    /// The width of each item displayed in the stack.
    /// If not set, defaults to the width of the StackPanel.
    /// </summary>
    public int ItemWidth;

    /// <summary>
    /// An event triggered when an element moves to the next position.
    /// </summary>
    public event Action<int>? OnMoveNext;

    /// <summary>
    /// An event triggered when an element moves to the previous position.
    /// </summary>
    public event Action<int>? OnMovePrev;

    /// <summary>
    /// An event triggered when an element was deleted.
    /// </summary>
    public event Action<int>? OnDeleteItem;

    /// <summary>
    /// The index of the first item to display. Used when drawing or scrolling.
    /// </summary>
    private int _firstItemIndex;

    public int MaxVisibleItems => Height / ItemHeight;
    public bool IsScrollingEnabled => _components.Count > MaxVisibleItems;

    /// <summary>
    /// Construct a stack panel.
    /// </summary>
    /// <param name="x">The x-position of this stack panel.</param>
    /// <param name="y">The y-position of this stack panel.</param>
    /// <param name="width">The width of this stack panel.</param>
    /// <param name="height">The height of this stack panel.</param>
    /// <param name="itemHeight">The height of each item in the stack.</param>
    /// <param name="itemWidth">The width of each item in the stack. Defaults to panel width if null.</param>
    public StackPanel(int x, int y, int width, int height, int itemHeight, int? itemWidth = null)
    {
        this.SetDestination(x, y, width, height);
        ItemHeight = itemHeight;
        ItemWidth = itemWidth ?? width;
    }

    /// <summary>
    /// Add components to the stack, then automatically calculates
    /// its initial position. The component will be placed in the
    /// center of its bounds by default.
    /// </summary>
    public void AddComponents(IEnumerable<IComponent> components)
    {
        _components.AddRange(components);

        // Recalculate positions for all items to ensure consistency
        var index = 0;
        foreach (var component in _components)
        {
            // StackPanel has only 1 column. Items are arranged vertically.
            var bound = new Rectangle(X, Y + index * ItemHeight, ItemWidth, ItemHeight);
            component.SetInCenterOfTheBounds(bound);
            index++;
        }
    }

    /// <inheritdoc cref="AddComponents(IEnumerable{IComponent})"/>
    public void AddComponents(params IComponent[] components) => AddComponents(components.AsEnumerable());

    public void RemoveAllComponents()
    {
        _components.Clear();
        _firstItemIndex = 0;
    }

    /// <summary>
    /// Moves the item at the specified index down by one position
    /// (swaps with the next item).
    /// </summary>
    /// <param name="index">The index of the item to move.</param>
    public void MoveNext(int index)
    {
        // Check if the index is valid and not the last item
        if (index < 0 || index >= _components.Count - 1) return;

        var current = _components[index];
        var next = _components[index + 1];

        _components[index] = next;
        _components[index + 1] = current;

        var temp = current.Position();
        current.SetPosition(next.X, next.Y);
        next.SetPosition(temp);
        OnMoveNext?.Invoke(index);
    }

    /// <summary>
    /// Moves the item at the specified index up by one position
    /// (swaps with the previous item).
    /// </summary>
    /// <param name="index">The index of the item to move.</param>
    public void MovePrev(int index)
    {
        // Check if the index is valid and not the first item
        if (index <= 0 || index >= _components.Count) return;

        var current = _components[index];
        var prev = _components[index - 1];

        _components[index] = prev;
        _components[index - 1] = current;

        var temp = current.Position();
        current.SetPosition(prev.X, prev.Y);
        prev.SetPosition(temp);
        OnMovePrev?.Invoke(index);
    }

    /// <summary>
    /// Delete the element at the specified index. Then move the
    /// elements after it up by one position.
    /// </summary>
    /// <param name="index">The index of the item to delete.</param>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _components.Count) return;

        _components.RemoveAt(index);

        for (var i = index; i < _components.Count; i++)
        {
            var component = _components[i];
            component.OffsetPosition(y: -ItemHeight);
        }

        var maxStartIndex = Math.Max(0, _components.Count - MaxVisibleItems);
        if (_firstItemIndex > maxStartIndex) _firstItemIndex = maxStartIndex;
        OnDeleteItem?.Invoke(index);
    }


    /// <inheritdoc/>
    public void Draw(SpriteBatch b)
    {
        Background?.Draw(b);
        var lastItemIndex = Math.Min(_firstItemIndex + MaxVisibleItems, _components.Count);
        for (var i = _firstItemIndex; i < lastItemIndex; i++)
        {
            var component = _components[i];
            if (Bounds.Contains(component.Bounds))
                component.Draw(b);
        }
    }

    /// <summary>
    /// Used to modify the components in the stack, we should never
    /// use this method to edit the position or size of each
    /// component, since it may ruin the layout.
    /// </summary>
    public void EditComponents(Action<IEnumerable<IComponent>> queryFunc) => queryFunc(_components);

    /// <summary>
    /// Used to query the components in the stack, we should never
    /// use this method to edit the position or size of each
    /// component, since it may ruin the layout.
    /// </summary>
    public TResult QueryComponents<TResult>(Func<IEnumerable<IComponent>, TResult> queryFunc) => queryFunc(_components);

    /// <inheritdoc cref="BaseMenu.ReceiveLeftClick"/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y)) return false;

        var lastItemIndex = Math.Min(_firstItemIndex + MaxVisibleItems, _components.Count);
        for (var i = _firstItemIndex; i < lastItemIndex; i++)
        {
            var component = _components[i];
            if (component is not IClickableComponent c) continue;
            var handled = c.ReceiveLeftClick(x, y);
            if (handled) return true;
        }

        return false;
    }

    /// <inheritdoc cref="BaseMenu.ReceiveCursorHover"/>
    public virtual bool ReceiveCursorHover(int x, int y)
    {
        if (!Bounds.Contains(x, y)) return false;

        var lastItemIndex = Math.Min(_firstItemIndex + MaxVisibleItems, _components.Count);
        for (var i = _firstItemIndex; i < lastItemIndex; i++)
        {
            var component = _components[i];
            if (component is not IClickableComponent c) continue;
            var handled = c.ReceiveCursorHover(x, y);
            if (handled) return true;
        }

        return false;
    }

    public virtual bool ReceiveScrollWheelAction(int amount)
    {
        var cursorPos = Game1.getMousePosition();
        if (!Bounds.Contains(cursorPos)) return false;
        if (_components.Count <= MaxVisibleItems) return true;

        // Vertical scrolling only. 1 "unit" of scroll moves by 1 item.
        var newIndex = _firstItemIndex - amount;
        var maxIndex = _components.Count - MaxVisibleItems;

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

        // Update component positions
        foreach (var component in _components)
        {
            var oldX = component.X;
            var oldY = component.Y;
            component.SetPosition(oldX, oldY + amount * ItemHeight);
        }

        return true;
    }

    public void Dispose()
    {
        foreach (var component in _components)
        {
            if (component is IDisposable d)
                d.Dispose();
        }

        _components.Clear();

        GC.SuppressFinalize(this);
    }
}