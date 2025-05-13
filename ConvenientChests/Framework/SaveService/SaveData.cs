using System;
using System.Collections.Generic;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.InventoryService;

namespace ConvenientChests.Framework.SaveService;

[Serializable]
internal class SaveData
{
    /// <summary>
    /// The mod version that produced this save data.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// A list of chest addresses and the chest data associated with them.
    /// </summary>
    public IEnumerable<ChestEntry> ChestEntries { get; set; }

    /// <summary>
    /// A list of player names and backpack items that locked from stash to chests.
    /// </summary>
    public IEnumerable<InventoryEntry> InventoryEntries { get; set; }
}