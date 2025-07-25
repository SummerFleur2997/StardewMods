using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.API;

public interface IBrsApi
{
    IHoeDirtData GetHoeDirtData(HoeDirt hoeDirt);
    int GetWaterRemainDays(HoeDirt hoeDirt);
}