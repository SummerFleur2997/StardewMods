using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.MultiplayerService;
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
        get
        {
            var id = ChestRef.ReadModDataAsInt64(SnapshotKey) ?? 0;
            return SnapshotManager.GetValueOrDefault(id);
        }
        set => ChestRef.WriteModData(SnapshotKey, value?.UniqueID);
    }

    /// <inheritdoc />
    public HashSet<string> AcceptedItems
    {
        get => Snapshot?.AcceptedItems ?? _acceptedItems;
        set => _acceptedItems = value;
    }

    private HashSet<string> _acceptedItems;

    public ChestData(Chest chest, string[] accepted, ChestDataSnapshot? snapshot)
    {
        ChestRef = chest;
        _acceptedItems = accepted.ToHashSet();
        Snapshot = snapshot;
    }

    /// <summary>
    /// Toggle whether this chest accepts the specified kind of item.
    /// 切换这个箱子是否接受指定类型的物品。
    /// </summary>
    /// <param name="itemKey">The item to toggle.</param>
    public void Toggle(string itemKey)
    {
        if (Snapshot != null)
            return;

        if (this.Accepts(itemKey))
            this.RemoveAccepted(itemKey);
        else
            this.AddAccepted(itemKey);

        if (Context.IsMultiplayer)
            MultiplayerServer.SendChestData(ChestRef, itemKey);
    }

    public void MigrateDataFromOldChest(Chest oldChest)
    {
        var oldData = oldChest.GetChestData();

        Snapshot = oldData.Snapshot;
        _acceptedItems = oldData._acceptedItems;
    }
}