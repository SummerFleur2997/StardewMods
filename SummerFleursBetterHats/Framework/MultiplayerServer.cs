using Common;
using StardewModdingAPI.Events;

namespace SummerFleursBetterHats.Framework;

public class MultiplayerServer : IModule
{
    public static readonly MultiplayerServer Instance = new();

    public bool IsActive { get; private set; }

    private IMultiplayerHelper MultiplayerHelper => ModEntry.ModHelper.Multiplayer;

    private MultiplayerServer() { }

    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;
        ModEntry.ModHelper.Events.GameLoop.DayStarted += OnDayStarted;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived -= OnMessageReceived;
        ModEntry.ModHelper.Events.GameLoop.DayStarted -= OnDayStarted;
    }

    public static void RegisterEvents()
    {
        ModEntry.ModHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        ModEntry.ModHelper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
    }

    /// <summary>
    /// For farmhands: send a request to edit data when the world status changed.
    /// </summary>
    public void SendEditRequest(ushort newData) =>
        MultiplayerHelper.SendMessage(newData, "MultiplayerDataEdit",
            new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });

    /// <summary>
    /// For farmhands: send a request to the host to sync SaveData.
    /// </summary>
    private void SendSyncRequest() =>
        MultiplayerHelper.SendMessage("null", "MultiplayerInit",
            new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });

    /// <summary>
    /// For host: send SaveData to the farmhand who requested it.
    /// </summary>
    private void SendSyncData(long fromWho) =>
        MultiplayerHelper.SendMessage(SaveManager.WorldStatus, "MultiplayerInit",
            new[] { ModEntry.Manifest.UniqueID }, new[] { fromWho });

    private void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        if (!Context.IsMainPlayer)
            SendSyncRequest();

        ModEntry.ModHelper.Events.GameLoop.DayStarted -= OnDayStarted;
    }

    private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        switch (e.Type)
        {
            // When receiving the "MultiplayerInit" message from the host, sync WorldStatus.
            // 收到来自房主的 MultiplayerInit 消息后，同步 SaveData
            case "MultiplayerInit" when e.FromPlayerID == Game1.MasterPlayer.UniqueMultiplayerID:
                var worldStatus = e.ReadAs<Dictionary<long, ushort>>();
                ModEntry.Log($"Received save data from {e.FromPlayerID}.", LogLevel.Info);
                SaveManager.WorldStatus = worldStatus;
                break;
            // When the host receives the "MultiplayerInit" message from other players, send SaveData.
            // 房主收到其他玩家的 MultiplayerInit 消息后，发送 SaveData
            case "MultiplayerInit" when Context.IsMainPlayer:
                ModEntry.Log($"Received sync request from {e.FromPlayerID}.", LogLevel.Info);
                SendSyncData(e.FromPlayerID);
                break;
            // When receiving the "MultiplayerDataEdit" message from other players, edit WorldStatus.
            // 收到其他玩家的 MultiplayerDataEdit 消息后，修正 WorldStatus
            case "MultiplayerDataEdit":
                var mask = e.ReadAs<ushort>();
                SaveManager.TryEditWorldStatus(e.FromPlayerID, mask);
                break;
        }
    }

    private static void OnSaveLoaded(object s, SaveLoadedEventArgs e)
    {
        if (Context.IsMultiplayer)
            Instance.Activate();
    }

    private static void OnReturnToTitle(object s, ReturnedToTitleEventArgs e)
    {
        if (Context.IsMultiplayer)
            Instance.Deactivate();
    }
}