using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
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
        ModEntry.ModHelper.Events.Display.RenderedActiveMenu += DrawingTooltip;

    private static void UnregisterTooltipDrawingEvent(object s, ReturnedToTitleEventArgs e) =>
        ModEntry.ModHelper.Events.Display.RenderedActiveMenu -= DrawingTooltip;

    private static void DrawingTooltip(object s, RenderedActiveMenuEventArgs e)
    {
        if (Game1.activeClickableMenu is not MenuWithInventory { hoveredItem: Hat hat }) return;
        var allHatdata = hat.GetHatData();
        // todo: wip
    }
}