using StardewValley.Constants;
using StardewValley.Delegates;
using StardewValley.Locations;
using StardewValley.Triggers;

namespace SummerFleursBetterHats;

/// <summary>
/// Extensive methods for the <see cref="StardewValley.GameStateQuery"/>
/// and <seealso cref="StardewValley.Triggers.TriggerActionManager"/>
/// </summary>
public static class GameExtensions
{
    public static void RegisterAll()
    {
        GameStateQuery.Register("SFBH_MINE_LEVEL", MINE_LEVEL);
        TriggerActionManager.RegisterAction("SFBH_AddMysteryBox", AddMysteryBox);
    }

    /// <summary>
    /// Effect of Santa Hat: if the player has forage mastery, get a
    /// Golden Mystery Box as santa's gift, otherwise, a Mystery Box.
    /// </summary>
    private static bool AddMysteryBox(string[] args, TriggerActionContext context, out string error)
    {
        try
        {
            var player = Game1.player;
            var gift = ItemRegistry.Create(player.stats.Get(StatKeys.Mastery(Farmer.foragingSkill)) != 0
                ? "(O)GoldenMysteryBox"
                : "(O)MysteryBox");
            player.addItemByMenuIfNecessary(gift);
            Game1.showGlobalMessage("You got a gift from Santa! Happy Feast Of The Winter Star!"); // TODO: i18n
            error = "no error";
            return true;
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }
    }

    /// <summary>
    /// A query used to check if the current location is the mine,
    /// and the mine level is in the specified range.
    /// </summary>
    /// <remarks>
    /// The query format is:
    /// <c>SFBH_MINE_LEVEL Here MinLevel [MaxLevel]</c>
    /// For example, the query text <c>"SFBH_MINE_LEVEL Here 40 80"</c>
    /// will return true if the current location is the mine, and the
    /// mine level is between 40 and 80.
    /// </remarks>
    private static bool MINE_LEVEL(string[] query, GameStateQueryContext context)
    {
        var location = context.Location;
        if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out var error))
            return GameStateQuery.Helpers.ErrorResult(query, error);

        // Check if the location is a mine
        if (location is not MineShaft mine)
            return false;

        if (!ArgUtility.TryGetInt(query, 2, out var minLevel, out error, "int minLevel") ||
            !ArgUtility.TryGetOptionalInt(query, 3, out var maxLevel, out error, int.MaxValue, "int maxLevel"))
            return GameStateQuery.Helpers.ErrorResult(query, error);

        // Check if the mine level is in the specified range
        var level = mine.mineLevel;
        if (level >= minLevel)
            return level <= maxLevel;

        return false;
    }
}