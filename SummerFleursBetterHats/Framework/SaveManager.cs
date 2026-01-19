using System.IO;
using Common.ExceptionService;
using StardewModdingAPI.Events;

namespace SummerFleursBetterHats.Framework;

internal static class SaveManager
{
    /// <summary>
    /// 存档数据的存储路径，位于 mod 文件夹下的 savedata 文件夹中。
    /// The path to the mod's save data file, relative to the mod folder.
    /// </summary>
    private static string SavePath => Path.Combine("savedata", $"{Constants.SaveFolderName}.json");

    /// <summary>
    /// 存档数据的绝对路径。
    /// The absolute path to the mod's save data file.
    /// </summary>
    private static string AbsoluteSavePath => Path.Combine(ModEntry.ModHelper.DirectoryPath, SavePath);

    /// <summary>
    /// Dictionary to store various status with player IDs as keys
    /// and uint values as flags.
    /// </summary>
    internal static Dictionary<long, uint> WorldStatus { get; set; }

    public static void RegisterEvents()
    {
        ModEntry.ModHelper.Events.GameLoop.DayStarted += OnDayStarted;
        ModEntry.ModHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        ModEntry.ModHelper.Events.GameLoop.Saving += OnSaving;
    }

    /// <summary>
    /// Extension method to attempt to modify world status for a player.
    /// </summary>
    public static bool TryEditWorldStatus(long who, uint mask)
    {
        // Try to get existing world status for the player
        if (!WorldStatus.TryGetValue(who, out var tradeInfo))
            return false;

        // Update the world status using bitwise OR operation with the mask
        WorldStatus[who] = tradeInfo | mask;
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            MultiplayerServer.Instance.SendEditRequest(mask);

        return true;
    }

    /// <summary>
    /// Extension method to check specific world status for a player.
    /// </summary>
    public static bool TryGetWorldStatus(this Farmer player, uint mask)
    {
        var id = player.UniqueMultiplayerID;
        // use the binary mask to check specific world status
        if (WorldStatus.TryGetValue(id, out var data))
            return (data & mask) != 0;

        // if there are no data for the player, add a new entry
        WorldStatus[id] = 0;
        return false;
    }

    /// <summary>
    /// Load save data from the save path.
    /// </summary>
    private static void Load()
    {
        if (!Context.IsMainPlayer) return;
        if (!File.Exists(AbsoluteSavePath))
        {
            WorldStatus = new Dictionary<long, uint>();
            return;
        }

        try
        {
            WorldStatus = ModEntry.ModHelper.Data.ReadJsonFile<Dictionary<long, uint>>(SavePath);
        }
        catch (InvalidSaveDataException ex)
        {
            WorldStatus = new Dictionary<long, uint>();
            ModEntry.Log($"Error loading data from {SavePath}, an empty data is created instead.", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    /// <summary>
    /// Generate save data and write it to the save path.
    /// </summary>
    private static void Save()
    {
        if (!Context.IsMainPlayer) return;
        try
        {
            ModEntry.ModHelper.Data.WriteJsonFile(SavePath, WorldStatus);
        }
        catch (InvalidSaveDataException ex)
        {
            ModEntry.Log($"Error saving data to {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    /// <summary>
    /// Event handler for the start of each new day in the game. Used to
    /// reset world status periodically.
    /// </summary>
    private static void OnDayStarted(object s, DayStartedEventArgs e)
    {
        // Clear the higher 4 bits of the info (monthly flags)
        if (Game1.Date.DayOfMonth == 1)
        {
            foreach (var player in WorldStatus.Keys)
            {
                var newValue = WorldStatus[player] & 0x0FFF;
                WorldStatus[player] = newValue;
            }
        }

        // Clear the center 8 bits of the info (weekly flags)
        if (Game1.Date.DayOfWeek == DayOfWeek.Sunday)
            foreach (var player in WorldStatus.Keys)
            {
                var newValue = WorldStatus[player] & 0xF00F;
                WorldStatus[player] = newValue;
            }

        // Clear the lower 4 bits of the info (daily flags)
        foreach (var player in WorldStatus.Keys)
        {
            var newValue = WorldStatus[player] & 0xFFF0;
            WorldStatus[player] = newValue;
        }
    }

    /// <summary>
    /// 在载入游戏存档时加载模组。
    /// Load modules when loading a game save.
    /// </summary>
    private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e) => Load();

    /// <summary>
    /// 在游戏向存档文件写入数据前触发（除了新建存档时）。
    /// Raised before the game begins writes data to the save file (except the initial save creation).
    /// </summary>
    private static void OnSaving(object sender, SavingEventArgs e) => Save();
}