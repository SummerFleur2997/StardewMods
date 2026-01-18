namespace SummerFleursBetterHats.HatWithPatches;

public static partial class HatWithPatches
{
    public static void RegisterAllPatches(Harmony harmony)
    {
        RegisterPatchForSeasonalHats(harmony);
        RegisterPatchForChefHat(harmony);
        RegisterPatchForCowboyHats(harmony);
        RegisterPatchForGarbageHat(harmony);
        RegisterPatchForGnomesCap(harmony);
        RegisterPatchForGoldenHelmet(harmony);
        RegisterPatchForGoblinMask(harmony);
        RegisterPatchForGovernorsHat(harmony);
        RegisterPatchForJesterHat(harmony);
        RegisterPatchForLuckyBow(harmony);
        RegisterPatchForMonsterHats(harmony);
        RegisterPatchForPetsHats(harmony);
        RegisterPatchForPolkaBow(harmony);
        RegisterPatchForQiMask(harmony);
        RegisterPatchForTruckerHat(harmony);
    }
}