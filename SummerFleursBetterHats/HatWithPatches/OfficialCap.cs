using StardewValley.Tools;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForOfficialCap(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(FishingRod), nameof(FishingRod.startMinigameEndFunction));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_OfficialCap_startMinigameEndFunction));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched FishingRod.startMinigameEndFunction for official cap successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for official cap: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Double the chance of golden treasure when the player is
    /// wearing the official cap.
    /// </summary>
    public static double DoubleGoldenTreasureChance() => PlayerHatIs(OfficialCapID) ? 2.0 : 1.0;

    /// <summary>
    /// Add a transpiler to the <see cref="FishingRod.startMinigameEndFunction"/>
    /// method to double the chance of golden treasure when the player is wearing
    /// the official cap.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_OfficialCap_startMinigameEndFunction
        (IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Add),
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
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(DoubleGoldenTreasureChance))),
            new(OpCodes.Mul)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}