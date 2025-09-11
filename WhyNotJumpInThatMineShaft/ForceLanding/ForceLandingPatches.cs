using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley.Locations;

namespace WhyNotJumpInThatMineShaft.ForceLanding;

internal static class ForceLandingPatches
{
    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.enterMineShaft"/> method.
    /// 确保不跳过 200 层和 300 层的宝箱层。
    /// Guarantee for the level 200 and 300 treasure room.
    /// </summary>
    /// <returns>List of transpiler</returns>
    public static IEnumerable<CodeInstruction> Patch_enterMineShaft(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        var index = codes.FindIndex(c => 
            c.opcode == OpCodes.Stfld && c.operand is FieldInfo { Name: "lastLevelsDownFallen" });

        if (index == -1) return codes;
        
        var injection = new List<CodeInstruction>
        {
            new (OpCodes.Ldarg_0),
            new (OpCodes.Ldloca_S, 0),
            new (OpCodes.Call, AccessTools.Method(typeof(ForceLandingPatches), nameof(AdjustLevelsDown)))
        };

        codes.InsertRange(index - 1, injection);
        return codes;
    }

    public static void AdjustLevelsDown(MineShaft __instance, ref int levelsDown)
    {
        if (__instance.mineLevel < 320 && __instance.mineLevel + levelsDown > 320)
        {
            levelsDown = 320 - __instance.mineLevel;
        }
        if (__instance.mineLevel < 420 && __instance.mineLevel + levelsDown > 420)
        {
            levelsDown = 420 - __instance.mineLevel;
        }
    }
}