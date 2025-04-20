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
    private readonly ISemanticVersion Version;
    private readonly ChestDataManager ChestDataManager;
    private readonly InventoryDataManager InventoryDataManager;

    public Saver(ISemanticVersion version, ChestDataManager chestDataManager, InventoryDataManager inventoryDataManager)
    {
        Version = version;
        ChestDataManager = chestDataManager;
        InventoryDataManager = inventoryDataManager;
    }

    /// <summary>
    /// Build save data for the current game state.
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
                    ChestDataManager.GetChestData(pair.Value),
                    new ChestAddress(location.Name, pair.Key));

            // fridges
            if (location is FarmHouse { upgradeLevel: >= 1 } farmHouse)
                yield return new ChestEntry(
                    ChestDataManager.GetChestData(farmHouse.fridge.Value),
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
                        ChestDataManager.GetChestData(pair.Value),
                        new ChestAddress(location.Name, pair.Key, ChestLocationType.Building, building.GetIndoorsName())
                    );
            }
        }
    }

    private IEnumerable<InventoryEntry> BuildInventoryEntries()
    {
        foreach (var farmer in Game1.getAllFarmers())
        {
            var playerName = farmer.Name;
            yield return new InventoryEntry(InventoryDataManager.GetInventoryData(playerName));
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