using StardewValley.Tools;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForBucketHat(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(FishingRod), nameof(FishingRod.doneHoldingFish));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_BucketHat_doneHoldingFish));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            Log("Patched FishingRod.doneHoldingFish for bucket hat successfully.");
        }
        catch (Exception ex)
        {
            Error($"Failed to patch for bucket hat: {ex.Message}");
        }
    }

    /// <summary>
    /// Multiply the chance of golden tag by 1.5 when the player is
    /// wearing the bucket hat.
    /// </summary>
    public static double AddGoldenTagChance() => PlayerHatIs(BucketHatID) ? 1.5 : 1.0;

    /// <summary>
    /// Add a transpiler to the <see cref="FishingRod.doneHoldingFish"/>
    /// method to multiply the chance of golden tag.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_BucketHat_doneHoldingFish(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Ldc_R8, 0.33),
            new(OpCodes.Ldarg_0)
        };
        matcher.MatchStartForward(target).Advance(1);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddGoldenTagChance))),
            new(OpCodes.Mul)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}