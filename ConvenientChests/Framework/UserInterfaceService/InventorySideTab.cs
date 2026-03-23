using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using UI.Component;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class InventorySideTab : IOverlay<GameMenu>
{
    public GameMenu RootMenu { get; }
    public Tooltip? Tooltip { get; private set; }

    private SpriteButton LockButton { get; }

    public InventorySideTab(GameMenu menu)
    {
        RootMenu = menu;
        LockButton = UIHelper.SideButton(0, 0, SideButtonVariant.Lock);

        // var delta = ModEntry.IsAndroid ? 100 + ModEntry.Config.MobileOffset : 106;
        LockButton.SetPosition(
            RootMenu.xPositionOnScreen + RootMenu.width / 2 - LockButton.Width - 106 * Game1.pixelZoom,
            RootMenu.yPositionOnScreen + 30 * Game1.pixelZoom);
    }

    public void DrawAboveUi(SpriteBatch b)
    {
        if (ModEntry.Config.HideSideTab)
            return;

        if (ModEntry.StashModule.IsActive || ModEntry.CategorizeModule.IsActive)
            LockButton.Draw(b);

        RootMenu.drawMouse(b);
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        if ((ModEntry.StashModule.IsActive || ModEntry.CategorizeModule.IsActive) && LockButton.Contains(x, y))
            return LockButton.ReceiveLeftClick(x, y);
        return false;
    }

    public bool ReceiveCursorHover(int x, int y)
    {
        if ((ModEntry.StashModule.IsActive || ModEntry.CategorizeModule.IsActive) && LockButton.Contains(x, y))
        {
            Tooltip = LockButton.Tooltip;
            return true;
        }

        return false;
    }
}