namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Condition of Magic Cowboy Hat: 25% of random chance based
    /// on day played, current player, and current time.
    /// </summary>
    private static bool Condition_MagicCowboyHat_TimeRandom()
    {
        var r = Utility.CreateRandom(
            Game1.stats.DaysPlayed,
            Game1.player.UniqueMultiplayerID,
            Game1.timeOfDay);

        for (var i = r.Next(0, 10); i < 10; i++)
            r.Next();

        return r.NextDouble() > 0.67;
    }
}