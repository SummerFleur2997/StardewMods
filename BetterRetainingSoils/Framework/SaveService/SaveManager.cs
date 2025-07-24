using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterRetainingSoils.DirtService;
using Common.ExceptionService;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Framework.SaveService;

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
    public static void Save()
    {
        if (!Context.IsMainPlayer) return;
        try
        {
            var saveData = GetSerializableData();
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
        try
        {
            var saveData = ModEntry.ModHelper.Data.ReadJsonFile<SaveData>(SavePath);
            foreach (var entry in saveData!.DirtEntries)
            {
                var location = Game1.getLocationFromName(entry.Key);
                if (location == null) continue; 
                var hoeDirts = new Dictionary<HoeDirt, int>();
                foreach (var dirtData in entry.Value)
                {
                    if (location.terrainFeatures.TryGetValue(dirtData.Tile, out var feature) && feature is HoeDirt hoeDirt)
                    {
                        hoeDirts.Add(hoeDirt, dirtData.Days);
                    }
                }
                foreach (var hoeDirt in hoeDirts.Keys)
                {
                    var data = hoeDirt.GetHoeDirtData();
                    data.Initialize(hoeDirts[hoeDirt]);
                }
            }
        }
        catch (InvalidSaveDataException ex)
        {
            ModEntry.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    private static SaveData GetSerializableData()
    {
        return new SaveData
        {
            Version = ModEntry.Manifest.Version.ToString(),
            DirtEntries = Game1.locations
                .Select(location => new
                {
                    location.Name,
                    DirtDataList = location.terrainFeatures.Pairs
                        .Select(pair => pair.Value)
                        .OfType<HoeDirt>()
                        .Where(h => h.state.Value != 2 && h.IsAvailable())
                        .Select(h => new DirtData(h))
                        .ToList()
                })
                .Where(item => item.DirtDataList.Count > 0)
                .ToDictionary(item => item.Name, item => item.DirtDataList)
        };
    }
}