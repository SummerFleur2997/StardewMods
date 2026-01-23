namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForMysteryHat(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Utility), nameof(Utility.tryRollMysteryBox));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_MysteryHat_tryRollMysteryBox));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Utility.tryRollMysteryBox for mystery hat successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for mystery hat: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Multiply the acquisition chance of mystery box by 1.25 if
    /// the player is wearing the mystery hat.
    /// </summary>
    public static double AddMysteryBoxChance() => PlayerHatIs(MysteryHatID) ? 1.25 : 1.0;

    /// <summary>
    /// Add a transpiler to the <see cref="Utility.tryRollMysteryBox"/>
    /// method to add the chance of mystery box.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_MysteryHat_tryRollMysteryBox(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Clt),
            new(OpCodes.Ret)
        };
        matcher.MatchStartForward(target);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddMysteryBoxChance))),
            new(OpCodes.Mul)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}