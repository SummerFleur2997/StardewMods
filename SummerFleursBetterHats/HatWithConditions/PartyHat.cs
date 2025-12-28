using StardewValley;
using StardewValley.Locations;

namespace SummerFleursBetterHats.HatWithConditions;

public static partial class HatWithConditions
{
    private const string PartyHatRedID = "(H)57";
    private const string PartyHatBlueID = "(H)58";
    private const string PartyHatGreenID = "(H)59";

    private static bool CheckConditionForPartyHat()
    {
        var currentLocation = Game1.currentLocation;
        if (currentLocation is not MineShaft { mineLevel: not 77377 } mineShaft) return false;
        return mineShaft.mineLevel switch
        {
            >= 1 and < 40 when Utilities.PlayerHatIs(PartyHatGreenID) => true,
            >= 40 and < 80 when Utilities.PlayerHatIs(PartyHatBlueID) => true,
            >= 80 when Utilities.PlayerHatIs(PartyHatRedID) => true,
            _ => false
        };
    }
}