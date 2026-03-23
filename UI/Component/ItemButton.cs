using JetBrains.Annotations;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemButton<T> : ItemLabel<T>, IClickableComponent, IHaveTooltip, IDisposable where T : Item
{
    public Tooltip? Tooltip { get; }

    public string SoundCue = "drumkit6";
    public event Action? OnPress;
    public event Action? OnHover;

    /// <summary>
    /// Construct an item button with a given item.
    /// </summary>
    /// <param name="item">The given item.</param>
    /// <param name="x">The x-position of item.</param>
    /// <param name="y">The y-position of item.</param>
    /// <param name="width">The width of the item, 64 by default.</param>
    /// <param name="height">The height of the item, 64 by default.</param>
    /// <param name="setTooltip">Whether to create a tooltip of the item.</param>
    public ItemButton(T? item, int x = 0, int y = 0, int width = 64, int height = 64, bool setTooltip = true)
        : base(item, x, y, width, height)
    {
        if (item is null || !setTooltip)
            return;

        Tooltip = new Tooltip(item);
    }

    /// <summary>
    /// Construct an item button with a qualified item id.
    /// </summary>
    /// <param name="item">The qualified item id.</param>
    /// <param name="x">The x-position of item.</param>
    /// <param name="y">The y-position of item.</param>
    /// <param name="width">The width of the item, 64 by default.</param>
    /// <param name="height">The height of the item, 64 by default.</param>
    /// <param name="setTooltip">Whether to create a tooltip of the item.</param>
    public ItemButton(string item, int x = 0, int y = 0, int width = 64, int height = 64, bool setTooltip = true)
        : this(ItemRegistry.Create<T>(item), x, y, width, height) { }

    /// <inheritdoc/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        OnPress?.Invoke();
        if (!string.IsNullOrEmpty(SoundCue)) Game1.playSound(SoundCue);
        return true;
    }

    /// <inheritdoc/>
    public virtual bool ReceiveCursorHover(int x, int y)
    {
        if (!Bounds.Contains(x, y))
        {
            Scale = Math.Max(Scale - 0.04f, 1f);
            return false;
        }

        Scale = Math.Min(Scale + 0.04f, 1.125f);
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