#nullable enable

using SummerFleursBetterHats.HatExtensions;

namespace SummerFleursBetterHats.HatWithConditions;

/// <summary>
/// A condition system for those hats that have special conditions.
/// </summary>
public static class HatWithConditions
{
    public static void WhenWarped(object? sender, WarpedEventArgs e)
    {
        var data = ModEntry.Manager.CachedHatData;
        if (data is not { Trigger: Trigger.LocationChanged }) return;

        if (data.CheckCondition())
        {
            if (!Game1.player.hasBuff(DefaultBuffID))
                Game1.player.applyBuff(data.ConvertToBuff(DefaultBuffID));
            data.TryPerformAction();
        }
        else
        {
            Game1.player.applyBuff(ModEntry.Manager.EmptyBuff);
        }
    }

    public static void WhenDayStarted(object? sender, DayStartedEventArgs e)
    {
        var data = ModEntry.Manager.CachedHatData;
        if (data is null) return;

        if (data.CheckCondition())
        {
            if (!Game1.player.hasBuff(DefaultBuffID))
                Game1.player.applyBuff(data.ConvertToBuff(DefaultBuffID));
            if (data.Trigger == Trigger.DayStarted)
                data.TryPerformAction();
        }
        else
        {
            Game1.player.applyBuff(ModEntry.Manager.EmptyBuff);
        }
    }
}