namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    private const ushort DarkVelvetBowMask = 1; // 0b0001

    /// <summary>
    /// Item roller for the dark velvet bow. See the detailed item
    /// list in the README.
    /// </summary>
    private static Item RollRandomItemForDarkVelvetBow()
    {
        var week = Game1.stats.DaysPlayed / 7;
        var r = Utility.CreateRandom(
            week,
            Game1.uniqueIDForThisGame,
            Game1.player.UniqueMultiplayerID,
            100); // id of the dark velvet bow

        return r.NextDouble() switch
        {
            < 0.30 => ItemRegistry.Create($"(O){r.Next(194, 244)}", 5), // random dish
            < 0.55 => ItemRegistry.Create("(O)770", 20), // mixed seeds
            < 0.76 => ItemRegistry.Create("(O)768", 10), // solar essence
            < 0.97 => ItemRegistry.Create("(O)769", 10), // void essence
            _ => ItemRegistry.Create("(O)645") // iridium sprinkler
        };
    }
}