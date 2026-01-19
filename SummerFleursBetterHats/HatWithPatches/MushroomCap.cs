using Object = StardewValley.Object;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForMushroomCap(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Object), nameof(Object.cutWeed));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_MushroomCap_cutWeed));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Object.cutWeed for mushroom cap successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for mushroom cap: {ex.Message}", LogLevel.Error);
        }
    }

    public static double AddMixedSeedChance() => PlayerHatIs(MushroomCapID) ? 0.05 : 0;

    public static IEnumerable<CodeInstruction> Patch_MushroomCap_cutWeed(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldfld),
            new(OpCodes.Ldstr, "Book_WildSeeds")
        };

        // check twice
        for (var i = 0; i < 2; i++)
        {
            matcher.MatchStartForward(target);

            // If the anchor instruction is not found, throw an exception.
            if (matcher.IsInvalid)
                throw new Exception("This method seems to have changed.");

            // Add the injection to the codes
            var injection = new List<CodeInstruction>
            {
                new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddMixedSeedChance))),
                new(OpCodes.Add)
            };
            matcher.InsertAndAdvance(injection);
            matcher.Advance(3);
        }

        return matcher.InstructionEnumeration();
    }
}