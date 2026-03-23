using Microsoft.Xna.Framework.Input;
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

    /// <summary>
    /// Handle a keyboard button pressed while the menu is open.
    /// </summary>
    /// <param name="key">The key of which was pressed.</param>
    /// <returns>True if the event is handled, false otherwise.</returns>
    public bool ReceiveKeyPress(Keys key) => false;
}