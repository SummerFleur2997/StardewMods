using BetterRetainingSoils.Framework;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.API;

public class BrsApi : IBrsApi
{
    public int GetWaterRemainDays(HoeDirt hoeDirt) => hoeDirt.GetWaterRemainDays();

    public void RefreshWaterRemainDays(HoeDirt hoeDirt) => hoeDirt.RefreshStatus();
}