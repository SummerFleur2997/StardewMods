namespace SummerFleursBetterHats.HatWithPatches;

public static partial class HatWithPatches
{
    public static void RegisterAllPatches(Harmony harmony)
    {
        RegisterPatchForBlueBonnet(harmony);
        RegisterPatchForBucketHat(harmony);
        // RegisterPatchForChefHat(harmony);
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
        RegisterPatchForMysteryHat(harmony);
        RegisterPatchForOfficialCap(harmony);
        RegisterPatchForPolkaBow(harmony);
        RegisterPatchForRadioactiveGoggles(harmony);
        RegisterPatchForTruckerHat(harmony);
    }

    #region  Log Wrappers

    private static void Log(string s) => ModEntry.Log(s);
    private static void Error(string s) => ModEntry.Log(s, LogLevel.Error);

    #endregion

}