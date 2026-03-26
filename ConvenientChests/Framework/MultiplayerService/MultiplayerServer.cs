using Common;
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
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
    /// <param name="itemKey">The changed accepted item.</param>
    public static void SendChestData(Chest chest, string? itemKey = null)
    {
        var playerID = Game1.player.UniqueMultiplayerID;
        var chestAddress = new ChestAddress(chest);
        var syncData = new MultiplayerChestSync(chestAddress, itemKey, playerID);

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

        ChestManager.ModifyChest(chestAddress, itemKey);
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