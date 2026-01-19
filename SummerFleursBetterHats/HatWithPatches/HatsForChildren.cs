using StardewValley.Characters;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForHatsForChildren(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Child), nameof(Child.checkAction));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_ChildrensHats_checkAction));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Child.checkAction for children's hat successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for children's hat: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Get an extra 10 friendship if the child is wearing
    /// color bows.
    /// </summary>
    public static int AddFriendshipToChild(Child child) => child.hat.Value?.QualifiedItemId switch
    {
        BlueBowID when child.Gender is Gender.Male => 20,
        PinkBowID when child.Gender is Gender.Female => 20,
        _ => 0
    };

    public static IEnumerable<CodeInstruction> Patch_ChildrensHats_checkAction(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find an anchor instruction for the injection
        var target = new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Farmer), nameof(Farmer.talkToFriend)));
        matcher.MatchStartForward(target);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddFriendshipToChild))),
            new(OpCodes.Add)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}