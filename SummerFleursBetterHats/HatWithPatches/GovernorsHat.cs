namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForGovernorsHat(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Event), "governorTaste");
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_GovernorsHat_governorTaste));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Event.governorTaste for governor's hat successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for governor's hat: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Friendship gains are doubled during the Luau Festival if
    /// the player is wearing the governor's hat.
    /// </summary>
    public static int DoubleFriendShip() => PlayerHatIs(GovernorsHatID) ? 2 : 1;

    /// <summary>
    /// Add a transpiler to the <see cref="Event.governorTaste"/> method
    /// to double the friendship bonus from the soup.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_GovernorsHat_governorTaste(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target1 = new CodeMatch[]
        {
            new(OpCodes.Ldc_I4_S, (sbyte)120),
            new(OpCodes.Ldstr, "Town")
        };
        var target2 = new CodeMatch[]
        {
            new(OpCodes.Ldc_I4_S, (sbyte)60),
            new(OpCodes.Ldstr, "Town")
        };

        foreach (var target in new[] { target1, target2 })
        {
            matcher.MatchStartForward(target).Advance(1);

            // If the anchor instruction is not found, throw an exception.
            if (matcher.IsInvalid)
                throw new Exception("This method seems to have changed.");

            // Add the injection to the codes
            var injection = new List<CodeInstruction>
            {
                new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(DoubleFriendShip))),
                new(OpCodes.Mul)
            };
            matcher.InsertAndAdvance(injection);
        }

        return matcher.InstructionEnumeration();
    }
}