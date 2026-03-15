using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.API;

public interface IBrsApi
{
    int GetWaterRemainDays(HoeDirt hoeDirt);
    void RefreshWaterRemainDays(HoeDirt hoeDirt);
}