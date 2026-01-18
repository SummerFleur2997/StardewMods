namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    /// <summary>
    /// Item roller for the wearable dwarf helm.
    /// </summary>
    private static Item RollRandomItemForWearableDwarfHelm()
    {
        var week = Game1.stats.DaysPlayed / 7;
        var r = Utility.CreateRandom(
            week,
            Game1.uniqueIDForThisGame,
            Game1.player.UniqueMultiplayerID,
            46); // id of the wearable dwarf helm

        return r.NextDouble() switch
        {
            < 0.20 => ItemRegistry.Create("(O)286", 20), // cherry bomb
            < 0.40 => ItemRegistry.Create("(O)287", 10), // bomb
            < 0.60 => ItemRegistry.Create("(O)288", 5), // mega bomb
            < 0.66 => ItemRegistry.Create("(O)66", 2), // amethyst
            < 0.72 => ItemRegistry.Create("(O)68", 2), // topaz
            < 0.78 => ItemRegistry.Create("(O)62", 2), // aquamarine
            < 0.84 => ItemRegistry.Create("(O)70", 2), // jade
            < 0.90 => ItemRegistry.Create("(O)60", 2), // emerald
            < 0.96 => ItemRegistry.Create("(O)64", 2), // ruby
            < 0.99 => ItemRegistry.Create("(O)72"), // diamond
            _ => ItemRegistry.Create("(O)74") // prismatic shard
        };
    }
}