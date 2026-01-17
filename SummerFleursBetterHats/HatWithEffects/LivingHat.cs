namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Effect of Living Hat: slowly recover health and energy.
    /// </summary>
    private static void Action_LivingHat_Healing()
    {
        if (!Game1.shouldTimePass())
            return;

        var player = Game1.player;
        var health = player.health;

        player.health = Math.Min(player.maxHealth, health + 1);
        player.Stamina += 2;
    }
}