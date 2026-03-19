using Newtonsoft.Json;

namespace ConvenientChests.Framework.DataStructs;

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
    [JsonConverter(typeof(DataConverter))]
    public HashSet<ItemKey> LockedItems { get; set; }

    public InventoryEntry() { }

    public InventoryEntry(InventoryData data, string playerName, long playerID)
    {
        PlayerName = playerName;
        PlayerID = playerID;
        LockedItems = data.LockedItemKinds;
    }

    public InventoryEntry(HashSet<ItemKey> itemKeys, Farmer player)
    {
        PlayerName = player.Name;
        PlayerID = player.UniqueMultiplayerID;
        LockedItems = itemKeys;
    }
}