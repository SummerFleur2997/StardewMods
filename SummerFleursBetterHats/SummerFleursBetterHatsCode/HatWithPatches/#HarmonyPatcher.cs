using HarmonyLib;

namespace SummerFleursBetterHats.HatWithPatches;

public static partial class HatWithPatches
{
    public static void RegisterAllPatches(Harmony harmony)
    {
        RegisterPatchForSeasonalHats(harmony);
        RegisterPatchForChefHat(harmony);
        RegisterPatchForCowboyHats(harmony);
        RegisterPatchForGarbageHat(harmony);
        RegisterPatchForGoldenHelmet(harmony);
        RegisterPatchForGoblinMask(harmony);
        RegisterPatchForQiMask(harmony);
        RegisterPatchForTruckerHat(harmony);
    }
}