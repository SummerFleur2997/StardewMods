using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley.Objects;
using UI;

namespace ConvenientChests.Framework.UserInterfaceService;

internal static class MenuManager
{
    public static PerScreen<BaseOverlay?> ScreenWidgetHost { get; } = new();
    private static IModEvents Events => ModEntry.ModHelper.Events;
    private static IInputHelper Input => ModEntry.ModHelper.Input;
    private static IReflectionHelper Reflection => ModEntry.ModHelper.Reflection;

    public static void CreateMenu(ItemGrabMenu itemGrabMenu)
    {
        // Ensure that the menu is for a chest
        if (itemGrabMenu.context is not Chest chest)
            return;

        // And not an enricher or mini shipping bin
        if (chest.SpecialChestType is Chest.SpecialChestTypes.Enricher or Chest.SpecialChestTypes.MiniShippingBin)
            return;

        ScreenWidgetHost.Value?.Dispose();
        var overlay = new ChestOverlay(itemGrabMenu, chest);
        ScreenWidgetHost.Value = new MenuHost<ItemGrabMenu>(Events, Input, Reflection, overlay);
    }

    public static void CreateMenu(GameMenu gameMenu)
    {
        ScreenWidgetHost.Value?.Dispose();
        var overlay = new InventoryOverlay(gameMenu);
        ScreenWidgetHost.Value = new MenuHost<GameMenu>(Events, Input, Reflection, overlay);
    }

    public static void ClearMenu()
    {
        ScreenWidgetHost.Value?.Dispose();
        ScreenWidgetHost.Value = null;
    }
}