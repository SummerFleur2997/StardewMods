namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    private const ushort BluebirdMaskMask = 8; // 0b1000

    /// <summary>
    /// Item roller for the bluebird mask. See the detailed item
    /// list in the README.
    /// </summary>
    private static Item RollRandomItemForBluebirdMask()
    {
        var week = Game1.stats.DaysPlayed / 7;
        var r = Utility.CreateRandom(
            week,
            Game1.uniqueIDForThisGame,
            Game1.player.UniqueMultiplayerID,
            80); // id of the bluebird mask

        return r.NextDouble() switch
        {
            < 0.30 => ItemRegistry.Create("(O)709", 20), // hardwood
            < 0.55 => ItemRegistry.Create("(O)831", 10), // taro tuber
            < 0.80 => ItemRegistry.Create("(O)833", 10), // pineapple seed
            < 0.90 => ItemRegistry.Create("(O)791"), // golden coconut
            < 0.95 => ItemRegistry.Create("(O)835"), // mango sapling
            _ => ItemRegistry.Create("(O)69") // banana sapling
        };
    }
}