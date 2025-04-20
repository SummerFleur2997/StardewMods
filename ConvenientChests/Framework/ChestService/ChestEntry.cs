using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ItemService;

namespace ConvenientChests.Framework.ChestService;

/// <summary>
/// A piece of saved data describing the location of a chest and what items
/// the chest at that location has been assigned to.
/// </summary>
internal class ChestEntry
{
    /// <summary>
    /// 箱子在存档中的位置。
    /// The chest's location in the world.
    /// </summary>
    public ChestAddress Address { get; set; }

    /// <summary>
    /// 位于 <see cref="Address"/> 处的箱子能够接受的 <see cref="ItemKey"/> 序列。
    /// The set of <see cref="ItemKey"/> that were configured to be accepted
    /// by the chest at <see cref="Address"/> .
    /// </summary>
    public Dictionary<ItemType, string> AcceptedItems { get; set; }

    public ChestEntry()
    {
    }

    public ChestEntry(ChestData data, ChestAddress address)
    {
        Address = address;
        AcceptedItems = data.AcceptedItemKinds
            .GroupBy(i => ItemHelper.GetItemType(i))
            .ToDictionary(
                g => g.Key,
                g => string.Join(",", g.Select(i => i.ItemId))
            );
    }


    public HashSet<ItemKey> GetItemSet()
    {
        return AcceptedItems
            .Select(e => new
            {
                Type = e.Key,
                ItemIDs = e.Value.Split(',')
            })
            .SelectMany(e => e.ItemIDs.Select(itemId => new ItemKey(e.Type.GetTypeDefinition(), itemId)))
            .ToHashSet();
    }
}