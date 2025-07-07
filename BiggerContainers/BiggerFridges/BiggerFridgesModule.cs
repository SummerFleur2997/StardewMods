using System;
using Common;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Objects;

namespace BiggerContainers.BiggerFridges;

internal class BiggerFridgesModule : IModule
{
    public bool IsActive => ModEntry.Config.BiggerFridge || ModEntry.Config.BiggerMiniFridge;

    public void Activate()
    {
        lock (ModEntry.Config)
        {
            var harmony = ModEntry.Harmony;
            RegisterHarmonyPatches(harmony);
        }
    }

    public void Deactivate() { }

    private static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalM1 = AccessTools.Method(typeof(Chest), "GetActualCapacity");
            var prefixM1 = AccessTools.Method(
                typeof(BiggerFridgesPatches), nameof(BiggerFridgesPatches.Patch_GetActualCapacity));
            harmony.Patch(original: originalM1, prefix: new HarmonyMethod(prefixM1));
            ModEntry.Log("Patched Chest.GetActualCapacity for fridges successfully.", LogLevel.Debug);

            // var type = Type.GetType("ConvenientChests.Framework.UserInterfacService.ChestOverlay, ConvenientChests");
            // var originalM2 = AccessTools.Method(type, "GetOffset");
            // var prefixM2 = AccessTools.Method(
            //     typeof(BiggerFridgesPatches), nameof(BiggerFridgesPatches.Patch_CC_ChestOverlay_GetOffset));
            // harmony.Patch(original: originalM2, prefix: new HarmonyMethod(prefixM2));
            // ModEntry.Log("Patched ChestOverlay.Getoffset for fridges successfully.", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }
}