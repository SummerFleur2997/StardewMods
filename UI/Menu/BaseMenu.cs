using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;

namespace UI.Menu;

/// <summary>
/// Provide a bridge between <see cref="IClickableMenu"/> and the
/// game's <see cref="StardewValley.Menus.IClickableMenu"/>.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class BaseMenu : StardewValley.Menus.IClickableMenu, IClickableMenu
{
    public Rectangle Bounds => new(xPositionOnScreen, yPositionOnScreen, width, height);

    /// <summary>
    /// The background of the menu.
    /// </summary>
    public IComponent Background;

    /// <summary>
    /// The children components of the menu. Avoid adding or removing components
    /// directly. Use <see cref="AddChild"/> and <see cref="RemoveChild"/> instead.
    /// </summary>
    public List<IComponent> Components = new();

    public BaseMenu(int x, int y, int width, int height)
        : base(x, y, width, height, true) =>
        Background = NineSlice.MenuBackground(new Rectangle(x, y, width, height));

    /// <summary>
    /// Add a component to the <see cref="Components"/>.
    /// </summary>
    public virtual void AddChild(IComponent child) => Components.Add(child);

    /// <summary>
    /// Add multiple components to the <see cref="Components"/>.
    /// </summary>
    public virtual void AddChildren(IEnumerable<IComponent> children)
    {
        foreach (var child in children)
            Components.Add(child);
    }

    /// <summary>
    /// Remove the given child from the <see cref="Components"/>.
    /// </summary>
    public virtual void RemoveChild(IComponent child)
    {
        Components.Remove(child);
        if (child is IDisposable d)
            d.Dispose();
    }

    /// <summary>
    /// Remove all components from the <see cref="Components"/>.
    /// </summary>
    public virtual void RemoveChildren()
    {
        foreach (var component in Components)
            if (component is IDisposable d)
                d.Dispose();

        Components.Clear();
    }

    public sealed override void draw(SpriteBatch b)
    {
        Background?.Draw(b);
        foreach (var child in Components)
            child.Draw(b);
        upperRightCloseButton.draw(b);
        Draw(b);
        drawMouse(b);
    }

    public virtual void Draw(SpriteBatch b) { }

    /// <inheritdoc/>
    public sealed override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        ReceiveLeftClick(x, y);
    }

    /// <summary>
    /// Handle the left click event, traverse all components from
    /// back to front to find the clickable component.
    /// </summary>
    /// <inheritdoc/>
    /// <seealso cref="receiveLeftClick"/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        // Travel from back to front
        for (var i = Components.Count - 1; i >= 0; i--)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            if (c.ReceiveLeftClick(x, y))
                return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public sealed override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);
        ReceiveCursorHover(x, y);
    }

    /// <summary>
    /// Handle the cursor hover event, traverse all components from
    /// back to front to find the clickable component.
    /// </summary>
    /// <inheritdoc/>
    /// <seealso cref="performHoverAction"/>
    public virtual bool ReceiveCursorHover(int x, int y)
    {
        // Travel from back to front
        for (var i = Components.Count - 1; i >= 0; i--)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            if (c.ReceiveCursorHover(x, y))
                return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public sealed override void receiveScrollWheelAction(int direction)
    {
        base.receiveScrollWheelAction(direction);
        var delta = direction switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
        ReceiveScrollWheelAction(delta);
    }

    /// <summary>
    /// Handle the scroll-wheel action, traverse all components from
    /// back to front to find the scrollable menu component.
    /// </summary>
    /// <param name="amount">The scroll amount, which should be set
    /// to -1 or 1 in the parent manager.</param>
    /// <inheritdoc/>
    /// <seealso cref="receiveScrollWheelAction"/>
    public virtual bool ReceiveScrollWheelAction(int amount)
    {
        // Travel from back to front
        for (var i = Components.Count - 1; i >= 0; i--)
        {
            var component = Components[i];
            if (component is not IClickableMenu m)
                continue;

            if (m.ReceiveScrollWheelAction(amount))
                return true;
        }

        return false;
    }

    public virtual void Dispose()
    {
        foreach (var component in Components)
            if (component is IDisposable d)
                d.Dispose();

        Components.Clear();
        GC.SuppressFinalize(this);
    }
}