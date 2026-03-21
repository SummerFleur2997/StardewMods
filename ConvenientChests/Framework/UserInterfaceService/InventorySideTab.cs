#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using UI.Component;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class InventorySideTab : IOverlay<GameMenu>
{
    public GameMenu RootMenu { get; }
    private Button LockButton { get; }

    public InventorySideTab(GameMenu menu)
    {
        RootMenu = menu;

        var padding = NineSlice.LeftProtrudingTab().TopBorderThickness;

        LockButton = new Button(NineSlice.LeftProtrudingTab(), I18n.LockItems_Title(),
            Color.Black, Game1.smallFont, padding: padding);

        var delta = ModEntry.IsAndroid ? 100 + ModEntry.Config.MobileOffset : 106;

        LockButton.SetPosition(
            RootMenu.xPositionOnScreen + RootMenu.width / 2 - LockButton.Width - delta * Game1.pixelZoom,
            RootMenu.yPositionOnScreen + 30 * Game1.pixelZoom);

        LockButton.Label.OffsetPosition(Game1.pixelZoom * 2);
    }

    public void Draw(SpriteBatch b)
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
}