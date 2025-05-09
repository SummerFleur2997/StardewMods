using ConvenientChests.CategorizeChests;
using ConvenientChests.Framework.ExceptionService;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.StashToChests;
using StardewModdingAPI;
using System;
using System.IO;

namespace ConvenientChests.Framework.SaveService;

/// <summary>
/// 模组存档数据的管理器，用于管理存档数据，加载保存模组存档数据。
/// The manager responsible for handling the mod's save data, saving and loading the mod state.
/// </summary>
internal static class SaveManager
{
    private static CategorizeChestsModule CategorizeModule { get; }
    private static StashToChestsModule StashModule { get; }
    private static ISemanticVersion Version { get; }
    
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

    static SaveManager()
    {
        Version = ModEntry.Manifest.Version;
        CategorizeModule = ModEntry.CategorizeModule;
        StashModule = ModEntry.StashModule;
    }

    /// <summary>
    /// Generate save data and write it to the save path.
    /// </summary>
    public static void Save(SaveData saveData = null)
    {
        try
        {
            var saver = new Saver(Version, CategorizeModule.ChestManager, StashModule.InventoryManager);
            saveData ??= saver.GetSerializableData();
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
        if (!File.Exists(AbsoluteSavePath)) return;
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
    
    private static void LoadSaveData()
    {
        var saveData = ModEntry.ModHelper.Data.ReadJsonFile<SaveData>(SavePath) ?? new SaveData();

        foreach (var entry in saveData.ChestEntries)
        {
            var chest = CategorizeModule.ChestManager.GetChestByAddress(entry.Address);
            var chestData = CategorizeModule.ChestManager.GetChestData(chest);

            chestData.AcceptedItemKinds = entry.GetItemSet();
        }

        foreach (var entry in saveData.InventoryEntries)
        {
            var player = StashModule.InventoryManager.GetPlayerByID(entry.PlayerID);
            var invtyData = StashModule.InventoryManager.GetInventoryData(player);

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