#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ItemService;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ConvenientChests.Framework.ChestService;

public static class ChestExtension
{
    /// <summary>
    /// 寻找当前玩家房子里的冰箱。
    /// Find the frige in current player's house.
    /// </summary>
    /// <param name="player">玩家 The player</param>
    /// <returns>玩家房子里的冰箱 The frige in player's house</returns>
    public static Chest? GetFridge(this Farmer player)
    {
        if (Game1.player.IsMainPlayer)
            return Utility.getHomeOfFarmer(player).fridge.Value;

        return Game1.locations.OfType<Cabin>()
            .FirstOrDefault(c => c.owner == player)?
            .fridge.Value;
    }

    /// <summary>
    /// 检查给定的箱子是否是特殊的箱子。
    /// Checks if the given chest is special chest.
    /// </summary>
    public static ChestTypes? CC_SpecialChestType(this Chest? chest)
    {
        return chest switch
        {
            { fridge.Value: true } => chest.GetFridgeType(),
            { specialChestType.Value: Chest.SpecialChestTypes.JunimoChest } => ChestTypes.JunimoChest,
            _ => ChestTypes.None
        };
    }

    /// <summary>
    /// 获取给定冰箱的类型。
    /// Gets the type of fridge for the given fridge.
    /// </summary>
    /// <returns><see cref="ChestTypes"/></returns>
    public static ChestTypes? GetFridgeType(this Chest chest)
    {
        // Judge whether the farmhouse has a fridge
        // 判断当前农舍内有没有冰箱
        var location = Game1.player.currentLocation;
        if (location is not FarmHouse { upgradeLevel: >= 1 }) return null;
        // 冰箱在农舍中的坐标位置为 (0, 0)
        // Fridges' tile location in farmhouse is always (0, 0)
        return chest.TileLocation == Vector2.Zero 
            ? ChestTypes.Fridge 
            : ChestTypes.MiniFridge;
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

    ///  <summary>
    ///  Attempt to move as much as possible of the player's inventory into the given chest
    ///  </summary>
    /// <param name="chest">The chest to put the items in.</param>
    /// <param name="sourceInventory"></param>
    /// <param name="items">Items to put in</param>
    /// <returns>List of Items that were successfully moved into the chest</returns>
    public static IEnumerable<Item> DumpItemsToChest(this Inventory sourceInventory, Chest chest,
        IEnumerable<Item> items)
    {
        return items.Select(item => sourceInventory.TryMoveItemToChest(chest, item))
            .OfType<Item>()
            .ToList();
    }

    ///  <summary>
    ///  Attempt to move as much as possible of the given item stack into the chest.
    ///  </summary>
    ///  <param name="sourceInventory"></param>
    ///  <param name="chest">The chest to put the items in.</param>
    ///  <param name="item">The items to put in the chest.</param>
    ///  <returns>True if at least some of the stack was moved into the chest.</returns>
    private static Item? TryMoveItemToChest(this IInventory sourceInventory, Chest chest, Item item)
    {
        var original = item.Stack;
        var remainder = chest.addItem(item);

        // nothing remains -> remove item
        if (remainder == null)
        {
            var index = sourceInventory.IndexOf(item);
            sourceInventory[index] = null;
            // item.Stack = original;
            return item;
        }

        // nothing changed
        if (remainder.Stack == item.Stack)
            return null;

        // update stack count
        item.Stack = remainder.Stack;

        // return copy for moved item
        var copy = item.Copy();
        copy.Stack = original - remainder.Stack;
        return copy;
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