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
internal class Saver
{
    /// <summary>
    /// The version of the mod.
    /// 模组版本信息。
    /// </summary>
    private ISemanticVersion Version { get; }

    /// <summary>
    /// The chest manager responsible for handling chest data.
    /// 负责处理箱子数据的箱子管理器。
    /// </summary>
    private ChestManager ChestManager { get; }

    /// <summary>
    /// The inventory manager responsible for handling inventory data.
    /// 负责处理背包数据的背包管理器。
    /// </summary>
    private InventoryManager InventoryManager { get; }

    public Saver(ISemanticVersion version, ChestManager chestManager, InventoryManager inventoryManager)
    {
        Version = version;
        ChestManager = chestManager;
        InventoryManager = inventoryManager;
    }

    /// <summary>
    /// Build save data for the current game state.
    /// 构建当前游戏状态的存档数据。
    /// </summary>
    public SaveData GetSerializableData()
    {
        return new SaveData
        {
            Version = Version.ToString(),
            ChestEntries = BuildChestEntries().Where(e => e.AcceptedItems.Any()).ToList(),
            InventoryEntries = BuildInventoryEntries()
        };
    }

    private IEnumerable<ChestEntry> BuildChestEntries()
    {
        foreach (var location in Game1.locations)
        {
            // chests
            foreach (var pair in GetLocationChests(location))
                yield return new ChestEntry(
                    ChestManager.GetChestData(pair.Value),
                    new ChestAddress(location.Name, pair.Key));

            // fridges
            if (location is FarmHouse { upgradeLevel: >= 1 } farmHouse)
                yield return new ChestEntry(
                    ChestManager.GetChestData(farmHouse.fridge.Value),
                    new ChestAddress
                    {
                        LocationName = farmHouse.uniqueName?.Value ?? farmHouse.Name,
                        LocationType = ChestLocationType.Refrigerator
                    }
                );
            else
            {
                foreach (var building in location.buildings.Where(b => b.indoors.Value != null))
                foreach (var pair in GetLocationChests(building.indoors.Value))
                    yield return new ChestEntry(
                        ChestManager.GetChestData(pair.Value),
                        new ChestAddress(location.Name, pair.Key, ChestLocationType.Building, building.GetIndoorsName())
                    );
            }
        }
    }

    private IEnumerable<InventoryEntry> BuildInventoryEntries()
    {
        foreach (var player in Game1.getAllFarmers())
        {
            yield return new InventoryEntry(InventoryManager.GetInventoryData(player));
        }
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