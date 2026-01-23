namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    /// <summary>
    /// Item roller for the magic turban.
    /// </summary>
    private static Item RollRandomItemForMrQisHat()
    {
        var month = (Game1.Date.Year - 1) * 4 + Game1.Date.SeasonIndex;
        var r = Utility.CreateRandom(
            month,
            Game1.uniqueIDForThisGame,
            Game1.player.UniqueMultiplayerID,
            82); // id of the mr. qi's hat

        return r.NextDouble() switch
        {
            < 0.22 => ItemRegistry.Create("(O)917", 20), // qi seasoning
            < 0.44 => ItemRegistry.Create("(O)918", 30), // hyper speed-gro
            < 0.66 => ItemRegistry.Create("(O)919", 30), // deluxe fertilizer
            < 0.88 => ItemRegistry.Create("(O)920", 30), // deluxe retaining soil
            < 0.96 => ItemRegistry.Create("(O)910", 2),  // radioactive bar
            _ => ItemRegistry.Create("(O)896") // galaxy soul
        };
    }
}