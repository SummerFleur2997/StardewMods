using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Objects;

namespace BiggerContainers.Framework.CommonPatcher;

public static class PatchShowMenu
{
    public static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalM3 = AccessTools.Method(typeof(Chest), "GetActualCapacity");
            var prefixM3 = AccessTools.Method(
                typeof(PatchShowMenu), nameof(Patch_ShowMenu));
            harmony.Patch(original: originalM3, prefix: new HarmonyMethod(prefixM3));
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Warn);
        }
    }

    /// <summary>
    /// Patches <see cref="StardewValley.Objects.Chest.ShowMenu"/> method.
    /// 如果箱子被容量被模组修改，则重新生成其菜单。If this mod modified the chest's capacity,
    /// regenerate its menu UI.
    /// </summary>
    /// <param name="__instance">要重新生成菜单的箱子。The chest to re-shows its menu.</param>
    /// <returns>是否需要使用原方法进一步处理，若为 true 则使用原方法继续处理。
    /// Whether it needs to be proceeded with the original method.</returns>
    public static bool Patch_ShowMenu(Chest __instance)
    {
        if (!ModEntry.FridgesModule.IsActive || __instance.MyChestType() == ChestTypes.None) return true;

        switch (__instance.GetFridgeType())
        {
            case ChestTypes.Fridge when ModEntry.Config.BiggerFridge:
            case ChestTypes.MiniFridge when ModEntry.Config.BiggerMiniFridge:
                ShowMenu();
                return false;
            default:
                return true;
        }

        void ShowMenu()
        {
            Game1.activeClickableMenu = new ItemGrabMenu(
                __instance.GetItemsForPlayer(),
                reverseGrab: false,
                showReceivingMenu: true,
                InventoryMenu.highlightAllItems,
                __instance.grabItemFromInventory,
                null,
                __instance.grabItemFromChest,
                snapToBottom: false,
                canBeExitedWithKey: true,
                playRightClickSound: true,
                allowRightClick: true,
                showOrganizeButton: true,
                1, __instance, -1, __instance);
        }
    }
}