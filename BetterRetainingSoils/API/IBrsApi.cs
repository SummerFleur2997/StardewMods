using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.API;

public interface IBrsApi
{
    bool GetIsWateredToday(HoeDirt hoeDirt);
    int GetWaterRemainDays(HoeDirt hoeDirt);
    void RefreshWaterRemainDays(HoeDirt hoeDirt);
}