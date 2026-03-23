using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemLabel<T> : IComponent, IHeldItem<T> where T : Item?
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

    /// <summary>
    /// The item of this label.
    /// </summary>
    public T? Item { get; set; }

    public float Scale = 1;

    /// <summary>
    /// Construct an item label with a given item.
    /// </summary>
    /// <param name="item">The given item.</param>
    /// <param name="x">The x-position of item.</param>
    /// <param name="y">The y-position of item.</param>
    /// <param name="width">The width of the item, 64 by default.</param>
    /// <param name="height">The height of the item, 64 by default.</param>
    public ItemLabel(T? item, int x = 0, int y = 0, int width = 64, int height = 64)
    {
        Item = item;
        this.SetDestination(x, y, width, height);
    }

    /// <summary>
    /// Construct an item label with a qualified item id.
    /// </summary>
    /// <param name="item">The qualified item id.</param>
    /// <param name="x">The x-position of item.</param>
    /// <param name="y">The y-position of item.</param>
    /// <param name="width">The width of the item, 64 by default.</param>
    /// <param name="height">The height of the item, 64 by default.</param>
    public ItemLabel(string item, int x = 0, int y = 0, int width = 64, int height = 64)
        : this(ItemRegistry.Create<T>(item), x, y, width, height) { }

    /// <inheritdoc />
    public virtual void Draw(SpriteBatch b) =>
        Item?.drawInMenu(b, new Vector2(X, Y), Width / 64f * Scale, 1, 1, StackDrawType.Hide);
}

public interface IHeldItem<out T> where T : Item?
{
    T? Item { get; }
}