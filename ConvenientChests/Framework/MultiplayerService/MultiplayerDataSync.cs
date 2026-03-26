#nullable disable
using ConvenientChests.Framework.DataStructs;

namespace ConvenientChests.Framework.MultiplayerService;

[Serializable]
internal class MultiplayerChestSync
{
    public ChestAddress ChestAddress { get; set; }
    public string ItemKey { get; set; }
    public long SenderID { get; set; }

    public MultiplayerChestSync() { }

    public MultiplayerChestSync(ChestAddress chestAddress, string itemKey, long senderID)
    {
        ChestAddress = chestAddress;
        ItemKey = itemKey;
        SenderID = senderID;
    }
}