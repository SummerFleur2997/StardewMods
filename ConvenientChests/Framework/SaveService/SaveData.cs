using ConvenientChests.Framework.DataStructs;

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
    public IEnumerable<ChestEntry> ChestEntries { get; set; } = Array.Empty<ChestEntry>();
}