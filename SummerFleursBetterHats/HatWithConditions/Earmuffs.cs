using StardewValley;

namespace SummerFleursBetterHats.HatWithConditions;

public static partial class HatWithConditions
{
    private const string EarmuffsID = "(H)11";

    /// <summary>
    /// Extra conditions for Earmuffs: current season is winter.
    /// </summary>
    private static bool CheckConditionForEarmuffs()
        => Utilities.PlayerHatIs(EarmuffsID) && Game1.currentSeason == "winter";
}