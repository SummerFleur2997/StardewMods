using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private const string GoblinMaskID = "(H)9";

    public static void RegisterPatchForGoblinMask(Harmony harmony)
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
    /// Scare the NPC and make it unavailable to talk.
    /// </summary>
    public static bool ScareNPC(NPC __instance)
    {
        if (!Utilities.PlayerHatIs(GoblinMaskID))
            return false;

        if (__instance.Name is "Krobus" or "Dwarf")
            return false;

        if (__instance.yJumpVelocity != 0f || __instance.Sprite.CurrentAnimation != null)
            return true;

        switch (__instance.Manners)
        {
            case NPC.rude:
                __instance.doEmote(Character.angryEmote);
                break;
            case NPC.polite:
                __instance.doEmote(Character.questionMarkEmote);
                break;
            default:
                __instance.doEmote(Character.exclamationEmote);
                __instance.jump();
                break;
        }

        __instance.faceTowardFarmerForPeriod(2000, 3, false, Game1.player);
        return true;
    }

    /// <summary>
    /// Add a transpiler to the CraftingPage.clickCraftingRecipe method
    /// to set the quality of the dish.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_GoblinMask_checkAction(IEnumerable<CodeInstruction> ci,
        ILGenerator il)
    {
        var matcher = new CodeMatcher(ci);

        // Find an anchor instruction for the injection
        var target = new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(NPC), nameof(NPC.CanReceiveGifts)));
        matcher.MatchStartForward(target).Advance(-1);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid) throw new Exception("This method seems to have changed.");

        var label = il.DefineLabel();
        matcher.AddLabels(new[] { label });

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(ScareNPC))),
            new(OpCodes.Brfalse_S, label),
            new(OpCodes.Ldc_I4_0),
            new(OpCodes.Ret)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}