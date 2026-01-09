using UI.Component;

namespace UI.Menu;

public interface IClickableMenu : IDisposable
{
    /// <inheritdoc cref="IClickableComponent.ReceiveCursorHover"/>
    public bool ReceiveCursorHover(int x, int y);

    /// <inheritdoc cref="IClickableComponent.ReceiveLeftClick"/>
    public bool ReceiveLeftClick(int x, int y);

    /// <summary>
    /// Handle the scroll-wheel action.
    /// </summary>
    /// <param name="amount">The scroll amount.</param>
    /// <returns>True if the event is handled, false otherwise.</returns>
    public bool ReceiveScrollWheelAction(int amount);
}