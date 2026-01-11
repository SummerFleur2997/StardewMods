using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley.Objects;

namespace BetterHatsAPI.Framework;

public static class TooltipHelper
{
    public static void RegisterEventsForTooltip(IModHelper helper)
    {
        helper.Events.GameLoop.SaveLoaded += RegisterTooltipDrawingEvent;
        helper.Events.GameLoop.ReturnedToTitle += UnregisterTooltipDrawingEvent;
    }

    private static void RegisterTooltipDrawingEvent(object s, SaveLoadedEventArgs e) =>
        ModEntry.ModHelper.Events.Input.ButtonsChanged += DrawingTooltip;

    private static void UnregisterTooltipDrawingEvent(object s, ReturnedToTitleEventArgs e) =>
        ModEntry.ModHelper.Events.Input.ButtonsChanged -= DrawingTooltip;

    private static void DrawingTooltip(object s, ButtonsChangedEventArgs e)
    {
        if (e.Held.FirstOrDefault() != SButton.LeftControl)
            return;

        // The Current menu is not null
        var menu = Game1.activeClickableMenu;
        if (menu is null)
            return;

        switch (menu)
        {
            case GameMenu { currentTab: 0 } m when m.GetCurrentPage() is InventoryPage { hoveredItem: Hat }:
            case ShopMenu { hoveredItem: Hat }:
            case MenuWithInventory { hoveredItem: Hat }:
                break;
            default:
                return;
        }
    }
}