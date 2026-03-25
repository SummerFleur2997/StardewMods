using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class InventoryOverlay : IOverlay<GameMenu>
{
    public GameMenu RootMenu { get; }

    private readonly LockItemMenu _lockItemMenu;

    public InventoryOverlay(GameMenu menu)
    {
        RootMenu = menu;

        _lockItemMenu = new LockItemMenu(menu.xPositionOnScreen - 80, menu.yPositionOnScreen + 128);
    }

    /// <inheritdoc />
    public void DrawUi(SpriteBatch b)
    {
        if (ModEntry.Config.HideSideTab)
            return;

        if (RootMenu.GetCurrentPage() is not InventoryPage inventoryPage)
            return;

        _lockItemMenu.Draw(b, inventoryPage.inventory);
        RootMenu.drawMouse(b);
    }

    /// <inheritdoc />
    public bool ReceiveLeftClick(int x, int y)
    {
        if (RootMenu.GetCurrentPage() is not InventoryPage inventoryPage)
            return false;

        return _lockItemMenu.ReceiveLeftClick(x, y, inventoryPage.inventory);
    }

    /// <inheritdoc />
    public bool ReceiveCursorHover(int x, int y) => _lockItemMenu.ReceiveCursorHover(x, y);

    /// <inheritdoc />
    public bool ReceiveKeyPress(Keys key) => _lockItemMenu.ReceiveKeyPress(key);
}