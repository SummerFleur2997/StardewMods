using System.IO;
using ConvenientChests.Framework.DataService;

namespace ConvenientChests.Framework.MigrateService;

/// <summary>
/// 从旧版本迁移存档数据的管理器。
/// The manager responsible for migrating save data from older versions.
/// </summary>
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
    /// Load save data from the save path.
    /// </summary>
    public static void Load()
    {
        if (!Context.IsMainPlayer || !File.Exists(AbsoluteSavePath))
            return;

        UpdateSaveData(out var data);
        try
        {
            data ??= new SaveData();

            foreach (var entry in data.ChestEntries)
            {
                if (!entry.Address.GetChestByAddress(out var chest, out var error))
                {
                    ModEntry.Log(error, LogLevel.Warn);
                    continue;
                }

                var chestData = chest.GetChestData();

                chestData.AcceptedItems = entry.AcceptedItems;
            }
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    /// <summary>
    /// 更新 2.0.0 之前的存档文件结构。
    /// Updates legacy save files (pre-2.0.0) to the current format.
    /// </summary>
    private static void UpdateSaveData(out SaveData? saveData)
    {
        saveData = null;
        var oldSaveData = ModEntry.ModHelper.Data.ReadJsonFile<SaveData>(SavePath);
        if (oldSaveData is null) return;

        var saveDataVersion = new SemanticVersion(oldSaveData.Version);
        if (!saveDataVersion.IsOlderThan("2.0.0-alpha")) return;

        ModEntry.Log($"Updating save data from {saveDataVersion} to {ModEntry.Manifest.Version}", LogLevel.Info);
        var newSaveData = new SaveData();

        try
        {
            newSaveData.Version = ModEntry.Manifest.Version.ToString();
            newSaveData.ChestEntries = oldSaveData.ChestEntries;
            saveData = newSaveData;
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error update data from {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }
}