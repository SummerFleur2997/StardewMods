using System;
using Common;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Objects;

namespace BiggerContainers.BiggerJunimoChests;

internal class BiggerJunimoChestsModule : IModule
{
    public bool IsActive => ModEntry.Config.BiggerJunimoChest is 1 or 2;

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
            var originalM3 = AccessTools.Method(typeof(Chest), "GetActualCapacity");
            var prefixM3 = AccessTools.Method(
                typeof(BiggerJunimoChestsPatches), nameof(BiggerJunimoChestsPatches.Patch_GetActualCapacity));
            harmony.Patch(original: originalM3, prefix: new HarmonyMethod(prefixM3));
            ModEntry.Log("Patched Chest.GetActualCapacity for junimo chests successfully.", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }
}