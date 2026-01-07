using Microsoft.Xna.Framework.Graphics;

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
    /// Draw the overlay on the screen.
    /// </summary>
    public void Draw(SpriteBatch b);
}