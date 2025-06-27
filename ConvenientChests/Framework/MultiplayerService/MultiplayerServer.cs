using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.ItemService;
using ConvenientChests.Framework.SaveService;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ConvenientChests.Framework.MultiplayerService;

internal class MultiplayerServer : IModule
{
    public bool IsActive { get; private set; }

    private static IMultiplayerHelper MultiplayerHelper => ModEntry.ModHelper.Multiplayer;

    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;

        if (!Context.IsMainPlayer)
            ModEntry.ModHelper.Events.Display.MenuChanged += OnMenuChanged;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived -= OnMessageReceived;
    }

    public static void SendChestData(Chest chest, ItemKey itemKey)
    {
        var playerID = Game1.player.UniqueMultiplayerID;
        var chestAddress = new ChestAddress(chest);
        var syncData = new MultiplayerChestSync(chestAddress, itemKey, playerID);

        MultiplayerHelper.SendMessage(syncData, "MultiplayerChestSync",
            new[] { ModEntry.Manifest.UniqueID });
    }

    public static void SendInventoryData(FarmHouse playerHouse, ItemKey itemKey)
    {
        var playerID = playerHouse.owner.UniqueMultiplayerID;
        var syncData = new MultiplayerInventorySync(playerID, itemKey);

        MultiplayerHelper.SendMessage(syncData, "MultiplayerInventorySync",
            new[] { ModEntry.Manifest.UniqueID });
    }

    private static void ReceiveChestData(MultiplayerChestSync multiplayerChestSyncData)
    {
        if (multiplayerChestSyncData.SenderID == Game1.player.UniqueMultiplayerID) return;
        var chestAddress = multiplayerChestSyncData.ChestAddress;
        var itemKey = multiplayerChestSyncData.ItemKey;

        ChestManager.ModifyChest(chestAddress, itemKey);
    }

    private static void ReceiveInventoryData(MultiplayerInventorySync multiplayerInventorySyncData)
    {
        if (multiplayerInventorySyncData.SenderID == Game1.player.UniqueMultiplayerID) return;
        var playerID = multiplayerInventorySyncData.PlayerID;
        var itemKey = multiplayerInventorySyncData.ItemKey;

        InventoryManager.ModifyInventory(playerID, itemKey);
    }

    private static void SendSaveData(long toPlayerID)
    {
        var sentSaveData = Saver.GetSerializableData();
        MultiplayerHelper.SendMessage(sentSaveData, "MultiplayerInit",
            new[] { ModEntry.Manifest.UniqueID }, new[] { toPlayerID });
    }

    private static void ReceiveSaveData(SaveData saveData)
    {
        SaveManager.LoadSaveData(saveData);
        ModEntry.ModHelper.Events.Display.MenuChanged -= OnMenuChanged;
    }

    private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        MultiplayerHelper.SendMessage("null", "MultiplayerInit",
            new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
    }

    private static void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        switch (e.Type)
        {
            // When receiving the "MultiplayerInit" message from the host, sync SaveData.
            // 收到来自房主的 MultiplayerInit 消息后，同步 SaveData
            case "MultiplayerInit" when e.FromPlayerID == Game1.MasterPlayer.UniqueMultiplayerID:
                var saveData = e.ReadAs<SaveData>();
                ModEntry.Log($"Received save data from {e.FromPlayerID}.", LogLevel.Info);
                ReceiveSaveData(saveData);
                break;
            // When the host receives the "MultiplayerInit" message from other players, send SaveData.
            // 房主收到其他玩家的 MultiplayerInit 消息后，发送 SaveData
            case "MultiplayerInit" when Context.IsMainPlayer:
                ModEntry.Log($"Received sync request from {e.FromPlayerID}.", LogLevel.Info);
                SendSaveData(e.FromPlayerID);
                break;
            // When receiving the "MultiplayerChestSync" message from other players, sync ChestData.
            // 收到其他玩家的 MultiplayerChestSync 消息后，同步 ChestData
            case "MultiplayerChestSync":
                var syncChestData = e.ReadAs<MultiplayerChestSync>();
                ModEntry.Log($"Received chest sync data from {e.FromPlayerID}.");
                ReceiveChestData(syncChestData);
                break;
            // When receiving the "MultiplayerInventorySync" message from other players, sync InventoryData.
            // 收到其他玩家的 MultiplayerInventorySync 消息后，同步 InventoryData
            case "MultiplayerInventorySync":
                var syncInventoryData = e.ReadAs<MultiplayerInventorySync>();
                ModEntry.Log($"Received inventory sync data from {e.FromPlayerID}.");
                ReceiveInventoryData(syncInventoryData);
                break;
        }
    }
}