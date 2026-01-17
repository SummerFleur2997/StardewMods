using StardewValley.Menus;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForChefHat(Harmony harmony)
    {
        try
        {
            // If the mod moonslime.CookingSkill is loaded, do not patch the method.
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("moonslime.CookingSkill")) return;

            var original = AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe");
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_ChefHat_clickCraftingRecipe));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched CraftingPage.clickCraftingRecipe for chef hat successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for chef hat: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Set the crafted dish's quality to silver quality if the player is wearing the chef hat.
    /// </summary>
    private static void SetDishQualityToSilver(CraftingPage __instance, Item crafted)
    {
        if (PlayerHatIs(ChefHatID) && __instance.cooking && crafted.Quality == 0)
            crafted.Quality = 1;
    }

    /// <summary>
    /// Add a transpiler to the CraftingPage.clickCraftingRecipe method
    /// to set the quality of the dish.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_ChefHat_clickCraftingRecipe(IEnumerable<CodeInstruction> ci)
    {
        var codes = new List<CodeInstruction>(ci);

        // Find an anchor instruction for the injection
        var index = codes.FindIndex(c =>
            c.opcode == OpCodes.Call &&
            c.operand is MethodInfo { Name: "DoesFarmerHaveAdditionalIngredientsInInventory" });
        for (; index < codes.Count; index++)
        {
            var code = codes[index];
            if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo { Name: "set_Quality" }) break;
        }

        // If the anchor instruction is not found, throw an exception.
        if (index == codes.Count) throw new Exception("This method seems to have changed.");

        // Get the crafted field
        var craftedField = codes
            .FirstOrDefault(c => c.opcode == OpCodes.Ldfld && c.operand is FieldInfo { Name: "crafted" })
            ?.operand as FieldInfo;
        if (craftedField == null) throw new Exception("The crafted field is not found.");

        // Check again if the method is original and not be transpiled by another mod
        if (codes[index + 1].opcode == OpCodes.Br_S && codes[index + 2].opcode == OpCodes.Ldnull &&
            codes[index + 3].opcode == OpCodes.Stloc_1 && codes[index + 4].opcode == OpCodes.Ldarg_0)
        {
            var injection = new List<CodeInstruction>
            {
                new(OpCodes.Ldarg_0), // this
                new(OpCodes.Ldloc_0), // Item* crafted
                new(OpCodes.Ldfld, craftedField), // Item crafted
                new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(SetDishQualityToSilver)))
            };

            codes.InsertRange(index + 4, injection);
        }

        return codes;
    }
}