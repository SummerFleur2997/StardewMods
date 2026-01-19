namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static readonly HashSet<string> CowboyHatsID = new()
    {
        CowgalHatID,
        CowpokeHatID,
        BlueCowboyHatID,
        RedCowboyHatID,
        DarkCowboyHatID
    };

    private static void RegisterPatchForHatsForHorse(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_HatsForHorse_getMovementSpeed));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Farmer.getMovementSpeed for horse hats successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for horse hats: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Add 0.5 speed if the player is riding a horse, or 1
    /// speed if the horse is wearing a small cap.
    /// </summary>
    public static float AddRidingHorseSpeed(Farmer farmer) =>
        (PlayerHatIn(CowboyHatsID) ? 0.5f : 0) + (farmer.mount.hat.Value?.QualifiedItemId == SmallCapID ? 1f : 0);

    /// <summary>
    /// Add a transpiler to the <see cref="Farmer.getMovementSpeed"/> method
    /// to add extra movement speed.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_HatsForHorse_getMovementSpeed(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find an anchor instruction for the injection
        var target = new CodeMatch(OpCodes.Ldstr, "Book_Horse");
        matcher.MatchStartForward(target).Advance(-2);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0), // this
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddRidingHorseSpeed))),
            new(OpCodes.Add)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}