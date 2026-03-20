using ConvenientChests.Framework.DataStructs;

namespace ConvenientChests.Framework.MultiplayerService;

[Serializable]
internal class MultiplayerChestSync
{
    public ChestAddress ChestAddress { get; set; }
    public ItemKey ItemKey { get; set; }
    public string Alias { get; set; }
    public string ItemId { get; set; }
    public long SenderID { get; set; }

    public MultiplayerChestSync() { }

    public MultiplayerChestSync(ChestAddress chestAddress, ItemKey itemKey, string alias, string itemId, long senderID)
    {
        ChestAddress = chestAddress;
        ItemKey = itemKey;
        Alias = alias;
        ItemId = itemId;
        SenderID = senderID;
    }
}

[Serializable]
internal class MultiplayerInventorySync
{
    public long PlayerID { get; set; }
    public ItemKey ItemKey { get; set; }
    public long SenderID { get; set; }

    public MultiplayerInventorySync() { }

    public MultiplayerInventorySync(long playerID, ItemKey itemKey)
    {
        PlayerID = playerID;
        ItemKey = itemKey;
        SenderID = playerID;
    }
}