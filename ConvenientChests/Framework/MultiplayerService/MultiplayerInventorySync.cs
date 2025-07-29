using System;
using ConvenientChests.Framework.ItemService;

namespace ConvenientChests.Framework.MultiplayerService;

[Serializable]
internal class MultiplayerInventorySync
{
    public long PlayerID { get; set; }
    public ItemKey ItemKey { get; set; }
    public long SenderID { get; set; }

    public MultiplayerInventorySync(long playerID, ItemKey itemKey)
    {
        PlayerID = playerID;
        ItemKey = itemKey;
        SenderID = playerID;
    }
}