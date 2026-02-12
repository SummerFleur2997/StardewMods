namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    /// <summary>
    /// Item roller for the green turban.
    /// </summary>
    private static Item RollRandomItemForGreenTurban()
    {
        var week = Game1.Date.TotalSundayWeeks;
        var r = Utility.CreateRandom(
            week,
            Game1.uniqueIDForThisGame,
            Game1.player.UniqueMultiplayerID,
            72); // id of the green turban

        var k = r.Next() & 15;
        for (var j = 0; j < k; j++)
        for (var i = 0; i < j; i++)
            r.Next();

        return r.NextDouble() switch
        {
            < 0.11 => ItemRegistry.Create("(O)275", 10), // artifact trove
            < 0.22 => ItemRegistry.Create("(O)261", 10), // warp totem: desert
            < 0.33 => ItemRegistry.Create("(O)88", 10), // coconut
            < 0.44 => ItemRegistry.Create("(O)90", 10), // cactus fruit
            < 0.55 => ItemRegistry.Create("(O)287", 10), // bomb
            < 0.66 => ItemRegistry.Create("(O)288", 5), // mega bomb
            < 0.77 => ItemRegistry.Create("(O)226", 5), // spicy eel
            < 0.88 => ItemRegistry.Create("(BC)71", 5), // ladder
            < 0.99 => ItemRegistry.Create("(O)428", 3), // cloth
            _ => ItemRegistry.Create("(O)279") // magic rock candy
        };
    }
}