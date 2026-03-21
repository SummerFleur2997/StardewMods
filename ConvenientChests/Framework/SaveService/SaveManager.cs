#nullable enable
using System.IO;
using ConvenientChests.Framework.DataService;

namespace ConvenientChests.Framework.SaveService;

/// <summary>
/// 模组存档数据的管理器，用于管理存档数据，加载保存模组存档数据。
/// The manager responsible for handling the mod's save data, saving and loading the mod state.
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
    /// Generate save data and write it to the save path.
    /// </summary>
    public static void Save(SaveData? saveData = null)
    {
        if (!Context.IsMainPlayer) return;
        try
        {
            saveData ??= Saver.GetSerializableData();
            ModEntry.ModHelper.Data.WriteJsonFile(SavePath, saveData);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error saving chest data to {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    /// <summary>
    /// Load save data from the save path.
    /// </summary>
    public static void Load()
    {
        if (!Context.IsMainPlayer || !File.Exists(AbsoluteSavePath)) return;
        UpdateSaveData();
        try
        {
            LoadSaveData();
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    public static void LoadSaveData(SaveData? saveData = null)
    {
        saveData ??= ModEntry.ModHelper.Data.ReadJsonFile<SaveData>(SavePath) ?? new SaveData();

        foreach (var entry in saveData.ChestEntries)
        {
            if (!entry.Address.GetChestByAddress(out var chest, out var error))
            {
                ModEntry.Log(error, LogLevel.Warn);
                continue;
            }

            var chestData = chest.GetChestData();
            var snapshot = SnapshotManager.GetValueOrDefault(entry.SnapshotID ?? 0);

            chestData.AcceptedItemKinds = entry.AcceptedItems;
            chestData.Snapshot = snapshot;

            if (entry.ItemIconID != null)
            {
                var item = ItemRegistry.Create(entry.ItemIconID);
                chestData.SetIcon(item);
            }

            chestData.SetAlias(entry.Alias);
        }
    }

    /// <summary>
    /// 更新 1.8.0 之前的存档文件结构。
    /// Updates legacy save files (pre-1.8.0) to the current format.
    /// </summary>
    private static void UpdateSaveData()
    {
        var oldSaveData = ModEntry.ModHelper.Data.ReadJsonFile<SaveData>(SavePath);
        if (oldSaveData is null) return;

        var saveDataVersion = new SemanticVersion(oldSaveData.Version);
        if (!saveDataVersion.IsOlderThan("1.8.1")) return;

        ModEntry.Log($"Updating save data from {saveDataVersion} to {ModEntry.Manifest.Version}", LogLevel.Info);
        var newSaveData = new SaveData();

        try
        {
            newSaveData.Version = ModEntry.Manifest.Version.ToString();
            newSaveData.ChestEntries = oldSaveData.ChestEntries;
            Save(newSaveData);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error update data from {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }
}