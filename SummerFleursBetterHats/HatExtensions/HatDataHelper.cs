using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;
using StardewValley.Objects;

namespace SummerFleursBetterHats.HatExtensions;

public static class HatDataHelper
{
    private static readonly string DataPath = Path.Combine("Assets", "HatData.json");

    /// <summary>
    /// A dictionary of all hat data. The format is
    /// <see cref="StardewValley.Item.QualifiedItemId"/> -> <see cref="HatData"/>.
    /// </summary>
    private static readonly Dictionary<string, HatData> AllHatData;

    /// <summary>
    /// 获取指定帽子的 <see cref="HatData"/> 数据。
    /// </summary>
    public static HatData GetHatData(this Hat hat) => AllHatData.GetValueOrDefault(hat.QualifiedItemId);

    static HatDataHelper()
    {
        var absoluteDataPath = Path.Combine(ModEntry.ModHelper.DirectoryPath, DataPath);
        if (!File.Exists(absoluteDataPath))
        {
            ModEntry.Log($"Hat data file not found at {absoluteDataPath}.", LogLevel.Error);
            return;
        }

        var data = ModEntry.ModHelper.Data.ReadJsonFile<Dictionary<string, HatData>>(DataPath);
        if (data is null)
        {
            ModEntry.Log($"Failed to load hat data from {absoluteDataPath}.", LogLevel.Error);
            return;
        }

        AllHatData = data;
        ModEntry.Log($"Loaded {AllHatData.Count} hat data.");
    }
}