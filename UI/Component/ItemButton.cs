using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemButton<T> : ItemLabel<T>, IClickableComponent, IDisposable where T : Item
{
    public Tooltip Tooltip;
    public event Action OnPress;
    public event Action OnHover;

    public ItemButton(T item, int x = 0, int y = 0, int width = 64, int height = 64)
        : base(item, x, y, width, height) =>
        Tooltip = new Tooltip(item);

    public ItemButton(T item, Rectangle destination)
        : base(item, destination) =>
        Tooltip = new Tooltip(item);

    public ItemButton(string item, int x = 0, int y = 0, int width = 64, int height = 64)
        : this(ItemRegistry.Create<T>(item), x, y, width, height) { }

    public ItemButton(string item, Rectangle destination)
        : this(ItemRegistry.Create<T>(item), destination) { }

    /// <summary>
    /// Handle the left click event, assuming the click is within
    /// the bounds of the component. So this should be checked before
    /// calling this method.
    /// </summary>
    /// <inheritdoc/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        OnPress?.Invoke();
        Game1.playSound("drumkit6");
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
        OnPress = null;
        OnHover = null;
        GC.SuppressFinalize(this);
    }
}