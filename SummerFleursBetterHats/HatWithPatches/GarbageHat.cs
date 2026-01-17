namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForGarbageHat(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.TryGetGarbageItem));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_GarbageHat_TryGetGarbageItem));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched GameLocation.TryGetGarbageItem for garbage hat successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for garbage hat: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Add 10% of the chance to find items in garbage can if the player is wearing the garbage hat.
    /// </summary>
    public static void SetGarbageChance(ref float baseChance)
    {
        if (PlayerHatIs(GarbageHatID))
            baseChance += 0.1f;
    }

    /// <summary>
    /// Add a transpiler to the <see cref="GameLocation.TryGetGarbageItem"/> method
    /// to add the chance of finding items in the garbage can.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_GarbageHat_TryGetGarbageItem(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find an anchor instruction for the injection
        var target = new CodeMatch(OpCodes.Ldstr, "Book_Trash");
        matcher.MatchStartForward(target).Advance(-2);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid) throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldloca_S, 3), // baseChance
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(SetGarbageChance)))
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}