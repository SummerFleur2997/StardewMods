using System;
using System.Collections.Generic;
using BetterRetainingSoils.DirtService;
using Microsoft.Xna.Framework;
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
internal struct DirtData(HoeDirt hoeDirt)
{
    public Vector2 Tile { get; set; } = hoeDirt.Tile;
    public int Days { get; set; } = hoeDirt.GetHoeDirtData().WaterRemainDays;
}