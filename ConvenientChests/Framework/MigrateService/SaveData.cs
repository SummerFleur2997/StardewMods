#nullable disable
using ConvenientChests.Framework.DataStructs;
using Newtonsoft.Json;

namespace ConvenientChests.Framework.MigrateService;

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

/// <summary>
/// A piece of saved data describing the location of a chest and what items
/// the chest at that location has been assigned to.
/// </summary>
[Serializable]
internal class ChestEntry
{
    /// <summary>
    /// 箱子在存档中的位置。
    /// The chest's location in the world.
    /// </summary>
    public ChestAddress Address { get; set; }

    /// <summary>
    /// 位于 <see cref="Address"/> 处的箱子能够接受的 <see cref="Item.QualifiedItemId"/> 序列。
    /// The sequence of <see cref="Item.QualifiedItemId"/> that were configured to be accepted
    /// by the chest at <see cref="Address"/> .
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(DataConverter))]
    public HashSet<string> AcceptedItems { get; set; }
}