using System;
using System.Linq;
using Common.ExceptionService;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ConvenientChests.Framework.ChestService;

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
    public string BuildingName { get; set; }

    /// <summary>
    /// The tile the chest is found on.
    /// </summary>
    public Vector2 Tile { get; set; }

    public ChestAddress() { }

    public ChestAddress(string locationName, Vector2 tile,
        ChestLocationType locationType = ChestLocationType.Normal, string buildingName = "")
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

    public GameLocation GetLocationFromAddress()
    {
        var location = Game1.locations.FirstOrDefault(l => l.Name == LocationName)
                       ?? throw new InvalidSaveDataException($"Can't find location named {LocationName}");

        if (LocationType != ChestLocationType.Building)
            return location;

        if (location.buildings.ToList() == null)
            throw new InvalidSaveDataException($"Can't find any buildings in location named {location.Name}");

        var building = location.buildings.SingleOrDefault(b => b.GetIndoorsName() == BuildingName)
                       ?? throw new InvalidSaveDataException(
                           $"Save data contains building data in {BuildingName} but building does not exist");

        return building.indoors.Value;
    }

    public Chest GetChestByAddress()
    {
        if (LocationType == ChestLocationType.Refrigerator)
        {
            var house = (FarmHouse)Game1.locations.SingleOrDefault(l =>
                l is FarmHouse f && LocationName == (f.uniqueName?.Value ?? f.Name));

            if (house == null)
                throw new InvalidSaveDataException(
                    $"Save data contains refrigerator data in {LocationName} but location does not exist");

            if (house.upgradeLevel < 1)
                throw new InvalidSaveDataException(
                    $"Save data contains refrigerator data in {LocationName} but refrigerator does not exist");

            return house.fridge.Value;
        }

        var location = GetLocationFromAddress();
        if (location.objects.ContainsKey(Tile) && location.objects[Tile] is Chest chest)
            return chest;

        throw new InvalidSaveDataException($"Can't find chest in {location.Name} at {Tile}");
    }
}