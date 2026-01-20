using StardewValley.Delegates;
using Helpers = StardewValley.GameStateQuery.Helpers;

namespace BetterHatsAPI.Framework;

public static class GameExtensions
{
    public static void RegisterMethods() => GameStateQuery.Register($"SummerFleur.BHA_{nameof(HAS_MOD)}", HAS_MOD);

    public static bool HAS_MOD(string[] query, GameStateQueryContext context) =>
        ArgUtility.TryGet(query, 1, out var modID, out var error, true, "string modID")
            ? ModEntry.ModHelper.ModRegistry.IsLoaded(modID)
            : Helpers.ErrorResult(query, error);
}