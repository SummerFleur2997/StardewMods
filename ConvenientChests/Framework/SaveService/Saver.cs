using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.InventoryService;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ConvenientChests.Framework.SaveService;

/// <summary>
/// The class responsible for producing data to be saved.
/// </summary>
internal static class Saver
{
    /// <summary>
    /// The version of the mod.
    /// 模组版本信息。
    /// </summary>
    private static ISemanticVersion Version { get; } = ModEntry.Manifest.Version;

    /// <summary>
    /// Build save data for the current game state.
    /// 构建当前游戏状态的存档数据。
    /// </summary>
    public static SaveData GetSerializableData()
    {
        return new SaveData
        {
            Version = Version.ToString(),
            ChestEntries = BuildChestEntries().Where(e => e.AcceptedItems.Any()).ToList(),
            InventoryEntries = BuildInventoryEntries() //.Where(e => e.LockedItems.Any()).ToList()
        };
    }

    private static IEnumerable<ChestEntry> BuildChestEntries()
    {
        foreach (var location in Game1.locations)
        {
            // chests
            foreach (var pair in GetLocationChests(location))
                yield return new ChestEntry(
                    pair.Value.GetChestData(),
                    new ChestAddress(location.Name, pair.Key));

            // fridges
            if (location is FarmHouse { upgradeLevel: >= 1 } farmHouse)
                yield return new ChestEntry(
                    farmHouse.fridge.Value.GetChestData(),
                    new ChestAddress
                        (farmHouse.uniqueName?.Value ?? farmHouse.Name, Vector2.Zero, ChestLocationType.Refrigerator)
                );
            else
                foreach (var building in location.buildings.Where(b => b.indoors.Value != null))
                foreach (var pair in GetLocationChests(building.indoors.Value))
                    yield return new ChestEntry(
                        pair.Value.GetChestData(),
                        new ChestAddress(location.Name, pair.Key, ChestLocationType.Building, building.GetIndoorsName())
                    );
        }
    }

    private static IEnumerable<InventoryEntry> BuildInventoryEntries()
    {
        return Game1.getAllFarmers()
            .Select(player => new InventoryEntry(
                InventoryManager.GetInventoryData(player),
                player.Name,
                player.UniqueMultiplayerID
            ));
    }

    /// <summary>
    /// Retrieve a collection of the chest objects present in the given
    /// location, keyed by their tile location.
    /// </summary>
    private static IDictionary<Vector2, Chest> GetLocationChests(GameLocation location)
    {
        return location.Objects.Pairs
            .Where(pair => pair.Value is Chest c && c.playerChest.Value)
            .ToDictionary(
                pair => pair.Key,
                pair => (Chest)pair.Value
            );
    }
}