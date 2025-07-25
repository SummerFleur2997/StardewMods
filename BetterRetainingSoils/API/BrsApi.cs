using BetterRetainingSoils.DirtService;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.API;

public class BrsApi : IBrsApi
{
    public IHoeDirtData GetHoeDirtData(HoeDirt hoeDirt)
    {
        return hoeDirt.GetHoeDirtData();
    }

    public int GetWaterRemainDays(HoeDirt hoeDirt)
    {
        return hoeDirt.GetHoeDirtData().WaterRemainDays;
    }
}