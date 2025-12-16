using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace SummerFleursBetterHats.HatWithPatches;

public static partial class HatWithPatches
{
    private const string GarbageHatID = "(H)66";

    public static void RegisterHarmonyPatchForGarbageHat(Harmony harmony)
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
            ModEntry.Log($"Failed to patch method TryGetGarbageItem: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Set the chance to find items in garbage can if the player is wearing the garbage hat.
    /// </summary>
    public static void SetGarbageChance(ref float baseChance)
    {
        if (Utilities.PlayerHatIs(GarbageHatID))
            baseChance += 0.1f;
    }

    /// <summary>
    /// Add a transpiler to the CraftingPage.clickCraftingRecipe method
    /// to set the quality of the dish.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_GarbageHat_TryGetGarbageItem(IEnumerable<CodeInstruction> ci)
    {
        var codes = new List<CodeInstruction>(ci);

        // Find an anchor instruction for the injection
        var index = codes.FindIndex(c =>
            c.opcode == OpCodes.Ldstr && c.operand is "Book_Trash");

        // If the anchor instruction is not found, throw an exception.
        if (index == -1) throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldloca_S, 3), // baseChance
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(SetGarbageChance)))
        };

        codes.InsertRange(index - 2, injection);

        return codes;
    }
}