using BiggerContainers.Framework;
using Common.ChestServices;
using StardewValley.Objects;

// using StardewValley;
// using StardewValley.Menus;

namespace BiggerContainers.BiggerFridges;

internal static class BiggerFridgesPatches
{
    public static bool ShouldPatchCCChestOverlay;

    /// <summary>
    /// Patches <see cref="StardewValley.Objects.Chest.GetActualCapacity"/> method.
    /// 如果箱子是一个有效的冰箱，并且相应的配置被启用，则修改其容量。If the chest is a valid fridge 
    /// and the corresponding configuration is enabled, modifies its capacity.
    /// </summary>
    /// <param name="__instance">要检查和调整容量的箱子。The chest to check and potentially modify.</param>
    /// <param name="__result">原函数的返回值。The return value of the original method.</param>
    /// <returns>是否需要使用原方法进一步处理，若为 true 则使用原方法继续处理。
    /// Whether it needs to be proceeded with the original method.</returns>
    public static bool Patch_GetActualCapacity(Chest __instance, ref int __result)
    {
        ShouldPatchCCChestOverlay = false;
        if (__instance.MyChestType() is not (ChestTypes.Fridge or ChestTypes.MiniFridge)) return true;
        if (!ModEntry.FridgesModule.IsActive) return true;

        switch (__instance.GetFridgeType())
        {
            case ChestTypes.Fridge when ModEntry.Config.BiggerFridge:
            case ChestTypes.MiniFridge when ModEntry.Config.BiggerMiniFridge:
                __result = 70;
                ShouldPatchCCChestOverlay = true;
                return false;
            default:
                return true;
        }
    }

    /* Abandoned method
    public static bool Patch_ShowMenu(Chest __instance)
    {
        if (!ModEntry.FridgesModule.IsActive || !__instance.IsValidFridge()) return true;

        ModEntry.Log("Checking for Big Fridge...");
        switch (__instance.GetFridgeType())
        {
            case ChestTypes.Fridge when ModEntry.Config.BiggerFridge:
            case ChestTypes.MiniFridge when ModEntry.Config.BiggerMiniFridge:
                ModEntry.Log("Big Fridge found, overriding menu.");
                ShowMenu();
                return false;
            default:
                ModEntry.Log("Big Fridge not found, using vanilla logic.");
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
    }*/
}