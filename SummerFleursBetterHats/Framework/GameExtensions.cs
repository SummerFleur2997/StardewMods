using StardewValley.Delegates;
using StardewValley.Locations;
using StardewValley.Triggers;
using Helpers = StardewValley.GameStateQuery.Helpers;

namespace SummerFleursBetterHats.Framework;

/// <summary>
/// Extensive methods for the <see cref="StardewValley.GameStateQuery"/>
/// and <seealso cref="StardewValley.Triggers.TriggerActionManager"/>
/// </summary>
public static class GameExtensions
{
    public static void RegisterMethods()
    {
        GameStateQuery.Register($"SFBH_{nameof(MINE_LEVEL)}", MINE_LEVEL);
        GameStateQuery.Register($"SFBH_{nameof(MINE_LEVEL_RANGE)}", MINE_LEVEL_RANGE);

        TriggerActionManager.RegisterAction($"SFBH_{nameof(ModifyWorldStatus)}", ModifyWorldStatus);
    }

    public static bool ModifyWorldStatus(string[] args, TriggerActionContext context, out string error)
    {
        if (!ArgUtility.TryGetInt(args, 1, out var value, out error, "ushort mask") ||
            !ArgUtility.TryGet(args, 1, out var shopID, out error, true, "string shopID"))
            return false;

        var mask = (ushort)value;
        var player = Game1.player;
        if (SaveManager.TryEditWorldStatus(player.UniqueMultiplayerID, mask))
        {
            ModEntry.Log($"Successfully modified trade info for player {player.Name} in {shopID}.");
            return true;
        }

        ModEntry.Log($"Failed to modify trade info for player {player.Name} in {shopID}.");
        return false;
    }

    /// <summary>
    /// A query used to check if the current location is the mine,
    /// and the mine level contains the specified value.
    /// </summary>
    /// <remarks>
    /// The query format is:
    /// <c>MINE_LEVEL Here [Levels]</c>
    /// For example, the query text <c>"MINE_LEVEL Here 10 20 30"</c>
    /// will return true if the current location is the mine, and the
    /// mine level is 10, 20, or 30.
    /// </remarks>
    private static bool MINE_LEVEL(string[] query, GameStateQueryContext context)
    {
        var location = context.Location;
        if (!Helpers.TryGetLocationArg(query, 1, ref location, out var error))
            return Helpers.ErrorResult(query, error);

        // Check if the location is a mine
        if (location is not MineShaft mine)
            return false;

        for (var i = 2; i < query.Length; i += 1)
        {
            if (!ArgUtility.TryGetInt(query, i, out var level, out error, "int mineLevel"))
                return Helpers.ErrorResult(query, error);

            if (mine.mineLevel == level)
                return true;
        }

        return false;
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
    private static bool MINE_LEVEL_RANGE(string[] query, GameStateQueryContext context)
    {
        var location = context.Location;
        if (!Helpers.TryGetLocationArg(query, 1, ref location, out var error))
            return Helpers.ErrorResult(query, error);

        // Check if the location is a mine
        if (location is not MineShaft mine)
            return false;

        if (!ArgUtility.TryGetInt(query, 2, out var minLevel, out error, "int minLevel") ||
            !ArgUtility.TryGetOptionalInt(query, 3, out var maxLevel, out error, int.MaxValue, "int maxLevel"))
            return Helpers.ErrorResult(query, error);

        // Check if the mine level is in the specified range
        var level = mine.mineLevel;
        if (level >= minLevel)
            return level <= maxLevel;

        return false;
    }
}