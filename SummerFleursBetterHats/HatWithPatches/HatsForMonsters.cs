using StardewValley.Monsters;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    /// <summary>
    /// Patch for monster hats, double the damage to the special monsters.
    /// </summary>
    private static void RegisterPatchForHatsForMonsters(Harmony harmony)
    {
        try
        {
            var parameters = new[]
                { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) };
            var original1 = AccessTools.Method(typeof(Bat), nameof(Monster.takeDamage), parameters);
            var original2 = AccessTools.Method(typeof(Duggy), nameof(Monster.takeDamage), parameters);
            var original3 = AccessTools.Method(typeof(Ghost), nameof(Monster.takeDamage), parameters);
            var original4 = AccessTools.Method(typeof(Mummy), nameof(Monster.takeDamage), parameters);
            var original5 = AccessTools.Method(typeof(Serpent), nameof(Monster.takeDamage), parameters);
            var original6 = AccessTools.Method(typeof(Skeleton), nameof(Monster.takeDamage), parameters);

            var postfix1 = AccessTools.Method(typeof(HatWithPatches), nameof(Patch_Bat_takeDamage));
            var postfix2 = AccessTools.Method(typeof(HatWithPatches), nameof(Patch_Duggy_takeDamage));
            var postfix3 = AccessTools.Method(typeof(HatWithPatches), nameof(Patch_Ghost_takeDamage));
            var postfix4 = AccessTools.Method(typeof(HatWithPatches), nameof(Patch_Mummy_takeDamage));
            var postfix5 = AccessTools.Method(typeof(HatWithPatches), nameof(Patch_Serpent_takeDamage));
            var postfix6 = AccessTools.Method(typeof(HatWithPatches), nameof(Patch_Skeleton_takeDamage));

            harmony.Patch(original1, postfix: new HarmonyMethod(postfix1));
            harmony.Patch(original2, postfix: new HarmonyMethod(postfix2));
            harmony.Patch(original3, postfix: new HarmonyMethod(postfix3));
            harmony.Patch(original4, postfix: new HarmonyMethod(postfix4));
            harmony.Patch(original5, postfix: new HarmonyMethod(postfix5));
            harmony.Patch(original6, postfix: new HarmonyMethod(postfix6));

            ModEntry.Log("Patched Monster.takeDamage for monster hats successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for monster hats: {ex.Message}", LogLevel.Error);
        }
    }

    /********
     * Postfixes （别看了）
     ********/

    public static void Patch_Bat_takeDamage(Bat __instance, ref int __result)
    {
        switch (__instance.Name)
        {
            case "Iridium Bat" when PlayerHatIs(GoldenMaskID):
            case "Magma Sprite" when PlayerHatIs(SwashbucklerHatID):
            case "Magma Sparker" when PlayerHatIs(SwashbucklerHatID):
                __result = (int)(1.5 * __result);
                return;
        }
    }

    public static void Patch_Duggy_takeDamage(ref int __result)
    {
        if (PlayerHatIs(HardHatID))
            __result = (int)(1.5 * __result);
    }

    public static void Patch_Ghost_takeDamage(ref int __result)
    {
        if (PlayerHatIs(ArcaneHatID))
            __result = (int)(1.5 * __result);
    }

    public static void Patch_Mummy_takeDamage(ref int __result)
    {
        if (PlayerHatIs(ArcaneHatID))
            __result = (int)(1.5 * __result);
    }

    public static void Patch_Serpent_takeDamage(ref int __result)
    {
        if (PlayerHatIs(GoldenMaskID))
            __result = (int)(1.5 * __result);
    }

    public static void Patch_Skeleton_takeDamage(ref int __result)
    {
        if (PlayerHatIs(SkeletonMaskID))
            __result = (int)(1.5 * __result);
    }
}