namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForPolkaBow(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(NPC), nameof(NPC.grantConversationFriendship));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_PolkaBow_grantConversationFriendship));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched NPC.grantConversationFriendship for polka bow successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for polka bow: {ex.Message}", LogLevel.Error);
        }
    }

    public static void AddFriendship(ref int amount)
    {
        if (PlayerHatIs(PolkaBowID))
            amount += 10;
    }

    public static IEnumerable<CodeInstruction> Patch_PolkaBow_grantConversationFriendship
        (IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldarg_2),
            new(OpCodes.Ldarg_0)
        };
        matcher.MatchStartForward(target);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Create a new instruction to replace the old one
        var oldEntrance = matcher.InstructionAt(0);
        var newEntrance = new CodeInstruction(OpCodes.Ldarga_S, 2); // amount
        newEntrance.labels.AddRange(oldEntrance.labels);
        oldEntrance.labels.Clear();

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            newEntrance, // amount
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddFriendship)))
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}