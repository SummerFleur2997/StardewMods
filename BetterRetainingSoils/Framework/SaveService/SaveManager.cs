using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BetterRetainingSoils.DirtService;
using Common.ExceptionService;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Framework.SaveService;

/// <summary>
/// 模组存档数据的管理器，用于管理存档数据，加载保存模组存档数据。
/// The manager responsible for handling the mod's savedata, saving and loading the mod state.
/// </summary>
internal static class SaveManager
{
    /// <summary>
    /// 存档数据的存储路径，位于 mod 文件夹下的 savedata 文件夹中。
    /// The path to the mod's savedata file, relative to the mod folder.
    /// </summary>
    private static string SavePath(string date)
        => Path.Combine("savedata", Constants.SaveFolderName??"error", $"{date}.json");

    /// <summary>
    /// 存档数据的绝对路径。
    /// The absolute path to the mod's savedata file.
    /// </summary>
    private static string AbsoluteSavePath(string date)
        => Path.Combine(ModEntry.ModHelper.DirectoryPath, SavePath(date));

    /// <summary>
    /// Generate save data and write it to the save path.
    /// </summary>
    public static void Save()
    {
        if (!Context.IsMainPlayer) return;
        var date = Util.ConvertToDate(Game1.stats.DaysPlayed);
        try
        {
            var saveData = GetSerializableData();
            ModEntry.ModHelper.Data.WriteJsonFile(SavePath(date), saveData);
            CleanCache(date);
        }
        catch (InvalidSaveDataException ex)
        {
            ModEntry.Log($"Error saving chest data to {SavePath(date)}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    /// <summary>
    /// Load save data from the save path.
    /// </summary>
    public static void Load()
    {
        var date = Util.ConvertToDate(Game1.stats.DaysPlayed);
        if (!Context.IsMainPlayer || !File.Exists(AbsoluteSavePath(date))) return;
        try
        {
            var saveData = ModEntry.ModHelper.Data.ReadJsonFile<SaveData>(SavePath(date));
            foreach (var entry in saveData!.DirtEntries)
            {
                var location = Game1.getLocationFromName(entry.Key);
                if (location == null) continue; 
                var hoeDirts = new Dictionary<HoeDirt, int>();
                foreach (var dirtData in entry.Value)
                {
                    if (location.terrainFeatures.TryGetValue(dirtData.Tile, out var f) && f is HoeDirt hoeDirt)
                        hoeDirts.Add(hoeDirt, dirtData.Days);
                    else if (location.Objects.TryGetValue(dirtData.Tile, out var o) && o is IndoorPot pot)
                        hoeDirts.Add(pot.hoeDirt.Value, dirtData.Days);
                }
                foreach (var hoeDirt in hoeDirts.Keys)
                {
                    var data = hoeDirt.GetHoeDirtData();
                    data.OnLoad(hoeDirts[hoeDirt]);
                }
            }
        }
        catch (InvalidSaveDataException ex)
        {
            ModEntry.Log($"Error loading chest data from {SavePath(date)}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    /// <summary>
    /// Build save data for the current game state.
    /// 构建当前游戏状态的存档数据。
    /// </summary>
    private static SaveData GetSerializableData()
    {
        var dirtEntries = new Dictionary<string, List<DirtData>>();

        foreach (var location in Game1.locations)
        {
            var cache = new List<DirtData>();

            var dirts = location.terrainFeatures.Pairs
                .Select(pair => pair.Value)
                .OfType<HoeDirt>()
                .Where(h => h.state.Value != 2 && h.IsAvailable())
                .ToList();

            var pots = location.Objects.Pairs
                .Select(pair => pair.Value)
                .OfType<IndoorPot>()
                .Where(h => h.hoeDirt.Value.state.Value != 2 && h.hoeDirt.Value.IsAvailable())
                .ToList();

            if (dirts.Count > 0)
                cache.AddRange(dirts.Select(h => new DirtData(h)));

            if (pots.Count > 0)
                cache.AddRange(pots.Select(p => new DirtData(p)));

            if (cache.Count > 0)
                dirtEntries.Add(location.Name, cache);
        }

        return new SaveData
        {
            Version = ModEntry.Manifest.Version.ToString(),
            DirtEntries = dirtEntries
        };
    }

    /// <summary>
    /// 清理指定日期的缓存文件，只保留最近 14 个文件。
    /// Clean up savedata files for the specified date, keeping only the most recent 14 files.
    /// </summary>
    /// <param name="date">日期，用于构建缓存路径。The date used to build the savedata path.</param>
    /// <seealso cref="Util.ConvertToDaysPlayed"/>
    private static void CleanCache(string date)
    {
        var dirPath = Path.GetDirectoryName(AbsoluteSavePath(date));
        if (!Directory.Exists(dirPath))
        {
            ModEntry.Log($"Directory not found: {SavePath(date)}");
            return;
        }
        try
        {
            // 获取所有 JSON 文件，如果文件数小于等于 14，无需清理
            // Get all JSON files, If the file counts <= 14, no cleanup needed
            var files = Directory.GetFiles(dirPath, "*.json");
            if (files.Length <= 14) return;

            // 解析文件名中的数字并排序
            // Parse number and then sort
            var fileInfoList = files
                .Select(filePath => 
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    return new
                    {
                        Path = filePath,
                        FileName = fileName,
                        Number = Util.ConvertToDaysPlayed(fileName)
                    };
                })
                .OrderByDescending(item => item.Number)
                .ToList();

            // 保留最近的 14 个文件，遍历并删除旧文件
            // Keep nearest 14 files, then iterate and delete old files
            var filesToKeep = fileInfoList
                .Take(14)
                .Select(item => item.Path)
                .ToHashSet();

            foreach (var file in files)
            {
                if (filesToKeep.Contains(file)) continue;
                File.Delete(file);
                ModEntry.Log($"Deleted: {Path.GetFileName(file)}");
            }

            ModEntry.Log("Cleanup complete.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error during cleanup: {ex.Message}");
        }
    }
}