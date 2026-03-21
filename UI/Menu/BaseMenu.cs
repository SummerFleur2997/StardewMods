#nullable enable
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using ClickableMenu = StardewValley.Menus.IClickableMenu;

namespace UI.Menu;

/// <summary>
/// Provide a bridge between <see cref="IClickableMenu"/> and the
/// game's <see cref="ClickableMenu"/>.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class BaseMenu : ClickableMenu, IClickableMenu
{
    public Rectangle Bounds => new(xPositionOnScreen, yPositionOnScreen, width, height);

    /// <summary>
    /// The parent menu of this menu, if any.
    /// </summary>
    public ClickableMenu? ParentMenu;

    /// <summary>
    /// The child menu of this menu, if any.
    /// </summary>
    public BaseMenu? ChildMenu;

    /// <summary>
    /// The background of the menu.
    /// </summary>
    public IComponent? Background;

    public event Action<IComponent>? OnComponentAdd;
    public event Action<IComponent>? OnComponentRemove;

    /// <summary>
    /// The children components of the menu. Avoid adding or removing components
    /// directly. Use <see cref="AddChild"/> and <see cref="RemoveChild"/> instead.
    /// </summary>
    public List<IComponent> Components = new();

    public BaseMenu(int x, int y, int width, int height) : base(x, y, width, height, true)
    {
        Background = NineSlice.MenuBackground(new Rectangle(x, y, width, height));
        exitFunction = OnExit;
    }

    /// <summary>
    /// Add a component to the <see cref="Components"/>.
    /// </summary>
    public void AddChild(IComponent child)
    {
        Components.Add(child);
        OnComponentAdd?.Invoke(child);
    }

    /// <summary>
    /// Add multiple components to the <see cref="Components"/>.
    /// </summary>
    public void AddChildren(IEnumerable<IComponent> children)
    {
        foreach (var child in children)
        {
            Components.Add(child);
            OnComponentAdd?.Invoke(child);
        }
    }

    /// <summary>
    /// Add multiple components to the <see cref="Components"/>.
    /// </summary>
    public void AddChildren(params IComponent[] children)
    {
        foreach (var child in children)
        {
            Components.Add(child);
            OnComponentAdd?.Invoke(child);
        }
    }

    /// <summary>
    /// Remove the given child from the <see cref="Components"/>.
    /// </summary>
    public void RemoveChild(IComponent child)
    {
        Components.Remove(child);
        OnComponentRemove?.Invoke(child);
        if (child is IDisposable d)
            d.Dispose();
    }

    /// <summary>
    /// Remove all components from the <see cref="Components"/>.
    /// </summary>
    public void RemoveChildren()
    {
        foreach (var component in Components)
        {
            OnComponentRemove?.Invoke(component);
            if (component is IDisposable d)
                d.Dispose();
        }

        Components.Clear();
    }

    /// <inheritdoc />
    public sealed override void draw(SpriteBatch b)
    {
        if (!Game1.options.showClearBackgrounds)
            b.Draw(Game1.fadeToBlackRect,
                new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
        Background?.Draw(b);
        OnDraw(b);
        foreach (var child in Components)
            child.Draw(b);
        upperRightCloseButton.draw(b);
        AfterDraw(b);
        drawMouse(b);
    }

    /// <summary>
    /// Draw objects after the background was drawn.
    /// (Above the backgrounds, but below all the components)
    /// They may be covered by some components.
    /// </summary>
    public virtual void OnDraw(SpriteBatch b) { }

    /// <summary>
    /// Draw objects after the whole menu was drawn.
    /// (Above all the components, include the close button)
    /// This is a good time to draw tooltips, submenus, etc.
    /// </summary>
    public virtual void AfterDraw(SpriteBatch b) { }

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

    /// <summary>
    /// The default exit process of this menu.
    /// </summary>
    public virtual void OnExit()
    {
        var parentMenu = ParentMenu;
        ParentMenu = null;
        if (parentMenu is null)
            return;

        Game1.activeClickableMenu = parentMenu;

        if (parentMenu is BaseMenu bm && bm.ChildMenu == this)
        {
            bm.ChildMenu = null;
            bm.RemoveDependency();
        }
    }

    /// <summary>
    /// The basic logic of dispose. This method will be called by the
    /// game when setting a new <see cref="Game1.activeClickableMenu"/>.
    /// To avoid itself being disposed when using child-menus, we should call
    /// <see cref="StardewValley.Menus.IClickableMenu.AddDependency"/>.
    /// </summary>
    public virtual void Dispose()
    {
        foreach (var component in Components)
            if (component is IDisposable d)
                d.Dispose();

        Components.Clear();
        GC.SuppressFinalize(this);
    }
}