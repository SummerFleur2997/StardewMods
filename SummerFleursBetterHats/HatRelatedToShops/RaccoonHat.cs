namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    /// <summary>
    /// Item roller for the raccoon hat.
    /// </summary>
    private static Item RollRandomItemForRaccoonHat()
    {
        var r = Utility.CreateRandom(
            Game1.stats.DaysPlayed,
            Game1.uniqueIDForThisGame,
            Game1.player.UniqueMultiplayerID,
            106); // index of the raccoon hat

        return r.NextDouble() switch
        {
            < 0.40 => Utility.getRaccoonSeedForCurrentTimeOfYear(Game1.player, r),
            < 0.50 => ItemRegistry.Create("(O)Moss", 5), // moss
            < 0.60 => ItemRegistry.Create("(O)770", 5), // mixed seeds
            < 0.70 => ItemRegistry.Create("(O)309", 2), // acorn
            < 0.80 => ItemRegistry.Create("(O)310", 2), // maple seed
            < 0.90 => ItemRegistry.Create("(O)311", 2), // pine cone
            < 0.98 => ItemRegistry.Create("(O)292", 2), // mahogany seed
            < 0.995 => ItemRegistry.Create("(O)872"), //fairy dust
            _ => ItemRegistry.Create("(O)279") // magic rock candy
        };
    }
}