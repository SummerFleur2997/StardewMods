using StardewValley;
using StardewValley.Constants;

namespace SummerFleursBetterHats.HatWithConditions;

public static partial class HatWithConditions
{
    private const string SantaHatID = "(H)25";

    /// <summary>
    /// Extra condition for Santa Hat: wear it at the beginning of the day.
    /// </summary>
    private static bool CheckConditionForSantaHat() =>
        Utilities.PlayerHatIs(SantaHatID) &&
        Game1.currentSeason == "winter" &&
        Game1.dayOfMonth == 25;

    /// <summary>
    /// Effect of Santa Hat: if the player has forage mastery, get a
    /// Golden Mystery Box as santa's gift, otherwise, a Mystery Box.
    /// </summary>
    private static void SantaHat_AddMysteryBox()
    {
        var player = Game1.player;
        var gift = player.stats.Get(StatKeys.Mastery(2)) != 0 ? "(O)GoldenMysteryBox" : "(O)MysteryBox";
        Game1.createObjectDebris(gift, (int)player.Tile.X, (int)player.Tile.Y, player.UniqueMultiplayerID);
        Game1.showGlobalMessage("You got a gift from Santa! Happy Feast Of The Winter Star!"); // TODO: i18n
    }
}