using Newtonsoft.Json;

namespace ConvenientChests.Framework.DataStructs;

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
    /// 箱子标签所显示的物品。
    /// The item that displays on the chest's label.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? ItemIconID { get; set; }

    /// <summary>
    /// 箱子标签备注。
    /// The user-defined alias for this chest.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Alias { get; set; }

    /// <summary>
    /// 箱子所使用的快照 ID。
    /// The ID of the snapshot that this chest is using.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public long? SnapshotID { get; set; }

    /// <summary>
    /// 位于 <see cref="Address"/> 处的箱子能够接受的 <see cref="ItemKey"/> 序列。
    /// The sequence of <see cref="ItemKey"/> that were configured to be accepted
    /// by the chest at <see cref="Address"/> .
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(DataConverter))]
    public HashSet<ItemKey> AcceptedItems { get; set; }

#pragma warning disable CS8618 // 预留给反序列化器使用的 ctor

    public ChestEntry() { }

#pragma warning restore CS8618

    public ChestEntry(ChestData data, ChestAddress address)
    {
        Address = address;
        ItemIconID = data.ItemIcon?.QualifiedItemId;
        Alias = data.Alias;
        SnapshotID = data.Snapshot?.UniqueID;
        AcceptedItems = data.AcceptedItemKinds;
    }

    public bool ShouldBeSerialized()
        => AcceptedItems.Any() || Alias != null || ItemIconID != null || SnapshotID is not (null or 0);
}
