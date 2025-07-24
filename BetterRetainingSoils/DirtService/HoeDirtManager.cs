using System.Runtime.CompilerServices;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.DirtService;

internal static class HoeDirtManager
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
    {
        return hoeDirt.fertilizer.Value switch
        {
            "370" or "(O)370" or "371" or "(O)371" => true,
            _ => false
        };
    }

    public static void DayUpdate(this HoeDirt hoeDirt)
    {
        var data = hoeDirt.GetHoeDirtData();
        if (hoeDirt.state.Value == 1)
        {
            data.RefreshStatus();
            data.IsWatered = true;
        }
        else
        {
            data.UpdateStatus();
            data.IsWatered = false;
        }
    }
}