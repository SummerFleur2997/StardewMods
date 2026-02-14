using StardewValley.Extensions;
using StardewValley.Objects;

namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    private static List<string> _availableHats = new();

    private static Item RollRandomItemForMouseEars()
    {
        var r = Utility.CreateRandom(
            Game1.stats.DaysPlayed,
            Game1.uniqueIDForThisGame,
            Game1.player.UniqueMultiplayerID,
            31); // id of the mouse ears

        var k = r.Next() & 15;
        for (var j = 0; j < k; j++)
        for (var i = 0; i < j; i++)
            r.Next();

        return ItemRegistry.Create<Hat>(r.ChooseFrom(_availableHats));
    }
}