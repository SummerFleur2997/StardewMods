using Common;
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.SaveService;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace ConvenientChests.Framework.MultiplayerService;

internal class MultiplayerServer : IModule
{
    /// <summary>
    /// Static singleton.
    /// </summary>
    public static readonly MultiplayerServer Instance = new();

    /// <inheritdoc />
    public bool IsActive { get; private set; }

    /// <summary>
    /// An easy access to <see cref="IMultiplayerHelper"/>
    /// </summary>
    private static IMultiplayerHelper MultiplayerHelper => ModEntry.ModHelper.Multiplayer;

    private MultiplayerServer() { }

    /// <inheritdoc />
    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;

        if (!Context.IsMainPlayer)
            ModEntry.ModHelper.Events.Display.MenuChanged += OnMenuChanged;
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived -= OnMessageReceived;
        ModEntry.ModHelper.Events.Display.MenuChanged -= OnMenuChanged;
    }

    /// <summary>
    /// When someone edit the chest data, send it to every one.
    /// </summary>
    /// <param name="chest">Which chest was modified.</param>
    /// <param name="itemKey">The itemkey of changed accept info.</param>
    /// <param name="alias">The new alias.</param>
    /// <param name="itemId">The qualified item id of new item icon.</param>
    public static void SendChestData(Chest chest, ItemKey? itemKey = null, string? alias = null, string? itemId = null)
    {
        var playerID = Game1.player.UniqueMultiplayerID;
        var chestAddress = new ChestAddress(chest);
        var syncData = new MultiplayerChestSync(chestAddress, itemKey, alias, itemId, playerID);

        MultiplayerHelper.SendMessage(syncData, "MultiplayerChestSync",
            new[] { ModEntry.Manifest.UniqueID });
    }

    /// <summary>
    /// Receive the chest data from other players when a chest is modified.
    /// </summary>
    /// <param name="multiplayerChestSyncChest">Wrapped data.</param>
    private static void ReceiveChestData(MultiplayerChestSync multiplayerChestSyncChest)
    {
        if (multiplayerChestSyncChest.SenderID == Game1.player.UniqueMultiplayerID) return;
        var chestAddress = multiplayerChestSyncChest.ChestAddress;
        var itemKey = multiplayerChestSyncChest.ItemKey;
        var alias = multiplayerChestSyncChest.Alias;
        var item = ItemRegistry.Create(multiplayerChestSyncChest.ItemId);

        ChestManager.ModifyChest(chestAddress, itemKey, alias, item);
    }

    /// <summary>
    /// When someone connected and request a save data, send SaveData to them.
    /// Only used for the host.
    /// </summary>
    /// <param name="toPlayerID">The player id who requested the save data.</param>
    private static void SendSaveData(long toPlayerID)
    {
        var sentSaveData = Saver.GetSerializableData();
        MultiplayerHelper.SendMessage(sentSaveData, "MultiplayerInit",
            new[] { ModEntry.Manifest.UniqueID }, new[] { toPlayerID });
    }

    /// <summary>
    /// Receive the save data from the host.
    /// </summary>
    /// <param name="saveData">The received save data.</param>
    private static void ReceiveSaveData(SaveData saveData)
    {
        SaveManager.LoadSaveData(saveData);
        ModEntry.ModHelper.Events.Display.MenuChanged -= OnMenuChanged;
    }

    /// <summary>
    /// Request the save data from the host when open a menu.
    /// </summary>
    private static void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        MultiplayerHelper.SendMessage("null", "MultiplayerInit",
            new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
    }

    /// <summary>
    /// Handle messages.
    /// </summary>
    private static void OnMessageReceived(object? sender, ModMessageReceivedEventArgs e)
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
        }
    }
}