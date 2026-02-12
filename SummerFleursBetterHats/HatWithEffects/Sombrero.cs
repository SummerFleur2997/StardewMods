namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Effect of Sombrero: Get 0.5% of total money earned on Sunday
    /// morning, with no limit.
    /// </summary>
    private static void Action_Sombrero_AddMoney()
    {
        if (Game1.Date.DayOfWeek != DayOfWeek.Sunday) return;
        var totalEarning = Game1.player.totalMoneyEarned;
        Game1.player.Money += (int)(totalEarning * 0.005);
    }
}