using System;
using Common;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Locations;

namespace WhyNotJumpInThatMineShaft.ForceLanding;

internal class ForceLandingModule : IModule
{
    private readonly Harmony _harmony = new (ModEntry.Manifest.UniqueID + ".ForceLanding");
    public bool IsActive { get; private set; }

    public void Activate()
    {
        IsActive = true;
        RegisterHarmonyPatches(_harmony);
    }

    public void Deactivate()
    {
        IsActive = false;
        _harmony.UnpatchAll(_harmony.Id);
    }

    private static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(MineShaft), nameof(MineShaft.enterMineShaft));
            var transpiler = AccessTools.Method(
                typeof(ForceLandingPatches), nameof(ForceLandingPatches.Patch_enterMineShaft));
            harmony.Patch(original: original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched MineShaft.enterMineShaft successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }
}