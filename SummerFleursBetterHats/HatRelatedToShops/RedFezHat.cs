using StardewValley.ItemTypeDefinitions;

namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    /// <summary>
    /// Item roller for the red fez hat.
    /// </summary>
    private static Item RollRandomItemForRedFezHat()
    {
        var r = Utility.CreateRandom(
            Game1.stats.DaysPlayed,
            Game1.uniqueIDForThisGame,
            Game1.player.UniqueMultiplayerID,
            105); // index of the red fez hat

        var k = r.Next() & 15;
        for (var j = 0; j < k; j++)
        for (var i = 0; i < j; i++)
            r.Next();

        string id;
        Item item;
        do
        {
            id = "(O)" + r.Next(16, 788);
            var amount = r.NextDouble() > 0.75 ? 5 : 1;
            item = ItemRegistry.Create(id, amount);
        } while (!ItemRegistry.GetDataOrErrorItem(id).IsLegitimate());

        return item;
    }

    private static bool IsLegitimate(this ParsedItemData data) =>
        !data.IsErrorItem && !data.ExcludeFromRandomSale &&
        data.Category is not (-999 or -24 or -20 or 0) &&
        data.ObjectType is not ("Quest" or "Minerals" or "Arch" or "Ring");
}