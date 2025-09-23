using BetterRetainingSoils.DirtService;
using Common;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Framework.MultiplayerService;

internal class MultiplayerServer : IModule
{
    public object Lock = new();
    public bool IsActive { get; private set; }

    private static IMultiplayerHelper MultiplayerHelper => ModEntry.ModHelper.Multiplayer;

    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived -= OnMessageReceived;
    }

    /// <summary>
    /// 当其它玩家浇水时，向主机请求更新地块数据。
    /// When a farmhand watering a crop, tell the host to update hoedirt data.
    /// </summary>
    /// <param name="dirt">其它玩家正在浇水的地块。The tile where the farmhand is watering.</param>
    public static void SendDirtData(HoeDirt dirt)
    {
        var data = new SyncData(Game1.currentLocation.Name, (int)dirt.Tile.X, (int)dirt.Tile.Y);
        MultiplayerHelper.SendMessage(data, "MultiplayerWateringSync",
            new[] {ModEntry.Manifest.UniqueID}, 
            new[] {Game1.MasterPlayer.UniqueMultiplayerID});
    }

    private static void ReceiveDirtData(SyncData data)
    {
        var tile = new Vector2(data.X, data.Y);
        // Get location from Syncdata
        // 从 Syncdata 处获取地点信息
        var location = Game1.getLocationFromName(data.LocationName);
        if (location == null)
        {
            ModEntry.Log("Invalid location!", LogLevel.Warn);
            return;
        }

        // Get hoedirt from coordinates
        // 从坐标处获取耕地
        if (location.terrainFeatures.TryGetValue(tile, out var feature))
        {
            if (feature is HoeDirt hoeDirt && hoeDirt.IsAvailable())
            {
                hoeDirt.GetHoeDirtData().RefreshStatus();
            }
            else
            {
                ModEntry.Log($"Terrain at ({tile.X},{tile.Y}) is not HoeDirt.", LogLevel.Warn);
            }
        }
        else
        {
            ModEntry.Log($"No terrain feature found at ({tile.X},{tile.Y}) in {location.Name}!", LogLevel.Warn);
        }
    }

    /// <summary>
    /// When the host receives the "MultiplayerWateringSync" message from other players.
    /// 房主收到其他玩家的 MultiplayerWateringSync 消息后。
    /// </summary>
    private static void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.Type != "MultiplayerWateringSync") return;

        var data = e.ReadAs<SyncData>();
        ModEntry.Log($"Received watering data from {e.FromPlayerID}.", LogLevel.Info);
        ReceiveDirtData(data);
    }
}