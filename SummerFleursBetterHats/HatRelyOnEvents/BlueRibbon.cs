using StardewModdingAPI.Events;
using StardewValley.Locations;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public partial class HatRelyOnEvents
{
    private const int ExtraToken = 998;

    private static int _blueRibbonLastTriggeredYear;

    /// <summary>
    /// When entering the Stardew Valley Fair with blue ribbon worn,
    /// get an extra 998 tokens.
    /// </summary>
    private static void BlueRibbonLocationChanged(object s, WarpedEventArgs e)
    {
        if (e.NewLocation is not Town { Name: "Temp" } || Game1.Date.Year == _blueRibbonLastTriggeredYear)
            return;

        _blueRibbonLastTriggeredYear = Game1.Date.Year;
        Game1.player.festivalScore += ExtraToken;
    }
}