using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ItemService;

namespace ConvenientChests.Framework.InventoryService;

internal class InventoryEntry
{
    /// <summary>
    /// 当前玩家的名字。
    /// The current player's name.
    /// </summary>
    public string PlayerName { get; set; }
    
    /// <summary>
    /// 当前玩家的 ID。
    /// The current player's ID.
    /// </summary>
    public long PlayerID { get; set; }

    public Dictionary<ItemType, string> LockedItems { get; set; }

    public InventoryEntry() { }

    public InventoryEntry(InventoryData data)
    {
        PlayerName = data.Player.Name;
        PlayerID = data.Player.UniqueMultiplayerID;
        LockedItems = data.LockedItemKinds
            .GroupBy(i => ItemHelper.GetItemType(i))
            .ToDictionary(
                g => g.Key,
                g => string.Join(",", g.Select(i => i.ItemId))
            );
    }

    public HashSet<ItemKey> GetItemSet()
    {
        return LockedItems
            .Select(e => new
            {
                Type = e.Key,
                ItemIDs = e.Value.Split(',')
            })
            .SelectMany(e => e.ItemIDs.Select(itemId => new ItemKey(e.Type.GetTypeDefinition(), itemId)))
            .ToHashSet();
    }
}