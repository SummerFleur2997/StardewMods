using StardewValley;

namespace SummerFleursBetterHats.HatWithConditions;

public static partial class HatWithConditions
{
    private const string SouwesterID = "(H)28";

    private static bool CheckConditionForSouwester() =>
        Utilities.PlayerHatIs(SouwesterID) &&
        Game1.currentLocation.IsOutdoors &&
        Game1.currentLocation.IsRainingHere();
}