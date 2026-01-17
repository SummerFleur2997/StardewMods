using StardewValley.Locations;

namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Modifier for the Logo Cap: Fishing and Luck Level are multiplied by the Mine Level.
    /// </summary>
    private static void Modifier_LogoCap_FishAreaInMine(Buff buff)
    {
        if (Game1.currentLocation is not MineShaft mineShaft)
            return;

        var multiplier = mineShaft.mineLevel switch
        {
            20 => 1,
            60 => 2,
            100 => 3,
            _ => 0
        };
        buff.effects.FishingLevel.Value *= multiplier;
        buff.effects.LuckLevel.Value *= multiplier;
    }
}