using StardewValley;

namespace SummerFleursBetterHats.HatWithConditions;

public static partial class HatWithConditions
{
    private const string GoodOlCapID = "(H)18";

    /// <summary>
    /// Extra condition for Good Ol' Cap: wear it at the beginning of the day.
    /// </summary>
    private static bool CheckConditionForGoodOlCap() => Utilities.PlayerHatIs(GoodOlCapID);

    /// <summary>
    /// Effect of Good Ol' Cap: get gold x20.
    /// </summary>
    private static void GoodOlCap_AddMoney() => Game1.player.Money += 20;
}