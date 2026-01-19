using xTile.Dimensions;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForJesterHat(Harmony harmony)
    {
        try
        {
            var original1 = AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject));
            var transpiler1 = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_Ticket_tryToReceiveActiveObject));
            harmony.Patch(original1, transpiler: new HarmonyMethod(transpiler1));
            ModEntry.Log("Patched NPC.tryToReceiveActiveObject for discount ticket successfully.");

            var original2 = AccessTools.Method(
                typeof(GameLocation), nameof(GameLocation.performAction),
                new[] { typeof(string[]), typeof(Farmer), typeof(Location) });
            var transpiler2 = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_MovieTheater_performAction));
            harmony.Patch(original2, transpiler: new HarmonyMethod(transpiler2));
            ModEntry.Log("Patched GameLocation.performAction for movie theater successfully.");

            var original3 = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction));
            var postfix3 = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_MovieTheater_answerDialogueAction));
            harmony.Patch(original3, postfix: new HarmonyMethod(postfix3));
            ModEntry.Log("Patched GameLocation.answerDialogueAction for movie theater successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for jester hat: {ex.Message}", LogLevel.Error);
        }
    }

    public static IEnumerable<CodeInstruction> Patch_Ticket_tryToReceiveActiveObject
        (IEnumerable<CodeInstruction> ci, ILGenerator il)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target1 = new CodeMatch[]
        {
            new(OpCodes.Ldloc_S),
            new(OpCodes.Ldstr, "(O)809")
        };
        matcher.MatchStartForward(target1);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        var label = il.DefineLabel();
        var operand1 = matcher.InstructionAt(0).operand;
        var operand2 = matcher.InstructionAt(2).operand;

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldloc_S, operand1),
            new(OpCodes.Ldstr, "(O)SummerFleur.BetterHats.DiscountedTicket"),
            new(OpCodes.Call, operand2),
            new(OpCodes.Brtrue_S, label)
        };
        matcher.InsertAndAdvance(injection);

        // Find another target to add the label
        var target2 = new CodeMatch(OpCodes.Ldstr, "ccMovieTheater");
        matcher.MatchStartForward(target2);

        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        matcher.InstructionAt(0).labels.Add(label);

        return matcher.InstructionEnumeration();
    }

    public static IEnumerable<CodeInstruction> Patch_MovieTheater_performAction(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target1 = new CodeMatch[]
        {
            new(OpCodes.Call),
            new(OpCodes.Callvirt),
            new(OpCodes.Ldstr, "(O)809"),
            new(OpCodes.Callvirt),
            new(OpCodes.Brfalse_S)
        };
        matcher.MatchStartForward(target1);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        var operand4 = matcher.InstructionAt(4).operand;
        var operand5 = matcher.InstructionAt(5).operand;

        var oldEntrance = matcher.InstructionAt(0);
        var newEntrance = new CodeInstruction(OpCodes.Call,
            AccessTools.Method(typeof(HatWithPatches), nameof(HaveDiscountTicket)));
        newEntrance.labels.AddRange(oldEntrance.labels);
        oldEntrance.labels.Clear();

        var exit = new CodeInstruction(OpCodes.Br);

        var injection = new List<CodeInstruction>
        {
            newEntrance,
            new(OpCodes.Brfalse_S, operand4), // goto no ticket
            new(OpCodes.Ldloc_S, operand5), // invited_npc
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(DrawDialogue))),
            exit
        };
        matcher.InsertAndAdvance(injection);

        // Find target instructions for the label
        var target2 = new CodeMatch[]
        {
            new(OpCodes.Ldstr, "EnterTheaterSpendTicket"),
            new(OpCodes.Callvirt),
            new(OpCodes.Br)
        };
        matcher.MatchStartForward(target2);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        var exitLabel = matcher.InstructionAt(2).operand;
        exit.operand = exitLabel;

        return matcher.InstructionEnumeration();
    }

    public static bool HaveDiscountTicket()
        => Game1.player.Items.ContainsId("(O)SummerFleur.BetterHats.DiscountedTicket");

    public static void DrawDialogue(NPC invited)
    {
        var question = invited != null
            ? Game1.content.LoadString("Strings\\Characters:MovieTheater_WatchWithFriendPrompt", invited.displayName)
            : Game1.content.LoadString("Strings\\Characters:MovieTheater_WatchAlonePrompt");
        Game1.currentLocation.createQuestionDialogue(question,
            Game1.currentLocation.createYesNoResponses(),
            "EnterTheaterSpendDiscountTicket");
    }

    public static void Patch_MovieTheater_answerDialogueAction(string questionAndAnswer)
    {
        if (questionAndAnswer != "EnterTheaterSpendDiscountTicket_Yes") return;

        Game1.player.Items.ReduceId("(O)SummerFleur.BetterHats.DiscountedTicket", 1);
        Rumble.rumble(0.15f, 200f);
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.currentLocation.playSound("doorClose", Game1.player.Tile);
        Game1.warpFarmer("MovieTheater", 13, 15, 0);
    }
}