using StardewValley.TerrainFeatures;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForHatsForBushBerries(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Bush), nameof(Bush.shake));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_HatsForBushBerries_shake));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Bush.shake for berry hats successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for berry hats: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// An extra berry if the player is wearing the butterfly bow or the pumpkin mask.
    /// </summary>
    public static int AddBerryCount(Bush bush) => PlayerHat()?.QualifiedItemId switch
    {
        ButterflyBowID when bush.GetShakeOffItem() == "(O)296" => 1,
        PumpkinMaskID when bush.GetShakeOffItem() == "(O)410" => 1,
        _ => 0
    };


    /// <summary>
    /// Add a transpiler to the <see cref="Bush.shake"/> method
    /// to add an extra berry. 
    /// </summary>
    private static IEnumerable<CodeInstruction> Patch_HatsForBushBerries_shake(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find an anchor instruction for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Stloc_2),
            new(OpCodes.Ldc_I4_0),
            new(OpCodes.Stloc_3),
            new(OpCodes.Br_S)
        };
        matcher.MatchStartForward(target);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddBerryCount))),
            new(OpCodes.Add)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}