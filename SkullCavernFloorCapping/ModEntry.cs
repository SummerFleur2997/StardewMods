using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Locations;

namespace SkullCavernFloorCapping;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    private static IMonitor ModMonitor { get; set; }
    private static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    #endregion

    public override void Entry(IModHelper helper)
    {
        ModMonitor = Monitor;
        var harmony = new Harmony(ModManifest.UniqueID);
        RegisterHarmonyPatches(harmony);
    }

    private static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(MineShaft), nameof(MineShaft.enterMineShaft));
            var transpiler = AccessTools.Method(typeof(ModEntry), nameof(Patch_enterMineShaft));
            harmony.Patch(original: original, transpiler: new HarmonyMethod(transpiler));
            Log("Patched MineShaft.enterMineShaft successfully.");
        }
        catch (Exception ex)
        {
            Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
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
            new (OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(AdjustLevelsDown)))
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