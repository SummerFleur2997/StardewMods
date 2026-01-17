using StardewValley.Extensions;

namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Modifier for the Sou'wester: +2 Fishing Level in stormy weather
    /// </summary>
    private static void Modifier_Souwester_Stormy(Buff buff)
    {
        if (Game1.currentLocation.GetWeather().Weather.EqualsIgnoreCase("Storm"))
            buff.effects.FishingLevel.Value = 2;
    }
}