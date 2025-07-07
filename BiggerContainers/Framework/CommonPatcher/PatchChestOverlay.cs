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
            var type = Type.GetType("ConvenientChests.Framework.UserInterfaceService.ChestOverlay, ConvenientChests");
            var originalM2 = AccessTools.Method(type, "GetOffset");
            var prefixM2 = AccessTools.Method(typeof(PatchChestOverlay), nameof(SetOffset));
            harmony.Patch(original: originalM2, prefix: new HarmonyMethod(prefixM2));
            ModEntry.Log("Patched ChestOverlay.Getoffset for fridges successfully.", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }

    public static bool SetOffset(ref int __result)
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