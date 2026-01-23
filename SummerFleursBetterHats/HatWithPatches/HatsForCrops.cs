namespace SummerFleursBetterHats.HatWithPatches;

/* After my injection, the method would like this:
    ...
    int numToHarvest = ??; // this equals to the original value

    Random r = Utility.CreateRandom(xTile * 7.0, yTile * 5.0, Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame);
    double i = r.NextDouble();

    int amount = crop.indexOfHarvest.Value switch
    {
        _ when PlayerHatIs(JunimoHatID) && i > 0.8 => 1,
        "889" when PlayerHatIs(QiMaskID) && i > 0.9 => 1,
        "417" when PlayerHatIs(WhiteTurbanID) && i > 0.9 => 1,
        _ => 0
    };

    numToHarvest += amount;

    if (data != null)
    ...
*/

public partial class HatWithPatches
{
    private static void RegisterPatchForHatsForCrops(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Crop), nameof(Crop.harvest));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_HatsForCrops_harvest));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Crop.harvest for hats for crops successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for hats for crops: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// 10% chance to get an extra fruit when harvesting crops.
    /// </summary>
    public static void AddCropCount(Crop crop, int xTile, int yTile, ref int numToHarvest)
    {
        // create an exclusive random, avoid potential effect to the original logic 
        var r = Utility.CreateRandom(xTile * 7.0, yTile * 5.0, Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame);
        var i = r.NextDouble();

        var amount = crop.indexOfHarvest.Value switch
        {
            _ when PlayerHatIs(JunimoHatID) && i > 0.8 => 1,
            "889" when PlayerHatIs(QiMaskID) && i > 0.9 => 1,
            "417" when PlayerHatIs(WhiteTurbanID) && i > 0.9 => 1,
            _ => 0
        };

        numToHarvest += amount;
    }

    /// <summary>
    /// Add a transpiler to the <see cref="Crop.harvest"/> method
    /// to add an extra crop (10%). 
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_HatsForCrops_harvest(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find an anchor instruction for the injection
        var target = new CodeMatch(i => i.opcode == OpCodes.Stloc_S && i.operand is LocalBuilder { LocalIndex: 15 });
        matcher.MatchStartForward(target);
        matcher.Advance(1);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0), // this
            new(OpCodes.Ldarg_1), // xTile
            new(OpCodes.Ldarg_2), // yTile
            new(OpCodes.Ldloca_S, 15), // &numToHarvest
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddCropCount)))
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}