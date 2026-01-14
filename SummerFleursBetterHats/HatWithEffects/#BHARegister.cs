using BetterHatsAPI.API;

namespace SummerFleursBetterHats.HatWithEffects;

public static partial class HatWithEffects
{
    private const string PackID = "SummerFleur.SummerFleursBetterHatsBHA";

    public static void RegisterCustomMethods()
    {
        var api = ModEntry.ModHelper.ModRegistry.GetApi<ISummerFleurBetterHatsAPI>("SummerFleur.BetterHatsAPI");
        if (api == null) return;

        api.SetCustomActionTrigger(LivingHatID, PackID, Action_LivingHat_Healing);
        api.SetCustomActionTrigger(SantaHatID, PackID, Action_SantaHat_AddMysteryBox);

        api.SetCustomConditionChecker(SpaceHelmetID, PackID, Condition_SpaceHelmet_InDifficultyMine);

        api.SetCustomBuffModifier(LogoCapID, PackID, Modifier_LogoCap_FishAreaInMine);
        api.SetCustomBuffModifier(SouwesterID, PackID, Modifier_Souwester_Stormy);
        api.SetCustomBuffModifier(SpaceHelmetID, PackID, Modifier_SpaceHelmet_MineDifficulty);
    }
}