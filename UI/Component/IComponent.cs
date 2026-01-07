using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

/// <summary>
/// The basic component of the UI. 
/// </summary>
public interface IComponent
{
    /// <summary>
    /// The boundary rectangle holding this component.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    /// The left position of the component in pixels.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// The top position of the component in pixels.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// The width of the current component in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the current component in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Draw the component on the screen.
    /// </summary>
    public void Draw(SpriteBatch b);
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public static class ComponentExtensions
{
    /// <summary>
    /// The pixel position of the top-left corner of the <see cref="IComponent"/>.
    /// </summary>
    public static Point Position(this IComponent component) => new(component.X, component.Y);

    /// <summary>
    /// The size of the component in pixels.
    /// </summary>
    public static Point Size(this IComponent component) => new(component.Width, component.Height);

    /// <summary>
    /// Set the position of the component.
    /// </summary>
    /// <param name="component">The target component.</param>
    /// <param name="x">The new x coordinate of the top-left corner of the <see cref="IComponent"/>.</param>
    /// <param name="y">The new y coordinate of the top-left corner of the <see cref="IComponent"/>.</param>
    public static void SetPosition(this IComponent component, int x, int y)
    {
        component.X = x;
        component.Y = y;
    }

    /// <summary>
    /// Set the position of the component.
    /// </summary>
    /// <param name="component">The target component.</param>
    /// <param name="point">The top-left corner of the destination point.</param>
    public static void SetPosition(this IComponent component, Point point)
    {
        component.X = point.X;
        component.Y = point.Y;
    }

    /// <summary>
    /// Set the size of the component.
    /// </summary>
    /// <param name="component">The target component.</param>
    /// <param name="width">The new width of the <see cref="IComponent"/>.</param>
    /// <param name="height">The new height of the <see cref="IComponent"/>.</param>
    public static void SetSize(this IComponent component, int width, int height)
    {
        component.Width = width;
        component.Height = height;
    }

    /// <summary>
    /// Set the position and size of the component.
    /// </summary>
    /// <param name="component">The target component.</param>
    /// <param name="x">The new x coordinate of the top-left corner of the <see cref="IComponent"/>.</param>
    /// <param name="y">The new y coordinate of the top-left corner of the <see cref="IComponent"/>.</param>
    /// <param name="width">The new width of the <see cref="IComponent"/>.</param>
    /// <param name="height">The new height of the <see cref="IComponent"/>.</param>
    public static void SetDestination(this IComponent component, int x, int y, int width, int height)
    {
        component.SetPosition(x, y);
        component.SetSize(width, height);
    }

    /// <summary>
    /// Set the position and size of the component.
    /// </summary>
    /// <param name="component">The target component.</param>
    /// <param name="rectangle">The rectangle containing the position and size information.</param>
    public static void SetDestination(this IComponent component, Rectangle rectangle)
    {
        component.SetPosition(rectangle.Location);
        component.SetSize(rectangle.Width, rectangle.Height);
    }

    /// <summary>
    /// Center the component within the specified bounds.
    /// </summary>
    /// <param name="component">The target component.</param>
    /// <param name="bounds">The bounds within which to center the component.</param>
    public static void SetInCenterOfTheBounds(this IComponent component, Rectangle bounds)
    {
        component.X = bounds.X + (bounds.Width - component.Width) / 2;
        component.Y = bounds.Y + (bounds.Height - component.Height) / 2;
    }

    /// <summary>
    /// Align the component to the left center of the bounds with equal margins.
    /// </summary>
    /// <param name="component">The target component.</param>
    /// <param name="bounds">The bounds within which to align the component.</param>
    public static void SetAtLeftCenterWithEqualMargins(this IComponent component, Rectangle bounds)
    {
        component.Y = bounds.Y + (bounds.Height - component.Height) / 2;
        component.X = component.Y - bounds.Y + bounds.X;
    }
}