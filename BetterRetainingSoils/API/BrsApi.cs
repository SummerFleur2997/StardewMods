using BetterRetainingSoils.DirtService;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.API;

public class BrsApi : IBrsApi
{
    public bool GetIsWateredToday(HoeDirt hoeDirt)
    {
        return hoeDirt.GetHoeDirtData().IsWateredToday;
    }

    public int GetWaterRemainDays(HoeDirt hoeDirt)
    {
        return hoeDirt.GetHoeDirtData().WaterRemainDays;
    }
}