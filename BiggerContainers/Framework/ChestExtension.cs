using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;

namespace BiggerContainers.Framework;

public static class ChestExtension
{
    /// <summary>
    /// 检查给定的箱子是否是特殊的箱子。
    /// Checks if the given chest is a special chest.
    /// </summary>
    public static ChestTypes MyChestType(this Chest chest)
    {
        return chest switch
        {
            { Location: FarmHouse { upgradeLevel: >= 1 } or IslandFarmHouse, TileLocation: {X: 0, Y: 0} } 
                => ChestTypes.Fridge,
            { Location: FarmHouse { upgradeLevel: >= 1 } or IslandFarmHouse, fridge.Value: true } 
                => ChestTypes.MiniFridge,
            { specialChestType.Value: Chest.SpecialChestTypes.JunimoChest } 
                => ChestTypes.JunimoChest,
            _ => ChestTypes.None
        };
    }

    /// <summary>
    /// 获取给定冰箱的类型。
    /// Gets the type of fridge for the given fridge.
    /// </summary>
    /// <returns><see cref="ChestTypes"/></returns>
    public static ChestTypes GetFridgeType(this Chest chest)
    {
        // Judge whether the farmhouse has a fridge
        // 判断当前农舍内有没有冰箱
        var location = chest.Location;
        if (location is not (FarmHouse { upgradeLevel: >= 1 } or IslandFarmHouse))
            return ChestTypes.None; 
        // 冰箱在农舍中的坐标位置为 (0, 0)
        // Fridges' tile location in farmhouse is always (0, 0)
        return chest.TileLocation == Vector2.Zero 
            ? ChestTypes.Fridge 
            : ChestTypes.MiniFridge;
    }
}