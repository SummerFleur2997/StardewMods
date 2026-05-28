using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ConvenientChests.Framework.Extensions;

public static class ChestExtension
{
    /// <summary>
    /// 寻找当前玩家房子里的冰箱。
    /// Find the fridge in the current player's house.
    /// </summary>
    /// <param name="player">玩家 The player</param>
    /// <returns>玩家房子里的冰箱 The fridge in player's house</returns>
    public static Chest? GetFridge(this Farmer player)
    {
        if (Game1.player.IsMainPlayer)
            return Utility.getHomeOfFarmer(player).fridge.Value;

        return Game1.locations.OfType<Cabin>()
            .FirstOrDefault(c => c.owner == player)?
            .fridge.Value;
    }

    /// <summary>
    /// 寻找搜寻半径范围内的箱子。
    /// Search for chests within the search radius.
    /// </summary>
    /// <param name="farmer">玩家 The player</param>
    /// <param name="radius">搜寻半径 Search radius</param>
    /// <returns>附近所有箱子的列表 List of all chests nearby</returns>
    public static IEnumerable<Chest> GetNearbyChests(this Farmer farmer, int radius)
    {
        var location = farmer.currentLocation;
        var point = farmer.Tile;

        // chests
        foreach (var chest in GetNearbyObjects<Chest>(location, point, radius))
            yield return chest;

        // fridge
        if (location is FarmHouse { upgradeLevel: > 0 } farmHouse)
        {
            var kitchenStandingSpot = farmHouse.getKitchenStandingSpot();
            if (InRadius(radius, point, kitchenStandingSpot.X + 2, kitchenStandingSpot.Y - 1))
                yield return farmHouse.fridge.Value;

            yield break;
        }

        // buildings
        var buildings = location.buildings
            .Where(building => InRadius(radius, point, building.tileX.Value, building.tileY.Value));

        foreach (var chest in buildings.SelectMany(building => building.buildingChests))
            yield return chest;
    }

    private static IEnumerable<T> GetNearbyObjects<T>(GameLocation location, Vector2 point, int radius)
        where T : Object
    {
        return location.Objects.Pairs
            .Where(p => p.Value is T && InRadius(radius, point, p.Key))
            .Select(p => (T)p.Value);
    }

    private static bool InRadius(int radius, Vector2 a, Vector2 b)
    {
        return Math.Abs(a.X - b.X) < radius && Math.Abs(a.Y - b.Y) < radius;
    }

    private static bool InRadius(int radius, Vector2 a, int x, int y)
    {
        return Math.Abs(a.X - x) < radius && Math.Abs(a.Y - y) < radius;
    }
}