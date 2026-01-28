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
            ModEntry.Log("Patched FishingRod.doneHoldingFish for bucket hat successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for bucket hat: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Multiply the chance of golden tag by 1.5 when the player is
    /// wearing the bucket hat.
    /// </summary>
    public static double AddGoldenTagChance() => PlayerHatIs(OfficialCapID) ? 1.5 : 1.0;

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
            new(OpCodes.Mul),
            new(OpCodes.Bge_Un_S),
            new(OpCodes.Ldarg_0)
        };
        matcher.MatchStartForward(target).Advance(1);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Mul),
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddGoldenTagChance)))
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}