using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemToggle<T> : ItemLabel<T>, IClickableComponent, IDisposable where T : Item
{
    public Tooltip Tooltip;
    public bool Active;
    public event Action OnToggle;
    public event Action OnHover;

    public ItemToggle(T item, bool active, int x = 0, int y = 0, int width = 64, int height = 64)
        : base(item, x, y, width, height)
    {
        Tooltip = new Tooltip(item);
        Active = active;
    }

    public ItemToggle(T item, bool active, Rectangle destination)
        : base(item, destination)
    {
        Tooltip = new Tooltip(item);
        Active = active;
    }

    public ItemToggle(string item, bool active, int x = 0, int y = 0, int width = 64, int height = 64)
        : this(ItemRegistry.Create<T>(item), active, x, y, width, height) { }

    public ItemToggle(string item, bool active, Rectangle destination)
        : this(ItemRegistry.Create<T>(item), active, destination) { }

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
        Game1.playSound("dwoop");
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
        OnToggle = null;
        OnHover = null;
        GC.SuppressFinalize(this);
    }
}