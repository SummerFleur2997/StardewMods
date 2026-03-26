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
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived -= OnMessageReceived;
    }

    /// <summary>
    /// When someone edit the chest data, send it to every one.
    /// </summary>
    /// <param name="chest">Which chest was modified.</param>
    /// <param name="attr">Which attribute need to update, 0 = AcceptItems, 1 = Alias.</param>
    public static void SendChestUpdateReq(Chest chest, int attr)
    {
        var chestAddress = new ChestAddress(chest);

        MultiplayerHelper.SendMessage(chestAddress, $"MultiplayerChestSync_{attr}", 
            new[] { ModEntry.Manifest.UniqueID });
    }

    /// <summary>
    /// Receive the chest data from other players when a chest is modified.
    /// </summary>
    /// <param name="chestAddress">The address of which chest was modified.</param>
    /// <param name="attr">Which attribute need to update, 0 = AcceptItems, 1 = Alias.</param>
    private static void ReceiveChestData(ChestAddress chestAddress, int attr)
    {
        ChestManager.UpdateChest(chestAddress, attr);
    }

    /// <summary>
    /// Handle messages.
    /// </summary>
    private static void OnMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        var syncChestData = e.ReadAs<ChestAddress>();
        switch (e.Type)
        {
            // When receiving the "MultiplayerChestSync_0" message from other players, sync ChestData.AcceptItems
            // 收到其他玩家的 MultiplayerChestSync_0 消息后，同步 ChestData.AcceptItems
            case "MultiplayerChestSync_0":
                ModEntry.Log($"Received chest sync request from {e.FromPlayerID}.");
                ReceiveChestData(syncChestData, 0);
                break;
            // When receiving the "MultiplayerChestSync_1" message from other players, sync ChestData.Alias
            // 收到其他玩家的 MultiplayerChestSync_1 消息后，同步 ChestData.Alias
            case "MultiplayerChestSync_1":
                ModEntry.Log($"Received chest sync request from {e.FromPlayerID}.");
                ReceiveChestData(syncChestData, 1);
                break;
        }
    }
}