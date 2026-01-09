using HarmonyLib;
using StardewValley.Locations;

namespace WhyNotJumpInThatMineShaft.ShaftPrompter;

internal static class MapScannerPatches
{
    public static void Initialize(Harmony harmony)
    {
        try
        {
            var originalM1 = AccessTools.Method(typeof(MineShaft), "doCreateLadderAt");
            var postfixM1 = AccessTools.Method(typeof(MapScanner), nameof(MapScanner.Patch_doCreateLadderAt));
            harmony.Patch(originalM1, postfix: new HarmonyMethod(postfixM1));
            ModEntry.Log("Patched MineShaft.doCreateLadderAt successfully.");

            var originalM2 = AccessTools.Method(typeof(MineShaft), "doCreateLadderDown");
            var postfixM2 = AccessTools.Method(typeof(MapScanner), nameof(MapScanner.Patch_doCreateLadderDown));
            harmony.Patch(originalM2, postfix: new HarmonyMethod(postfixM2));
            ModEntry.Log("Patched MineShaft.doCreateLadderDown successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }
}