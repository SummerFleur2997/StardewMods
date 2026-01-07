using JetBrains.Annotations;

namespace UI.Component;

/// <summary>
/// Component that can be clicked or received cursor hover events.
/// </summary>
public interface IClickableComponent : IComponent
{
    /// <summary>
    /// Handle the left click event.
    /// </summary>
    /// <param name="x">The x position of the cursor in pixel.</param>
    /// <param name="y">The y position of the cursor in pixel.</param>
    /// <returns>True if the event is handled, false otherwise.</returns>
    public bool ReceiveLeftClick(int x, int y);

    /// <summary>
    /// Handle the cursor hover event.
    /// </summary>
    /// <param name="x">The x position of the cursor in pixel.</param>
    /// <param name="y">The y position of the cursor in pixel.</param>
    /// <returns>True if the event is handled, false otherwise.</returns>
    public bool ReceiveCursorHover(int x, int y);
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public static class ClickableComponentExtensions
{
    public static bool Contains(this IClickableComponent component, int x, int y)
        => component.Bounds.Contains(x, y);
}