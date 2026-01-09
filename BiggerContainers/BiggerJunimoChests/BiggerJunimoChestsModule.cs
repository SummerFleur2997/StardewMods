using Common;
using HarmonyLib;
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
            var originalM2 = AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity));
            var prefixM2 = AccessTools.Method(
                typeof(BiggerJunimoChestsPatches), nameof(BiggerJunimoChestsPatches.Patch_GetActualCapacity));
            harmony.Patch(original: originalM2, prefix: new HarmonyMethod(prefixM2));
            ModEntry.Log("Patched Chest.GetActualCapacity for junimo chests successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }
}