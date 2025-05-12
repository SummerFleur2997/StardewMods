using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ItemService;

namespace ConvenientChests.Framework.InventoryService;

[Serializable]
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

    /// <summary>
    /// ID 为 <see cref="PlayerID"/> 的玩家背包被锁定物品的 <see cref="ItemKey"/> 序列。
    /// The sequence of <see cref="ItemKey"/> that were configured to be locked
    /// in the backpack of the player with ID <see cref="PlayerID"/>.
    /// </summary>
    public Dictionary<ItemType, string> LockedItems { get; set; }

    public InventoryEntry() { }

    public InventoryEntry(InventoryData data, string playerName, long playerID)
    {
        PlayerName = playerName;
        PlayerID = playerID;
        LockedItems = data.LockedItemKinds
            .GroupBy(ItemHelper.GetItemType)
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