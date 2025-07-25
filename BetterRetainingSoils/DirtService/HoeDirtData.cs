using System;
using BetterRetainingSoils.API;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.DirtService;

/// <summary>
/// 耕地的额外属性，包含由本模组定义的状态。
/// The extra data associated with a dirt, contains the status of this dirt defined by this.
/// </summary>
public class HoeDirtData : IHoeDirtData
{
    private readonly WeakReference<HoeDirt> _dirtRef;

    /// <inheritdoc cref="WaterRemainDays"/>
    private int _waterRemainDays = -1;

    /// <summary>
    /// 这块耕地的水分还可以维持这么多天。若为 -1 则代表没有使用合法的保湿土壤。
    /// This dirt can still maintain moisture for these days.
    /// </summary>
    public int WaterRemainDays
    {
        get
        {
            if (IsWateredToday && _dirtRef.TryGetTarget(out var dirt) && dirt.IsAvailable())
                return GetMaxWaterRemainDays();
            return _waterRemainDays;
        }
        set => _waterRemainDays = value;
    }

    /// <summary>
    /// 当前耕地今天是否手动浇过水？例如使用水壶或洒水器浇水。
    /// Whether the dirt is watered manual todey. e.g. use the watercan or a sprinkler
    /// </summary>
    public bool IsWateredToday { get; set; }

    public HoeDirtData(HoeDirt dirt)
    {
        var recognizedAsWatered = dirt.Location.IsRainingHere() && dirt.Location.IsOutdoors;
        _dirtRef = new WeakReference<HoeDirt>(dirt);
        IsWateredToday = recognizedAsWatered;
    }

    /// <summary>
    /// 根据配置文件获取保湿土壤最多能维持水分多少天。
    /// Gets how many days a retaining soil can maintain 
    /// moisture according to the configuration file.
    /// </summary>
    private int GetMaxWaterRemainDays()
    {
        if (!_dirtRef.TryGetTarget(out var dirt)) return -1;
        return dirt switch
        {
            _ when dirt.IsFertilizerAppliedWith("371") => ModEntry.Config.QualitySoilRemainDays,
            _ when dirt.IsFertilizerAppliedWith("370") => ModEntry.Config.BasicSoilRemainDays,
            _ => -1
        };
    }

    /// <summary>
    /// 给耕地浇水时，更新水分维持时间。
    /// Updates moisture retention duration when watering the soil.
    /// </summary>
    public void RefreshStatus()
    {
        // 标记当前耕地今天已浇水。
        // Mark this dirt as watered today.
        if (!_dirtRef.TryGetTarget(out var dirt)) return;
        IsWateredToday = true;
        // 若使用了保湿土壤，更新水分维持时间。
        // If used a retaining soil, refresh moisture retention duration.
        var maxDays = GetMaxWaterRemainDays();
        if (maxDays <= 0) return;
        WaterRemainDays = maxDays;
        dirt.state.Value = 1;
    }

    /// <summary>
    /// 当新的一天开始时，减少水分维持时间。
    /// Reduce moisture retention duration when a new day start.
    /// </summary>
    public void UpdateStatus()
    {
        if (!_dirtRef.TryGetTarget(out var dirt) || GetMaxWaterRemainDays() <= 0 || WaterRemainDays <= 0) return;
        // 若保湿土壤还能维持水分，减少水分维持时间。
        // If a retaining soil can still remaining water, reduce moisture retention duration.
        if (--WaterRemainDays > 0) dirt.state.Value = 1;
        IsWateredToday = false;
    }

    /// <summary>
    /// 当加载存档时，从单独的存档数据文件中恢复耕地状态。
    /// When load a save, recover the dirt's status from the savedata.
    /// </summary>
    public void OnLoad(int waterRemainDays) => WaterRemainDays = waterRemainDays;
}