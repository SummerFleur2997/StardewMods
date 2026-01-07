using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemToggle : IClickableComponent, IDisposable
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

    public Item Item;
    public Tooltip Tooltip;
    public bool Active;
    public event Action OnToggle;
    public event Action OnHover;

    public ItemToggle(Item item, bool active, int x = 0, int y = 0, int width = 64, int height = 64)
    {
        Item = item;
        Tooltip = new Tooltip(item);
        Active = active;
        this.SetDestination(x, y, width, height);
    }

    public ItemToggle(Item item, bool active, Rectangle destination)
    {
        Item = item;
        Tooltip = new Tooltip(item);
        Active = active;
        this.SetDestination(destination);
    }

    public ItemToggle(string item, bool active, int x = 0, int y = 0, int width = 64, int height = 64)
        : this(ItemRegistry.Create(item), active, x, y, width, height) { }

    public ItemToggle(string item, bool active, Rectangle destination)
        : this(ItemRegistry.Create(item), active, destination) { }

    public virtual void Draw(SpriteBatch b)
    {
        var alpha = Active ? 1.0f : 0.33f;
        Item.drawInMenu(b, new Vector2(X, Y), Width / 64f, alpha, 1, StackDrawType.Hide);
    }

    /// <summary>
    /// Handle the left click event, assuming the click is within
    /// the bounds of the component. So this should be checked before
    /// calling this method.
    /// </summary>
    /// <inheritdoc/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        Active = !Active;
        OnToggle?.Invoke();
        Game1.playSound("dwoop");
        return true;
    }

    /// <summary>
    /// Handle the mouse hover event, assuming the mouse is within
    /// the bounds of the component. So this should be checked before
    /// calling this method.
    /// </summary>
    /// <inheritdoc/>
    public virtual bool ReceiveCursorHover(int x, int y)
    {
        OnHover?.Invoke();
        return true;
    }

    public virtual void Dispose()
    {
        OnToggle = null;
        OnHover = null;
        GC.SuppressFinalize(this);
    }
}