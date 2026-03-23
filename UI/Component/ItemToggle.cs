using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemToggle<T> : ItemLabel<T>, IClickableComponent, IHaveTooltip, IDisposable where T : Item
{
    public Tooltip? Tooltip { get; }

    public string SoundCue = "dwoop";
    public bool Active;
    public event Action? OnToggle;
    public event Action? OnHover;

    /// <summary>
    /// Construct an item toggle with a given item.
    /// </summary>
    /// <param name="item">The given item.</param>
    /// <param name="active">The initial status of this toggle.</param>
    /// <param name="x">The x-position of item.</param>
    /// <param name="y">The y-position of item.</param>
    /// <param name="width">The width of the item, 64 by default.</param>
    /// <param name="height">The height of the item, 64 by default.</param>
    /// <param name="setTooltip">Whether to create a tooltip of the item.</param>
    public ItemToggle(T? item, bool active, int x = 0, int y = 0, int width = 64, int height = 64,
        bool setTooltip = true) : base(item, x, y, width, height)
    {
        if (item is null || !setTooltip)
            return;

        Tooltip = new Tooltip(item);
        Active = active;
    }

    /// <summary>
    /// Construct an item toggle with a qualified item id.
    /// </summary>
    /// <param name="item">The qualified item id.</param>
    /// <param name="active">The initial status of this toggle.</param>
    /// <param name="x">The x-position of item.</param>
    /// <param name="y">The y-position of item.</param>
    /// <param name="width">The width of the item, 64 by default.</param>
    /// <param name="height">The height of the item, 64 by default.</param>
    /// <param name="setTooltip">Whether to create a tooltip of the item.</param>
    public ItemToggle(string item, bool active, int x = 0, int y = 0, int width = 64, int height = 64,
        bool setTooltip = true) : this(ItemRegistry.Create<T>(item), active, x, y, width, height) { }

    public override void Draw(SpriteBatch b)
    {
        var alpha = Active ? 1.0f : 0.33f;
        Item?.drawInMenu(b, new Vector2(X, Y), Width / 64f, alpha, 1, StackDrawType.Hide);
    }

    /// <inheritdoc/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        if (!Bounds.Contains(x, y))
            return false;

        Active = !Active;
        OnToggle?.Invoke();
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
        OnToggle = null;
        OnHover = null;
        GC.SuppressFinalize(this);
    }
}