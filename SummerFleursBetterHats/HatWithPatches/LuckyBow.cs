namespace SummerFleursBetterHats.HatWithPatches;

/* After my injection, the property would like this:

    public double DailyLuck => Math.Min(
        Math.Max(
            sharedDailyLuck + (double)(hasSpecialCharm ? 0.025f : 0f) + PlayerHatIs(LuckyBowID) ? 0.015 : 0,
            -0.2),
        0.2);

*/

public partial class HatWithPatches
{
    private static void RegisterPatchForLuckyBow(Harmony harmony)
    {
        try
        {
            var original = AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.DailyLuck));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_LuckyBow_DailyLuck));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Farmer.DailyLuck for lucky bow successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for lucky bow: {ex.Message}", LogLevel.Error);
        }
    }

    public static double AddDailyLuck() => PlayerHatIs(LuckyBowID) ? 0.015 : 0;

    public static IEnumerable<CodeInstruction> Patch_LuckyBow_DailyLuck(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch(OpCodes.Add);
        matcher.MatchStartForward(target);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Add),
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddDailyLuck)))
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}