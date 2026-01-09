using System.IO;
using Common.ExceptionService;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.InventoryService;

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
    public static void Save(SaveData saveData = null)
    {
        if (!Context.IsMainPlayer) return;
        try
        {
            saveData ??= Saver.GetSerializableData();
            ModEntry.ModHelper.Data.WriteJsonFile(SavePath, saveData);
        }
        catch (InvalidSaveDataException ex)
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
        catch (InvalidSaveDataException ex)
        {
            ModEntry.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    public static void LoadSaveData(SaveData saveData = null)
    {
        saveData ??= ModEntry.ModHelper.Data.ReadJsonFile<SaveData>(SavePath);

        foreach (var entry in saveData!.ChestEntries)
        {
            var chestData = entry.Address.GetChestByAddress().GetChestData();
            chestData.AcceptedItemKinds = entry.GetItemSet();
        }

        foreach (var entry in saveData!.InventoryEntries)
        {
            var invtyData = InventoryManager.GetPlayerByID(entry.PlayerID).GetInventoryData();

            invtyData.LockedItemKinds = entry.GetItemSet();
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

        var newSaveData = new SaveData();

        try
        {
            newSaveData.Version = ModEntry.Manifest.Version.ToString();
            newSaveData.ChestEntries = oldSaveData.ChestEntries;
            newSaveData.InventoryEntries = Array.Empty<InventoryEntry>();
            Save(newSaveData);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error update data from {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }
}