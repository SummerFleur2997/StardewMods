using System;
using BiggerContainers.BiggerFridges;
using BiggerContainers.BiggerJunimoChests;
using HarmonyLib;
using StardewModdingAPI;

namespace BiggerContainers.Framework.CommonPatcher;

internal static class PatchChestOverlay
{
    public static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            if (ModEntry.ModHelper.ModRegistry.Get("SummerFleur.ConvenientChests") is null) return;
            var type = Type.GetType("ConvenientChests.Framework.UserInterfaceService.ChestOverlay, ConvenientChests");
            var originalM4 = AccessTools.Method(type, "GetOffset");
            var prefixM4 = AccessTools.Method(typeof(PatchChestOverlay), nameof(Patch_SetOffset));
            harmony.Patch(original: originalM4, prefix: new HarmonyMethod(prefixM4));
            ModEntry.Log("Patched ChestOverlay.Getoffset for fridges successfully.", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Warn);
        }
    }

    /// <summary>
    /// Patches ConvenientChests.Framework.UserInterfaceService.ChestOverlay method.
    /// </summary>
    /// <param name="__result">原函数的返回值。The return value of the original method.</param>
    /// <returns>是否需要使用原方法进一步处理，若为 true 则使用原方法继续处理。
    /// Whether it needs to be proceeded with the original method.</returns>
    public static bool Patch_SetOffset(ref int __result)
    {
        if (BiggerFridgesPatches.ShouldPatchCCChestOverlay)
        {
            __result = 128;
            return false;
        }
        if (BiggerJunimoChestsPatches.ShouldPatchCCChestOverlay)
        {
            __result = ModEntry.Config.BiggerJunimoChest switch
            {
                1 => 112,
                2 => 128,
                _ => 34
            };
            return false;
        }

        return true;
    }
}