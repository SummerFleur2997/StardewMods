namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Condition for the Bridal Veil and the Top Hat: Today is
    /// the player's first wedding ceremony.
    /// </summary>
    private static bool Condition_WeddingHats_IsFirstWedding()
    {
        var isGenderFit = Game1.player.Gender switch
        {
            Gender.Male when PlayerHatIs(TopHatID) => true,
            Gender.Female when PlayerHatIs(BridalVeilID) => true,
            _ => false
        };

        return Game1.weddingToday && isGenderFit && !Game1.player.eventsSeen.Contains("-2");
    }

    /// <summary>
    /// Action for the Bridal Veil and the Top Hat: add 250
    /// friendship points to the player's spouse.
    /// </summary>
    private static void Action_WeddingHats_AddFriendShip()
    {
        var spouse = Game1.player.getSpouse();
        Game1.player.changeFriendship(380, spouse); // 380 * 0.66 ≈ 250
    }
}