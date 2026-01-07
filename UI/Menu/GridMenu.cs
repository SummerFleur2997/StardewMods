using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;

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
    public int MaxItemColumns;

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
    public int MaxItemRows;

    /// <summary>
    /// The size of each item displayed in the grid.
    /// </summary>
    public int ItemSize;

    /// <summary>
    /// The index of the first item to display. Used when drawing or
    /// scrolling the grid.
    /// </summary>
    private int _firstItemIndex;

    public int MaxVisibleItems => MaxItemColumns * MaxItemRows;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="itemSize"></param>
    /// <param name="maxItemColumns"></param>
    /// <param name="maxItemRows"></param>
    public GridMenu(int x, int y, int width, int height, int itemSize,
        int maxItemColumns = 99, int maxItemRows = 99)
    {
        this.SetDestination(x, y, width, height);
        MaxItemColumns = maxItemColumns;
        MaxItemRows = maxItemRows;
        ItemSize = itemSize;

        if (itemSize * maxItemColumns > width)
            MaxItemColumns = width / itemSize;
        if (itemSize * maxItemRows > height)
            MaxItemRows = height / itemSize;
    }

    /// <summary>
    /// Add a component to the grid, then automatically calculates
    /// its initial position. The component will be placed in the
    /// center of its bounds by default.
    /// </summary>
    public void AddComponent(IComponent component)
    {
        var index = Components.Count;

        Components.Add(component);

        var column = index % MaxItemColumns;
        var row = index / MaxItemColumns;
        var bound = new Rectangle(X + column * ItemSize, Y + row * ItemSize, ItemSize, ItemSize);
        component.SetInCenterOfTheBounds(bound);
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
    }

    /// <inheritdoc cref="BaseMenu.ReceiveLeftClick"/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        for (var i = Components.Count - 1; i >= 0; i--)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            if (!component.Bounds.Contains(x, y))
                continue;

            var handled = c.ReceiveLeftClick(x, y);
            if (handled) return true;
        }

        return false;
    }

    /// <inheritdoc cref="BaseMenu.ReceiveCursorHover"/>
    public virtual bool ReceiveCursorHover(int x, int y)
    {
        for (var i = Components.Count - 1; i >= 0; i--)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            if (!component.Bounds.Contains(x, y))
                continue;

            var handled = c.ReceiveCursorHover(x, y);
            if (handled) return true;
        }

        return false;
    }

    public virtual bool ReceiveScrollWheelAction(int amount)
    {
        if (Components.Count <= MaxVisibleItems)
            return true;

        var newIndex = _firstItemIndex - amount * MaxItemColumns;
        var maxIndex = ((Components.Count - 1) / MaxItemColumns - MaxItemRows + 1) * MaxItemColumns;

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

    public void Dispose()
    {
        foreach (var component in Components)
            if (component is IDisposable d)
                d.Dispose();

        Components.Clear();
        GC.SuppressFinalize(this);
    }
}