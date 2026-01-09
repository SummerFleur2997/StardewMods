using BetterRetainingSoils.DirtService;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Framework.SaveService;

[Serializable]
internal class SaveData
{
    /// <summary>
    /// The mod version that produced this save data.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// A dictionary of location names with available hoedirts.
    /// </summary>
    public Dictionary<string, List<DirtData>> DirtEntries { get; set; }
}

[Serializable]
internal struct DirtData
{
    public Vector2 Tile { get; set; }
    public int Days { get; set; }

    public DirtData(HoeDirt hoeDirt)
    {
        Tile = hoeDirt.Tile;
        Days = hoeDirt.GetHoeDirtData().WaterRemainDays;
    }

    public DirtData(IndoorPot pot)
    {
        Tile = pot.TileLocation;
        Days = pot.hoeDirt.Value.GetHoeDirtData().WaterRemainDays;
    }
}