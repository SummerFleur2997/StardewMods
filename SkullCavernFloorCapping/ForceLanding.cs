using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Locations;

namespace SkullCavernFloorCapping;

internal class ForceLanding
{
    private static readonly Harmony Harmony = new (ModEntry.Manifest.UniqueID);

    public static void Activate() => RegisterHarmonyPatches(Harmony);

    public static void Deactivate() => Harmony.UnpatchAll(Harmony.Id);

    private static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(MineShaft), nameof(MineShaft.enterMineShaft));
            var transpiler = AccessTools.Method(typeof(ForceLanding), nameof(Patch_enterMineShaft));
            harmony.Patch(original: original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched MineShaft.enterMineShaft successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }

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
            new (OpCodes.Call, AccessTools.Method(typeof(ForceLanding), nameof(AdjustLevelsDown)))
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