using System.IO;
using StardewModdingAPI.Events;

namespace SummerFleursBetterHats.Framework;

internal static class SaveManager
{
    private const string DataKey = "SummerFleur.SummerFleursBetterHats.Status";

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
    private static uint Status { get; set; }

    public static void RegisterEvents()
    {
        ModEntry.ModHelper.Events.GameLoop.DayStarted += OnDayStarted;
        ModEntry.ModHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        ModEntry.ModHelper.Events.GameLoop.Saving += OnSaving;
    }

    /// <summary>
    /// Extension method to attempt to modify world status for a player.
    /// </summary>
    public static bool TryEditLocalPlayerStatus(uint mask)
    {
        try
        {
            Status |= mask;
            if (Context.IsMultiplayer && !Context.IsMainPlayer)
            {
                Game1.player.modData[DataKey] = Status.ToString();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Extension method to check specific world status for a player.
    /// </summary>
    public static bool TryGetLocalPlayerStatus(uint mask) =>
        // use the binary mask to check specific world status
        (Status & mask) != 0;

    /// <summary>
    /// Load save data from the save path.
    /// </summary>
    private static void Load()
    {
        var raw = Game1.player.modData.GetValueOrDefault(DataKey, "0");
        Status = uint.TryParse(raw, out var num) ? num : 0;

        if (!File.Exists(AbsoluteSavePath))
            return;

        try
        {
            var dict = ModEntry.ModHelper.Data.ReadJsonFile<Dictionary<long, uint>>(SavePath);
            if (dict == null)
                return;

            foreach (var (id, status) in dict)
            {
                var player = Game1.GetPlayer(id);
                if (player == null)
                    continue;

                if (player == Game1.player)
                    Status = status;

                player.modData[DataKey] = status.ToString();
            }
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error migrate data from {SavePath}.", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    /// <summary>
    /// Generate save data and write it to the save path.
    /// </summary>
    private static void Save()
    {
        Game1.player.modData[DataKey] = Status.ToString();
        if (!Context.IsMainPlayer || !File.Exists(AbsoluteSavePath)) return;
        File.Delete(AbsoluteSavePath);
    }

    /// <summary>
    /// Event handler for the start of each new day in the game. Used to
    /// reset world status periodically.
    /// </summary>
    private static void OnDayStarted(object s, DayStartedEventArgs e)
    {
        // Clear the mid-higher 8 bits of the info (monthly flags)
        if (Game1.Date.DayOfMonth == 1)
        {
            Status &= 0x00FFFF;
        }

        // Clear the mid-lower 8 bits of the info (weekly flags)
        if (Game1.Date.DayOfWeek == DayOfWeek.Sunday)
        {
            Status &= 0xFF00FF;
        }

        // Clear the lower 4 bits of the info (daily flags)
        Status &= 0xFFFF00;
    }

    /// <summary>
    /// 在载入游戏存档时加载模组。
    /// Load modules when loading a game save.
    /// </summary>
    private static void OnSaveLoaded(object s, SaveLoadedEventArgs e) => Load();

    /// <summary>
    /// 在游戏向存档文件写入数据前触发（除了新建存档时）。
    /// Raised before the game begins writes data to the save file (except the initial save creation).
    /// </summary>
    private static void OnSaving(object s, SavingEventArgs e) => Save();
}