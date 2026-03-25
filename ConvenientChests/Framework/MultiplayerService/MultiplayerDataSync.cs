#nullable disable
using ConvenientChests.Framework.DataStructs;

namespace ConvenientChests.Framework.MultiplayerService;

[Serializable]
internal class MultiplayerChestSync
{
    public ChestAddress ChestAddress { get; set; }
    public string ItemKey { get; set; }
    public string Alias { get; set; }
    public string ItemId { get; set; }
    public long SenderID { get; set; }

    public MultiplayerChestSync() { }

    public MultiplayerChestSync(ChestAddress chestAddress, string itemKey, string alias, string itemId, long senderID)
    {
        ChestAddress = chestAddress;
        ItemKey = itemKey;
        Alias = alias;
        ItemId = itemId;
        SenderID = senderID;
    }
}