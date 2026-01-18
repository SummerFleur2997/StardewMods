namespace SummerFleursBetterHats.HatWithPatches;

/* After my injection, the method would like this:
    ...
    int numToHarvest = 1;

    var r = Utility.CreateRandom(xTile * 7.0, yTile * 5.0, Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame);
    if (crop.indexOfHarvest.Value == "889" && PlayerHatIs(QiMaskID) && r.NextDouble() < 0.1)
        numToHarvest++;

    if (data != null)
    ...
*/

public partial class HatWithPatches
{
    private static void RegisterPatchForQiMask(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Crop), nameof(Crop.harvest));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_QiMask_harvest));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Crop.harvest for qi mask successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for qi mask: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// 10% chance to get an extra fruit when harvesting qi fruit.
    /// </summary>
    public static void AddQiMelonCount(Crop crop, int xTile, int yTile, ref int numToHarvest)
    {
        // create an exclusive random, avoid potential effect to the original logic 
        var r = Utility.CreateRandom(xTile * 7.0, yTile * 5.0, Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame);
        if (crop.indexOfHarvest.Value == "889" && PlayerHatIs(QiMaskID) && r.NextDouble() < 0.1)
            numToHarvest++;
    }

    /// <summary>
    /// Add a transpiler to the <see cref="Crop.harvest"/> method
    /// to add an extra qi fruit (10%). 
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_QiMask_harvest(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find an anchor instruction for the injection
        var target = new CodeMatch(i => i.opcode == OpCodes.Stloc_S && i.operand is LocalBuilder { LocalIndex: 15 });
        matcher.MatchStartForward(target);
        matcher.Advance(1);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid) throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0), // this
            new(OpCodes.Ldarg_1), // xTile
            new(OpCodes.Ldarg_2), // yTile
            new(OpCodes.Ldloca_S, 15), // numToHarvest
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddQiMelonCount)))
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}