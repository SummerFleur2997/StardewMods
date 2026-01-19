using StardewValley.Characters;

namespace SummerFleursBetterHats.HatWithPatches;

/* After my injection, the method would like this:
    ...
    friendshipTowardFarmer.Set(...);

    if (... < (GetPetData().GiftChance + AddGiftChance(this pet)))
    ...
*/

public partial class HatWithPatches
{
    private static void RegisterPatchForHatsForPets(Harmony harmony)
    {
        try
        {
            var delegateMethodType = typeof(Pet).GetNestedType("<>c__DisplayClass59_0", BindingFlags.NonPublic);
            var original = delegateMethodType?.GetMethod("<checkAction>b__0",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_PetsHats_checkAction));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Pet.checkAction for pet's hat successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for pet's hat: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Add 10% of the chance to get gifts from the pet if the
    /// hat is a Fishing Hat for cats or a Hair Bone for dogs.
    /// </summary>
    public static double AddGiftChance(Pet pet) => pet.hat.Value?.QualifiedItemId switch
    {
        HairBoneID when pet is { petType.Value: Pet.type_dog } => 0.1,
        FishingHatID when pet is { petType.Value: Pet.type_cat } => 0.1,
        _ => 0
    };


    /// <summary>
    /// Add a transpiler to the <see cref="Pet.checkAction"/> method
    /// to add the chance of gifts.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_PetsHats_checkAction(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Conv_R8),
            new(OpCodes.Bge_Un_S),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld)
        };
        matcher.MatchStartForward(target).Advance(1);
        var operand = matcher.InstructionAt(2).operand;

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld, operand),
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(AddGiftChance))),
            new(OpCodes.Add)
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}