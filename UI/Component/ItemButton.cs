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

    /// <inheritdoc/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnPress?.Invoke();
        Game1.playSound("drumkit6");
        return true;
    }

    /// <inheritdoc/>
    public virtual bool ReceiveCursorHover(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

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