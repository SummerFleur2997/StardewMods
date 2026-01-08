using ConvenientChests.Framework.UserInterfaceService;
using ConvenientChests.StashToChests.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using UI.Component;

namespace ConvenientChests.Framework.InventoryService;

internal class InventorySideTab : IOverlay<GameMenu>
{
    public GameMenu RootMenu { get; }
    private TextButton LockButton { get; set; }

    public InventorySideTab(GameMenu menu)
    {
        RootMenu = menu;
        AddAndPositionButtons();
    }

    public void Draw(SpriteBatch b)
    {
        if (ModEntry.StashModule.IsActive || ModEntry.CategorizeModule.IsActive)
            LockButton?.Draw(b);
        RootMenu.drawMouse(b);
    }

    private void AddAndPositionButtons()
    {
        var padding = NineSlice.LeftProtrudingTab().TopBorderThickness;

        LockButton = new TextButton(NineSlice.LeftProtrudingTab(), I18n.LockItems_Title(),
            Color.Black, Game1.smallFont, padding: padding);
        LockButton.OnPress += OpenLockMenu;

        var delta = ModEntry.IsAndroid ? 100 + ModEntry.Config.MobileOffset : 106;

        LockButton.SetPosition(
            RootMenu.xPositionOnScreen + RootMenu.width / 2 - LockButton.Width - delta * Game1.pixelZoom,
            RootMenu.yPositionOnScreen + 30 * Game1.pixelZoom);

        LockButton.Label.OffsetPosition(Game1.pixelZoom * 2);
    }

    private void OpenLockMenu()
    {
        var data = Game1.player.GetInventoryData();
        var menu = new LockMenu(RootMenu.xPositionOnScreen, RootMenu.yPositionOnScreen,
            RootMenu.width, RootMenu.height, data, IClickableMenu.borderWidth);
        menu.exitFunction = ExitFunction;
        Game1.activeClickableMenu = menu;
        return;

        void ExitFunction() => Game1.activeClickableMenu = RootMenu;
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        if ((ModEntry.StashModule.IsActive || ModEntry.CategorizeModule.IsActive) && LockButton.Contains(x, y))
            return LockButton.ReceiveLeftClick(x, y);
        return false;
    }

    public bool ReceiveCursorHover(int x, int y) => false;
}