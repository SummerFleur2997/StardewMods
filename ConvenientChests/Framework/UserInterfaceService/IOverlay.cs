using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ConvenientChests.Framework.UserInterfaceService;

public interface IOverlay<out T>
{
    public T RootMenu { get; }

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

    /// <summary>
    /// Handle the scroll-wheel action.
    /// </summary>
    /// <param name="amount">The scroll amount.</param>
    /// <returns>True if the event is handled, false otherwise.</returns>
    public bool ReceiveScrollWheelAction(int amount) => false;

    /// <summary>
    /// Handle the key press event.
    /// </summary>
    /// <param name="key">The pressed key</param>
    public bool ReceiveKeyPress(Keys key) => false;

    /// <summary>
    /// Draw the overlay on the screen.
    /// </summary>
    public void DrawUi(SpriteBatch b);
}