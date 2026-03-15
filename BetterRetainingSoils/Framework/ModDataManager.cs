using System.IO;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Framework;

/// <summary>
/// 模组存档数据的管理器，用于管理存档数据，加载保存模组存档数据。
/// The manager responsible for handling the mod's savedata, saving and loading the mod state.
/// </summary>
internal static class ModDataManager
{
    private const string ModID = "SummerFleur.BetterRetainingSoils";

    /// <summary>
    /// Generate save data and write it to the save path.
    /// </summary>
    public static int GetWaterRemainDays(this HoeDirt dirt) =>
        dirt.modData.TryGetValue(ModID, out var rawValue) && int.TryParse(rawValue, out var day)
            ? day
            : 0;

    private static void SetWaterRemainDays(this HoeDirt dirt, int? days = null)
    {
        try
        {
            if (days is null)
            {
                dirt.modData.Remove(ModID);
                return;
            }

            dirt.modData[ModID] = days.ToString();
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to set water remain days: {ex}", LogLevel.Error);
        }
    }

    public static void RefreshStatus(this HoeDirt dirt)
    {
        var days = dirt.GetMaxWaterRemainDays();
        dirt.SetWaterRemainDays(days);
    }

    public static void DayUpdate(this HoeDirt dirt)
    {
        var days = dirt.GetWaterRemainDays() - 1;
        switch (days)
        {
            case < 0:
                return;
            case > 0:
                dirt.state.Set(HoeDirt.watered);
                break;
        }

        dirt.SetWaterRemainDays(days);
    }

    /// <summary>
    /// 根据配置文件获取保湿土壤最多能维持水分多少天。
    /// Gets how many days a retaining soil can maintain 
    /// moisture according to the configuration file.
    /// </summary>
    private static int GetMaxWaterRemainDays(this HoeDirt dirt)
    {
        if (dirt.fertilizer.Value?.Contains(HoeDirt.waterRetentionSoilQualityID) == true)
            return ModEntry.Config.QualitySoilRemainDays;

        if (dirt.fertilizer.Value?.Contains(HoeDirt.waterRetentionSoilID) == true)
            return ModEntry.Config.BasicSoilRemainDays;

        return -1;
    }

    /// <summary>
    /// 清理旧版本的存档缓存文件。
    /// Clean up the cache files of the old version.
    /// </summary>
    public static void TryCleanLegacyCache()
    {
        var dirPath = Path.Combine(ModEntry.ModHelper.DirectoryPath, "savedata");
        if (!Directory.Exists(dirPath))
            return;

        Directory.Delete(dirPath, true);
    }
}