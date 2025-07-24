using System;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.DirtService;

public class HoeDirtData(HoeDirt dirt)
{
    private readonly WeakReference<HoeDirt> _dirtRef = new (dirt);
    private int _waterRemainDays = -1;
    public bool IsWatered { get; set; }

    public int WaterRemainDays
    {
        get
        {
            if (IsWatered && _dirtRef.TryGetTarget(out var dirt) && dirt.IsAvailable())
                return GetMaxWaterRemainDays();
            return _waterRemainDays;
        }
        private set => _waterRemainDays = value;
    }

    private int GetMaxWaterRemainDays()
    {
        if (!_dirtRef.TryGetTarget(out var dirt)) return -1;
        return dirt.fertilizer.Value switch
        {
            "370" or "(O)370" => ModEntry.Config.BasicSoilRemainDays,
            "371" or "(O)371" => ModEntry.Config.QualitySoilRemainDays,
            _ => -1
        };
    }

    public void RefreshStatus()
    {
        var maxDays = GetMaxWaterRemainDays();
        if (maxDays < 0 || !_dirtRef.TryGetTarget(out var dirt)) return;
        WaterRemainDays = maxDays;
        dirt.state.Value = 1;
        IsWatered = true;
    }

    public void UpdateStatus()
    {
        if (GetMaxWaterRemainDays() < 0 || WaterRemainDays <= 0 || !_dirtRef.TryGetTarget(out var dirt)) return;
        if (--WaterRemainDays > 0) dirt.state.Value = 1;
        IsWatered = false;
    }

    public void Initialize(int waterRemainDays) => WaterRemainDays = waterRemainDays;
}