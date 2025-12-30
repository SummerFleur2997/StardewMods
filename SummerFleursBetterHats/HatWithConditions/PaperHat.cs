using StardewValley;
using StardewValley.Locations;

namespace SummerFleursBetterHats.HatWithConditions;

public static partial class HatWithConditions
{
    private const string PaperHatID = "(H)PaperHat";

    private static bool CheckConditionForPaperHat() =>
        Utilities.PlayerHatIs(PaperHatID) &&
        Game1.currentLocation is IslandLocation or IslandFarmHouse;
}