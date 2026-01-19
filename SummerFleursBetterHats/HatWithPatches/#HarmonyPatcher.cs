namespace SummerFleursBetterHats.HatWithPatches;

public static partial class HatWithPatches
{
    public static void RegisterAllPatches(Harmony harmony)
    {
        RegisterPatchForChefHat(harmony);
        RegisterPatchForGarbageHat(harmony);
        RegisterPatchForGnomesCap(harmony);
        RegisterPatchForGoldenHelmet(harmony);
        RegisterPatchForGoblinMask(harmony);
        RegisterPatchForGovernorsHat(harmony);
        RegisterPatchForHatsForBushBerries(harmony);
        RegisterPatchForHatsForChildren(harmony);
        RegisterPatchForHatsForCrops(harmony);
        RegisterPatchForHatsForHorse(harmony);
        RegisterPatchForHatsForMonsters(harmony);
        RegisterPatchForHatsForPets(harmony);
        RegisterPatchForJesterHat(harmony);
        RegisterPatchForLuckyBow(harmony);
        RegisterPatchForMushroomCap(harmony);
        RegisterPatchForPolkaBow(harmony);
        RegisterPatchForTruckerHat(harmony);
    }
}