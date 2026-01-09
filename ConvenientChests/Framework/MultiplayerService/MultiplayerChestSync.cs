using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.ItemService;

namespace ConvenientChests.Framework.MultiplayerService;

[Serializable]
internal class MultiplayerChestSync
{
    public ChestAddress ChestAddress { get; set; }
    public ItemKey ItemKey { get; set; }
    public long SenderID { get; set; }

    public MultiplayerChestSync(ChestAddress chestAddress, ItemKey itemKey, long senderID)
    {
        ChestAddress = chestAddress;
        ItemKey = itemKey;
        SenderID = senderID;
    }
}