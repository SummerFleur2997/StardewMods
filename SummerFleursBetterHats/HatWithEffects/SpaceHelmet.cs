using StardewValley.Locations;

namespace SummerFleursBetterHats.HatWithEffects;

public static partial class HatWithEffects
{
    /// <summary>
    /// Checker for the Space Helmet: whether the player
    /// is in the dangerous mine.
    /// </summary>
    /// <returns></returns>
    private static bool Condition_SpaceHelmet_InDifficultyMine() =>
        Game1.currentLocation is MineShaft m && m.GetAdditionalDifficulty() > 0;

    /// <summary>
    /// Modifier for the Space Helmet: multiplies the buff
    /// based on the mine's additional difficulty.
    /// </summary>
    private static void Modifier_SpaceHelmet_MineDifficulty(Buff buff)
    {
        if (Game1.currentLocation is not MineShaft mineShaft)
            return;

        var multiplier = mineShaft.GetAdditionalDifficulty();
        buff.effects.AttackMultiplier.Value *= multiplier;
        buff.effects.Defense.Value *= multiplier;
        buff.effects.Speed.Value *= multiplier;
    }
}