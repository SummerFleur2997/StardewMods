using BetterHatsAPI.API;

namespace SummerFleursBetterHats.HatWithEffects;

public static partial class HatWithEffects
{
    private const string PackID = "SummerFleur.SummerFleursBetterHatsBHA";

    public static void RegisterCustomMethods(ISummerFleurBetterHatsAPI api)
    {
        api.SetCustomActionTrigger(BridalVeilID, PackID, Action_WeddingHats_AddFriendShip);
        api.SetCustomActionTrigger(LivingHatID, PackID, Action_LivingHat_Healing);
        api.SetCustomActionTrigger(SantaHatID, PackID, Action_SantaHat_AddMysteryBox);
        api.SetCustomActionTrigger(TopHatID, PackID, Action_WeddingHats_AddFriendShip);

        api.SetCustomConditionChecker(BridalVeilID, PackID, Condition_WeddingHats_IsFirstWedding);
        api.SetCustomConditionChecker(SpaceHelmetID, PackID, Condition_SpaceHelmet_InDifficultyMine);
        api.SetCustomConditionChecker(TopHatID, PackID, Condition_WeddingHats_IsFirstWedding);

        api.SetCustomBuffModifier(LogoCapID, PackID, Modifier_LogoCap_FishAreaInMine);
        api.SetCustomBuffModifier(SouwesterID, PackID, Modifier_Souwester_Stormy);
        api.SetCustomBuffModifier(SpaceHelmetID, PackID, Modifier_SpaceHelmet_MineDifficulty);
    }
}