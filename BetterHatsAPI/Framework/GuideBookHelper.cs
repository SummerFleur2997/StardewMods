using BetterHatsAPI.GuideBook;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley.Objects;

namespace BetterHatsAPI.Framework;

public static class GuideBookHelper
{
    public static void RegisterEventsForGuideBook(IModHelper helper)
    {
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
    }

    private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        => ModEntry.ModHelper.Events.Input.ButtonsChanged += OnButtonChanged;

    private static void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        => ModEntry.ModHelper.Events.Input.ButtonsChanged -= OnButtonChanged;

    private static void OnButtonChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (!ModEntry.Config.OpenGuideBookKey.JustPressed())
            return;

        Hat hat;
        var m = Game1.activeClickableMenu;
        var menu = m is GameMenu gameMenu ? gameMenu.GetCurrentPage() : m;
        switch (menu)
        {
            case MenuWithInventory inventoryMenu:
                hat = (Game1.player.CursorSlotItem ?? inventoryMenu.heldItem ?? inventoryMenu.hoveredItem) as Hat;
                break;
            case InventoryPage inventory:
                hat = (Game1.player.CursorSlotItem ?? inventory.hoveredItem) as Hat;
                break;
            case ShopMenu shopMenu:
                var entry = shopMenu.hoveredItem;
                hat = entry as Hat;
                break;
            default:
                hat = null;
                break;
        }

        var x = (Game1.uiViewport.Width - GuideMenu.MenuWidth) / 2;
        var y = (Game1.uiViewport.Height - GuideMenu.MenuHeight) / 2;
        var guideBook = new GuideMenu(x, y, hat ?? Game1.player.hat?.Value ?? ItemRegistry.Create<Hat>("(H)70"));
        if (m is not null) guideBook.exitFunction = () => Game1.activeClickableMenu = m;
        Game1.activeClickableMenu = guideBook;
    }
}