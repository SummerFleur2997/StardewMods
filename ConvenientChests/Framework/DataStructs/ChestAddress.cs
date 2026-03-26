using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ConvenientChests.Framework.DataStructs;

/// <summary>
/// A key that uniquely identifies a spot in the world where a chest exists.
/// </summary>
[Serializable]
internal class ChestAddress
{
    public ChestLocationType LocationType { get; set; }

    /// <summary>
    /// The name of the GameLocation where the chest is.
    /// </summary>
    public string LocationName { get; set; }

    /// <summary>
    /// The name of the building the chest is in, if the location is a
    /// buildable location.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? BuildingName { get; set; }

    /// <summary>
    /// The tile the chest is found on.
    /// </summary>
    public Vector2 Tile { get; set; }

#pragma warning disable CS8618 // 预留给反序列化器使用的 ctor

    public ChestAddress() { }

#pragma warning restore CS8618

    public ChestAddress(string locationName, Vector2 tile,
        ChestLocationType locationType = ChestLocationType.Normal, string? buildingName = null)
    {
        LocationName = locationName;
        Tile = tile;
        LocationType = locationType;
        BuildingName = buildingName;
    }

    public ChestAddress(Chest chest)
    {
        var location = chest.Location;
        var tile = chest.TileLocation;

        if (tile == Vector2.Zero && location is FarmHouse { upgradeLevel: >= 1 })
        {
            LocationName = location.Name;
            Tile = Vector2.Zero;
            LocationType = ChestLocationType.Refrigerator;
            BuildingName = null;
        }

        if (Game1.locations.Contains(location))
        {
            LocationName = location.Name;
            Tile = tile;
            LocationType = ChestLocationType.Normal;
            BuildingName = "";
        }
        else
        {
            LocationName = location.GetRootLocation().Name;
            Tile = tile;
            LocationType = ChestLocationType.Building;
            BuildingName = location.ParentBuilding.GetIndoorsName();
        }
    }

    private bool GetLocationFromAddress(
        [NotNullWhen(true)] out GameLocation? location, [NotNullWhen(false)] out string? error)
    {
        location = Game1.locations.FirstOrDefault(l => l.Name == LocationName);
        error = null;
        if (location == null)
        {
            error = $"Can't find location named {LocationName}";
            return false;
        }

        if (LocationType != ChestLocationType.Building)
        {
            return true;
        }

        if (location.buildings == null)
        {
            error = $"Can't find any buildings in location named {location.Name}";
            return false;
        }

        var building = location.buildings.FirstOrDefault(b => b.GetIndoorsName() == BuildingName);
        if (building == null)
        {
            error = $"Address data contains building data in {BuildingName} but building does not exist";
            return false;
        }

        location = building.indoors.Value;
        return true;
    }

    public bool GetChestByAddress([NotNullWhen(true)] out Chest? chest, [NotNullWhen(false)] out string? error)
    {
        error = null;
        chest = null;

        if (LocationType == ChestLocationType.Refrigerator)
        {
            var house = Game1.locations.FirstOrDefault(l =>
                l is FarmHouse f && LocationName == (f.uniqueName?.Value ?? f.Name)) as FarmHouse;

            if (house == null)
            {
                error = $"Address data contains refrigerator data in {LocationName} but location does not exist";
                return false;
            }

            if (house.upgradeLevel < 1)
            {
                error = $"Address data contains refrigerator data in {LocationName} but refrigerator does not exist";
                return false;
            }

            chest = house.fridge.Value;
            return true;
        }

        if (!GetLocationFromAddress(out var location, out error))
        {
            return false;
        }

        if (location.objects.TryGetValue(Tile, out var obj) && obj is Chest c)
        {
            chest = c;
            return true;
        }

        error = $"Can't find chest in {location.Name} at {Tile}";
        return false;
    }
}

internal enum ChestLocationType
{
    Normal,
    Building,
    Refrigerator
}