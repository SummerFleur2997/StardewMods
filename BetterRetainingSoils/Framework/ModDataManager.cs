using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Framework;

/// <summary>
/// 模组存档数据的管理器，用于管理存档数据，加载保存模组存档数据。
/// The manager responsible for handling the mod's savedata, saving and loading the mod state.
/// </summary>
internal static class ModDataManager
{
    private const string ModID = "SummerFleur.BetterRetainingSoils";

    private static readonly Queue<HoeDirt> ToUpdate = new();

    /// <summary>
    /// Generate save data and write it to the save path.
    /// </summary>
    public static int GetWaterRemainDays(this HoeDirt dirt) =>
        dirt.modData.TryGetValue(ModID, out var rawValue) && int.TryParse(rawValue, out var day)
            ? day
            : 0;

    /// <summary>
    /// Set the number of days the retaining soil can maintain moisture.
    /// </summary>
    /// <param name="dirt">The target dirt.</param>
    /// <param name="days">The remain days of moisure.</param>
    public static void SetWaterRemainDays(this HoeDirt dirt, int? days = null)
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

    /// <summary>
    /// Refresh the water remain days of dirt.
    /// </summary>
    /// <param name="dirt">The target dirt.</param>
    public static void RefreshStatus(this HoeDirt dirt)
    {
        var days = dirt.GetMaxWaterRemainDays();
        dirt.SetWaterRemainDays(days);
    }

    /// <summary>
    /// Check if the dirt should be updated.
    /// </summary>
    /// <param name="dirt">The dirt instance.</param>
    public static void DayUpdate(this HoeDirt dirt)
    {
        if (dirt.GetMaxWaterRemainDays() > 0)
            ToUpdate.Enqueue(dirt);
    }

    public static void OnSaveLoaded(object s, SaveLoadedEventArgs e)
    {
        Game1.locations
            .Select(l => l.terrainFeatures.Values.OfType<HoeDirt>())
            .SelectMany(dirts => dirts.Where(d => d.GetMaxWaterRemainDays() > 0))
            .ToList()
            .ForEach(l => ToUpdate.Enqueue(l));
    }

    /// <summary>
    /// Update dirts after <see cref="Game1.OnDayStarted"/> to ensure
    /// everything is in ready state.
    /// </summary>
    public static void OnDayStarted(object s, DayStartedEventArgs e)
    {
        while (ToUpdate.TryDequeue(out var dirt))
        {
            try
            {
                var days = dirt.state.Value == 1
                    ? dirt.GetMaxWaterRemainDays()
                    : dirt.GetWaterRemainDays() - 1;

                switch (days)
                {
                    case < 0:
                        continue;
                    case > 0:
                        dirt.state.Set(HoeDirt.watered);
                        break;
                }

                dirt.SetWaterRemainDays(days);
            }
            catch (Exception exception)
            {
                ModEntry.Log($"Failed to update dirt at {dirt.Location.NameOrUniqueName} {dirt.Tile}", LogLevel.Error);
                ModEntry.Log(exception.ToString(), LogLevel.Error);
            }
        }

        ToUpdate.Clear();
        
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
}