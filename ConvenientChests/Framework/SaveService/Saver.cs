using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ConvenientChests.Framework.SaveService;

/// <summary>
/// The class responsible for producing data to be saved.
/// </summary>
internal static class Saver
{
    /// <summary>
    /// Build save data for the current game state.
    /// 构建当前游戏状态的存档数据。
    /// </summary>
    public static SaveData GetSerializableData()
    {
        return new SaveData
        {
            Version = ModEntry.Manifest.Version.ToString(),
            ChestEntries = BuildChestEntries().Where(e => e.ShouldBeSerialized()).ToList(),
        };
    }

    private static IEnumerable<ChestEntry> BuildChestEntries()
    {
        foreach (var location in Game1.locations)
        {
            // chests
            foreach (var kvp in location.GetChestsHere())
                yield return kvp.Value.ToChestEntry(new ChestAddress(location.Name, kvp.Key));

            // fridges
            if (location is FarmHouse { upgradeLevel: >= 1 } farmHouse)
            {
                var address = new ChestAddress(
                    farmHouse.uniqueName?.Value ?? farmHouse.Name, Vector2.Zero, ChestLocationType.Refrigerator);
                yield return farmHouse.fridge.Value.ToChestEntry(address);
            }
            else
            {
                var buildings = location.buildings.Where(b => b.indoors.Value != null);
                foreach (var building in buildings)
                foreach (var kvp in building.indoors.Value.GetChestsHere())
                {
                    var address = new ChestAddress(
                        location.Name, kvp.Key, ChestLocationType.Building, building.GetIndoorsName());
                    yield return kvp.Value.ToChestEntry(address);
                }
            }
        }
    }

    /// <summary>
    /// Retrieve a collection of the chest objects present in the given
    /// location, keyed by their tile location.
    /// </summary>
    private static IDictionary<Vector2, Chest> GetChestsHere(this GameLocation location)
    {
        return location.Objects.Pairs
            .Where(pair => pair.Value is Chest c && c.playerChest.Value)
            .ToDictionary(
                pair => pair.Key,
                pair => (Chest)pair.Value
            );
    }
}