using StardewModdingAPI.Events;
using StardewValley.Locations;
using SummerFleursBetterHats.Framework;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public partial class HatRelyOnEvents
{
    private const int ExtraEggScore = 10;

    /// <summary>
    /// When entering the skull cavern with gil's hat worn
    /// during the desert festival, get 10 egg score.
    /// </summary>
    private static void GilsHatLocationChanged(object s, WarpedEventArgs e)
    {
        // Ensure the new location is skull cavern
        if (e.NewLocation is not MineShaft { mineLevel: 121 })
            return;

        // Check whether gil's hat is worn
        if (!PlayerHatIs(GilsHatID))
        {
            ModEvents.Player.Warped -= GilsHatLocationChanged;
            return;
        }

        // Check whether it is the desert festival, and 
        // whether the effect has been triggered today
        var player = Game1.player;
        var desertFestivalDate = Utility.GetDayOfPassiveFestival("DesertFestival");
        if (desertFestivalDate <= 0 || player.TryGetWorldStatus(GilsHatMask))
            return;

        player.team.calicoEggSkullCavernRating.Value += ExtraEggScore;
        SaveManager.TryEditWorldStatus(player.UniqueMultiplayerID, GilsHatMask);
    }
}