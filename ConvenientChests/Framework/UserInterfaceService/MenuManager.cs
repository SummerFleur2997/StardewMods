using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley.Objects;
using UI.UserInterface;

namespace ConvenientChests.Framework.UserInterfaceService;

internal static class MenuManager
{
    public static PerScreen<WidgetHost> ScreenWidgetHost { get; } = new();

    public static void CreateMenu(ItemGrabMenu itemGrabMenu)
    {
        if (itemGrabMenu.context is not Chest chest) return;
        ScreenWidgetHost.Value =
            new WidgetHost(ModEntry.ModHelper.Events, ModEntry.ModHelper.Input, ModEntry.ModHelper.Reflection);
        var overlay = new ChestOverlay(itemGrabMenu, chest);
        ScreenWidgetHost.Value.RootWidget.AddChild(overlay);
    }

    public static void CreateMenu(GameMenu gameMenu)
    {
        ScreenWidgetHost.Value =
            new WidgetHost(ModEntry.ModHelper.Events, ModEntry.ModHelper.Input, ModEntry.ModHelper.Reflection);
        var overlay = new InventoryOverlay(gameMenu);
        ScreenWidgetHost.Value.RootWidget.AddChild(overlay);
    }

    public static void ClearMenu()
    {
        ScreenWidgetHost.Value?.Dispose();
        ScreenWidgetHost.Value = null;
    }
}