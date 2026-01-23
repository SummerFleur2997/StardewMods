namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    /// <summary>
    /// Item roller for the dark velvet bow.
    /// </summary>
    private static Item RollRandomItemForDarkVelvetBow()
    {
        var week = Game1.Date.TotalSundayWeeks;
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