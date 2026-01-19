namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Effect of Santa Hat: if the player has forage mastery, get a
    /// Golden Mystery Box as santa's gift, otherwise, a Mystery Box.
    /// </summary>
    private static void Action_Sombrero_AddMoney()
    {
        if (Game1.Date.DayOfWeek != DayOfWeek.Sunday) return;
        var totalEarning = Game1.player.totalMoneyEarned;
        Game1.player.Money += (int)(totalEarning * 0.005);
    }
}