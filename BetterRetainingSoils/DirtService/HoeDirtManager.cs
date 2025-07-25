using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.DirtService;

/// <summary>
/// 负责处理耕地数据的耕地管理器。
/// The hoedirt manager responsible for handling dirt data.
/// </summary>
public static class HoeDirtManager
{
    private static readonly ConditionalWeakTable<HoeDirt, HoeDirtData> Table = new();

    /// <summary>
    /// 获取 <see cref="HoeDirtData"/>。
    /// Get <see cref="HoeDirtData"/>.
    /// </summary>
    /// <param name="hoeDirt">当前耕地。 Current hoedirt.</param>
    public static HoeDirtData GetHoeDirtData(this HoeDirt hoeDirt)
    {
        return Table.GetValue(hoeDirt, h => new HoeDirtData(h));
    }

    /// <summary>
    /// 判断当前耕地是否使用了初级保湿土壤或者高级保湿土壤。
    /// Judge whether this dirt is using a Basic/Quality Retaining Soil.
    /// </summary>
    /// <param name="hoeDirt">需要判断的耕地。 The dirt to judge.</param>
    public static bool IsAvailable(this HoeDirt hoeDirt)
        => hoeDirt.IsFertilizerAppliedWith("370") || hoeDirt.IsFertilizerAppliedWith("371");

    public static IEnumerable<HoeDirt> GetHoeDirt(this GameLocation location)
    {
        return location.terrainFeatures.Pairs
            .Select(pair => pair.Value)
            .OfType<HoeDirt>()
            .Where(h => h.state.Value != 2);
    }

    /// <summary>
    /// 当新的一天开始时，更新当前耕地的状态。
    /// When the game begins a new day, update the status of current dirt.
    /// </summary>
    /// <seealso cref="Patcher.PatchWaterRetention.Patch_GetFertilizerWaterRetentionChance"/>
    /// <remarks>
    /// 由于在 Patch_GetFertilizerWaterRetentionChance 中已经将土壤保湿概率设为了 0，且本方法在
    /// DayStarted 后执行，也就是说在本方法执行时，若当前耕地处于已浇水的情况，则代表有某种 “外界因素”
    /// 给土壤浇水了。例如洒水器，玩家的配偶等等。
    /// Since the soil water retention probability has been set to 0 in
    /// Patch_GetFertilizerWaterRetentionChance, and this method executes
    /// after DayStarted, it means that when this method runs, if the current
    /// dirt is watered, it indicates some "external factor" has watered the
    /// soil. For example, a sprinkler or the player's spouse, etc.
    /// </remarks>
    public static void DayUpdate(this HoeDirt hoeDirt)
    {
        var data = hoeDirt.GetHoeDirtData();
        switch (hoeDirt.state.Value)
        {
            case 1:
                data.RefreshStatus();
                data.IsWateredToday = true;
                break;
            case 0:
                data.UpdateStatus();
                data.IsWateredToday = false;
                break;
        }
    }

    /// <summary>
    /// 检查是否使用了某种肥料。
    /// Check if fertilizer is applied.
    /// </summary>
    /// <param name="dirt">HoeDirt 实例。 The HoeDirt instance.</param>
    /// <param name="itemId">需要检查的肥料。 The fertilizer you want to check.</param>
    public static bool IsFertilizerAppliedWith(this HoeDirt dirt, string itemId)
    {
        return dirt.fertilizer.Value?.Contains(itemId) ?? false;
    }

    /// <summary>
    /// Clear ConditionalWeakTable
    /// 清理 ConditionalWeakTable
    /// </summary>
    public static void ClearHoeDirtData()
    {
        Table.Clear();
    }
}