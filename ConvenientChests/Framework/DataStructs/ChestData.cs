using System.Diagnostics.CodeAnalysis;
using ConvenientChests.Framework.DataService;
using StardewValley.Objects;
using static ConvenientChests.Framework.DataService.ModDataManager;

namespace ConvenientChests.Framework.DataStructs;

/// <summary>
/// The extra data associated with a chest, contains the list of items it accepts.
/// 箱子的额外属性，包含该箱子所接受的物品列表。
/// </summary>
internal class ChestData : IChestData
{
    public readonly Chest ChestRef;

    /// <summary>
    /// 箱子标签备注。
    /// The user-defined alias for this chest.
    /// </summary>
    public string? Alias
    {
        get => ChestRef.ReadModData(AliasKey);
        set => ChestRef.WriteModData(AliasKey, value);
    }

    /// <summary>
    /// 箱子标签所显示的物品。
    /// The item that displays on the chest's label.
    /// </summary>
    public Item? ItemIcon
    {
        get => ChestRef.ReadModDataAsItem(ItemIconKey);
        set => ChestRef.WriteModData(ItemIconKey, value?.QualifiedItemId);
    }

    /// <summary>
    /// 箱子所使用的快照。
    /// The snapshot that this chest is using.
    /// </summary>
    public ChestDataSnapshot? Snapshot
    {
        get => _snapshot;
        set
        {
            ChestRef.WriteModData(SnapshotKey, value?.UniqueID);
            _snapshot = value;
        }
    }

    private ChestDataSnapshot? _snapshot;

    /// <inheritdoc />
    public HashSet<string> AcceptedItems
    {
        get => Snapshot?.AcceptedItems ?? _acceptedItems;
        set => _acceptedItems = value;
    }

    private HashSet<string> _acceptedItems;

    /// <summary>
    /// Whether the <see cref="AcceptedItems"/> was modified
    /// in the multiplayer and needs to be synced.
    /// </summary>
    public bool Dirty { get; private set; }

    /// <summary>
    /// Whether the chest is using a snapshot. This property
    /// is used in multipayer to prevent farmhands from modifying
    /// the snapshot.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Snapshot))]
    public bool IsUsingSnapshot => ChestRef.ReadModData(SnapshotKey) != null;

    public ChestData(Chest chest, HashSet<string> accepted, ChestDataSnapshot? snapshot)
    {
        ChestRef = chest;
        Snapshot = snapshot;
        _acceptedItems = accepted;
    }

    /// <summary>
    /// Toggle whether this chest accepts the specified kind of item.
    /// 切换这个箱子是否接受指定类型的物品。
    /// </summary>
    /// <param name="item">The item to toggle.</param>
    public void Toggle(string item)
    {
        if (Snapshot != null)
            return;

        if (this.Accepts(item))
            this.RemoveAccepted(item);
        else
            this.AddAccepted(item);

        if (Context.IsMultiplayer)
            Dirty = true;
    }

    /// <summary>
    /// Set the <see cref="AcceptedItems"/> and then write them
    /// to the modData dictionary instantly.
    /// </summary>
    public void SetAcceptedItemAndWriteToModData(HashSet<string>? accepted = null)
    {
        if (accepted is not null)
            AcceptedItems = accepted;
        ChestRef.WriteModDataAsEnumerable(AcceptedItemsKey, _acceptedItems);
    }

    /// <summary>
    /// Called after a multiplayer sync request was received.
    /// Updates the <see cref="AcceptedItems"/> from mod data.
    /// </summary>
    public void UpdateAcceptedItems()
    {
        if (Snapshot != null)
            return;

        _acceptedItems = ChestRef.ReadModDataAsEnumerable(AcceptedItemsKey).ToHashSet();
    }

    public void MigrateDataFromOldChest(Chest oldChest)
    {
        var oldData = oldChest.GetChestData();

        Snapshot = oldData.Snapshot;
        _acceptedItems = oldData._acceptedItems;
    }
}