using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Locations;
using WhyNotJumpInThatMineShaft.Framework;

namespace WhyNotJumpInThatMineShaft;

public static class Patcher
{
    public static void Initialize(Harmony harmony)
    {
        try
        {
            var originalM1 = AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkAction));
            var prefixM1 = AccessTools.Method(typeof(ShaftChecker), nameof(ShaftChecker.Patch_checkAction));
            harmony.Patch(original: originalM1, prefix: new HarmonyMethod(prefixM1));
            ModEntry.Log("Patched MineShaft.checkAction successfully.");

            var originalM2 = AccessTools.Method(typeof(MineShaft), "doCreateLadderDown");
            var postfixM2 = AccessTools.Method(typeof(MapScanner), nameof(MapScanner.Patch_doCreateLadderDown));
            harmony.Patch(original: originalM2, postfix: new HarmonyMethod(postfixM2));
            ModEntry.Log("Patched MineShaft.checkAction successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }
}