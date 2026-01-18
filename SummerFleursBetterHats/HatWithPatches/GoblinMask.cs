namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForGoblinMask(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(NPC), nameof(NPC.checkAction));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_GoblinMask_checkAction));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched NPC.checkAction for goblin mask successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for goblin mask: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Scare the NPC and make them unavailable to talk.
    /// </summary>
    public static bool ScareNPC(NPC npc)
    {
        try
        {
            if (!PlayerHatIs(GoblinMaskID))
                return false;

            if (npc.Name is "Krobus" or "Dwarf")
                return false;

            if (npc.yJumpVelocity != 0f || npc.Sprite.CurrentAnimation != null)
                return true;

            switch (npc.Manners)
            {
                case NPC.rude:
                    npc.doEmote(Character.angryEmote);
                    break;
                case NPC.polite:
                    npc.doEmote(Character.questionMarkEmote);
                    break;
                default:
                    npc.doEmote(Character.exclamationEmote);
                    npc.jump();
                    break;
            }

            npc.faceTowardFarmerForPeriod(2000, 3, false, Game1.player);
            return true;
        }
        catch (Exception ex)
        {
            ModEntry.Log("Failed to scare NPC: " + ex.Message, LogLevel.Error);
            return false;
        }
    }

    /// <summary>
    /// Add a transpiler to the <see cref="NPC.checkAction"/> method
    /// to scare the npc.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_GoblinMask_checkAction
        (IEnumerable<CodeInstruction> ci, ILGenerator il)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target1 = new CodeMatch[]
        {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Callvirt),
            new(OpCodes.Callvirt),
            new(OpCodes.Ldc_I4_1)
        };
        var target2 = new CodeMatch[]
        {
            new(OpCodes.Ldloc_2),
            new(OpCodes.Brfalse)
        };

        foreach (var target in new[] { target1, target2 })
        {
            matcher.MatchStartForward(target);

            // If the anchor instruction is not found, throw an exception.
            if (matcher.IsInvalid)
                throw new Exception("This method seems to have changed.");

            // Create a new instruction to replace the old one
            var oldEntrance = matcher.InstructionAt(0);
            var newEntrance = new CodeInstruction(OpCodes.Ldarg_0);
            newEntrance.labels.AddRange(oldEntrance.labels);
            oldEntrance.labels.Clear();

            // Add a label to the old entrance for the jump instruction
            var label = il.DefineLabel();
            oldEntrance.labels.Add(label);

            // Add the injection to the codes
            var injection = new List<CodeInstruction>
            {
                newEntrance, // this
                new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(ScareNPC))),
                new(OpCodes.Brfalse_S, label),
                new(OpCodes.Ldc_I4_0), // false
                new(OpCodes.Ret)
            };
            matcher.InsertAndAdvance(injection);
        }

        return matcher.InstructionEnumeration();
    }
}