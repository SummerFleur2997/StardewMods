using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace ConvenientChests.Framework;

internal static class Utility
{
    public static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
            yield return YieldBatchElements(enumerator, batchSize - 1);
    }

    private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
    {
        yield return source.Current;
        for (var i = 0; i < batchSize && source.MoveNext(); i++)
            yield return source.Current;
    }

    public static IDictionary<TKey, IEnumerable<TValue>> KeyBy<TKey, TValue>(this IEnumerable<TValue> values,
        Func<TValue, TKey> makeKey)
    {
        var dict = new Dictionary<TKey, IEnumerable<TValue>>();

        foreach (var value in values)
        {
            var key = makeKey(value);

            if (!dict.ContainsKey(key))
                dict[key] = new List<TValue>();

            ((List<TValue>)dict[key]).Add(value);
        }

        return dict;
    }

    /// <summary>
    /// Get all game locations.
    /// </summary>
    public static IEnumerable<GameLocation> GetLocations()
    {
        return Game1.locations
            .Concat(
                from location in Game1.locations
                where location is not null
                from building in location.buildings
                where building.indoors.Value != null
                select building.indoors.Value
            );
    }

    public static string GetCategoryBaseName(int category)
    {
        switch (category)
        {
            case -103:
                return "Skill Book";
            case -102:
                return "Book";
            case -101:
            case -98:
                break;
            case -100:
                return "Clothes";
            case -99:
                return "Tool";
            case -97:
                return "Footwear";
            case -96:
                return "Ring";
            case -81:
                return "Forage";
            case -80:
                return "Flower";
            case -79:
                return "Fruit";
            case -78:
            case -77:
            case -76:
                break;
            case -75:
                return "Vegetable";
            case -74:
                return "Seed";
            case -28:
                return "Monster Loot";
            case -27:
            case -26:
                return "Artisan Goods";
            case -25:
            case -7:
                return "Cooking";
            case -24:
                return "Decor";
            case -22:
                return "Fishing Tackle";
            case -21:
                return "Bait";
            case -20:
                return "Trash";
            case -19:
                return "Fertilizer";
            case -18:
            case -14:
            case -6:
            case -5:
                return "Animal Product";
            case -16:
            case -15:
                return "Resource";
            case -12:
            case -2:
                return "Mineral";
            case -8:
                return "Crafting";
            case -4:
                return "Fish";
            case 0:
                return "Artifact";
        }

        return "";
    }
}