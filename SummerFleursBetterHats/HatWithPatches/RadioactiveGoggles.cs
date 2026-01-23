using StardewValley.Locations;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForRadioactiveGoggles(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(MineShaft), "createLitterObject");
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_RadioactiveGoggles_createLitterObject));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched MineShaft.createLitterObject for radioactive goggles successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for radioactive goggles: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Add 5% of the spawn chance to radioactive nodes if
    /// the player is wearing the radioactive goggles.
    /// </summary>
    public static double AddRadioactiveNodeChance() => PlayerHatIs(RadioactiveGogglesID) ? 0.004 : 0;

    /// <summary>
    /// Add a transpiler to the <see cref="MineShaft.createLitterObject"/>
    /// method to add the spawn chance of radioactive nodes.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_RadioactiveGoggles_createLitterObject
        (IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Add),
            new(OpCodes.Bge_Un_S),
            new(OpCodes.Ldstr, "95")
        };
        matcher.MatchStartForward(target).Advance(1);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddRadioactiveNodeChance))),
            new(OpCodes.Add)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}