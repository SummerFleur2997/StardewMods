using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace UI.UserInterface;

/// <summary>
/// A positioned, resizable element in the interface
/// that can also contain other elements.
/// </summary>
internal class Widget : IDisposable
{
    /****
     ** 字段与属性
     ** Fields & Properties
     ****/

    # region Fields & Properties

    private Widget _parent;
    private readonly List<Widget> _children = new();
    private int _width;
    private int _height;
    private Point Size => new(Width, Height);
    public Rectangle LocalBounds => new(Point.Zero, Size);
    protected Rectangle GlobalBounds => new(GlobalPosition, Size);
    protected Point GlobalPosition => Globalize(Point.Zero);

    private Widget Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            OnParent(value);
        }
    }

    public IEnumerable<Widget> Children => _children.AsReadOnly();

    public Point Position { get; set; }

    public int X
    {
        get => Position.X;
        set => Position = new Point(value, Position.Y);
    }

    public int Y
    {
        get => Position.Y;
        set => Position = new Point(Position.X, value);
    }


    public int Width
    {
        get => _width;
        set
        {
            _width = value;
            OnDimensionsChanged();
        }
    }

    public int Height
    {
        get => _height;
        set
        {
            _height = value;
            OnDimensionsChanged();
        }
    }

    #endregion

    public Widget()
    {
        Position = Point.Zero;
        Width = 1;
        Height = 1;
    }

    protected virtual void OnParent(Widget parent) { }
    protected virtual void OnContentsChanged() { }
    protected virtual void OnDimensionsChanged() { }

    public virtual void Draw(SpriteBatch batch)
    {
        DrawChildren(batch);
    }

    private void DrawChildren(SpriteBatch batch)
    {
        foreach (var child in Children)
            child.Draw(batch);
    }

    protected Point Globalize(Point point)
    {
        var global = new Point(point.X + Position.X, point.Y + Position.Y);
        return Parent?.Globalize(global) ?? global;
    }

    public virtual bool ReceiveButtonPress(SButton input)
    {
        return PropagateButtonPress(input);
    }

    protected bool PropagateButtonPress(SButton input)
    {
        foreach (var child in Children)
        {
            var handled = child.ReceiveButtonPress(input);
            if (handled) return true;
        }

        return false;
    }

    public virtual bool ReceiveCursorHover(Point point)
    {
        return PropagateCursorHover(point);
    }

    protected bool PropagateCursorHover(Point point)
    {
        foreach (var child in Children)
        {
            var localPoint = new Point(point.X - child.Position.X, point.Y - child.Position.Y);

            if (child.LocalBounds.Contains(localPoint))
            {
                var handled = child.ReceiveCursorHover(localPoint);
                if (handled) return true;
            }
        }

        return false;
    }

    public virtual bool ReceiveLeftClick(Point point)
    {
        return PropagateLeftClick(point);
    }

    protected bool PropagateLeftClick(Point point)
    {
        foreach (var child in Children)
        {
            var localPoint = new Point(point.X - child.Position.X, point.Y - child.Position.Y);

            if (child.LocalBounds.Contains(localPoint))
            {
                var handled = child.ReceiveLeftClick(localPoint);
                if (handled) return true;
            }
        }

        return false;
    }

    public virtual bool ReceiveScrollWheelAction(int amount)
    {
        return PropagateScrollWheelAction(amount);
    }

    protected bool PropagateScrollWheelAction(int amount)
    {
        foreach (var child in Children)
        {
            var handled = child.ReceiveScrollWheelAction(amount);
            if (handled) return true;
        }

        return false;
    }

    public T AddChild<T>(T child) where T : Widget
    {
        child.Parent = this;
        _children.Add(child);

        OnContentsChanged();

        return child;
    }

    public void RemoveChild(Widget child)
    {
        _children.Remove(child);
        child.Parent = null;

        OnContentsChanged();
    }

    public void RemoveChildren()
    {
        foreach (var child in Children)
            child.Parent = null;

        _children.Clear();

        OnContentsChanged();
    }

    public void CenterHorizontally()
    {
        var containerWidth = Parent?.Width ?? Game1.uiViewport.Width;
        X = containerWidth / 2 - Width / 2;
    }

    public void CenterVertically()
    {
        var containerHeight = Parent?.Height ?? Game1.uiViewport.Height;
        Y = containerHeight / 2 - Height / 2;
    }

    public virtual void Dispose()
    {
        foreach (var child in Children)
            child.Dispose();
    }
}