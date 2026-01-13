#nullable enable
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemLabel<T> : IComponent where T : Item?
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

    public T? Item;

    public ItemLabel(T? item, int x = 0, int y = 0, int width = 64, int height = 64)
    {
        Item = item;
        this.SetDestination(x, y, width, height);
    }

    public ItemLabel(T? item, Rectangle destination)
    {
        Item = item;
        this.SetDestination(destination);
    }

    public ItemLabel(string item, int x = 0, int y = 0, int width = 64, int height = 64)
        : this(ItemRegistry.Create<T>(item), x, y, width, height) { }

    public ItemLabel(string item, Rectangle destination)
        : this(ItemRegistry.Create<T>(item), destination) { }

    public virtual void Draw(SpriteBatch b) =>
        Item?.drawInMenu(b, new Vector2(X, Y), Width / 64f, 1, 1, StackDrawType.Hide);
}