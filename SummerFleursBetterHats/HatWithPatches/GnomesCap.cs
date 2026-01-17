namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForGnomesCap(Harmony harmony)
    {
        try
        {
            var delegateMethodType = typeof(Utility).GetNestedType("<>c", BindingFlags.NonPublic);
            var original = delegateMethodType?.GetMethod("<pickFarmEvent>b__213_0",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_GnomesCap_pickFarmEvent));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Utility.pickFarmEvent for gnome's cap successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for gnome's cap: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Add 10% of the chance to trigger fairy event for each player wearing gnome's cap.
    /// </summary>
    public static void AddFairyChance(ref double additionalChance)
    {
        foreach (var player in Game1.getOnlineFarmers())
            if (player.HatIs(GnomesCapID))
                additionalChance += 0.01;
    }

    /// <summary>
    /// Add a transpiler to the <see cref="Utility.pickFarmEvent"/> method
    /// to add the chance of fairy event.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_GnomesCap_pickFarmEvent(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find an anchor instruction for the injection
        var target = new CodeMatch(OpCodes.Ldc_R8, 0.007);
        matcher.MatchStartForward(target).Advance(2);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid) throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldloca_S, 1),
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddFairyChance)))
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}