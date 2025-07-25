namespace BetterRetainingSoils.API;

public interface IHoeDirtData
{
    public int WaterRemainDays { get; set; }
    public bool IsWateredToday { get; set; }
}