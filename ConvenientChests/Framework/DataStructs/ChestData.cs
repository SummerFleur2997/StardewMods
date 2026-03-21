#nullable enable
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.MultiplayerService;
using StardewValley.Objects;

namespace ConvenientChests.Framework.DataStructs;

/// <summary>
/// The extra data associated with a chest, contains the list of items it accepts.
/// 箱子的额外属性，包含该箱子所接受的物品列表。
/// </summary>
internal class ChestData : IChestData
{
    private readonly WeakReference<Chest> _chestRef;

    public string? Alias { get; private set; }

    public Item? ItemIcon { get; private set; }

    public HashSet<ItemKey> AcceptedItemKinds
    {
        get => Snapshot?.AcceptedItemKinds ?? _acceptedItemKinds;
        set => _acceptedItemKinds = value;
    }

    private HashSet<ItemKey> _acceptedItemKinds = new();

    public ChestDataSnapshot? Snapshot { get; set; }

    public ChestData(Chest chest)
    {
        _chestRef = new WeakReference<Chest>(chest);
    }

    /// <inheritdoc cref="ToggleItem(ItemKey, bool)"/>
    public void ToggleItem(ItemKey itemKey) => ToggleItem(itemKey, false);

    public Chest? GetChest() => _chestRef.TryGetTarget(out var chest) ? chest : null;

    /// <summary>
    /// Toggle whether this chest accepts the specified kind of item.
    /// 切换这个箱子是否接受指定类型的物品。
    /// </summary>
    /// <param name="itemKey">The item to toggle.</param>
    /// <param name="receiver">Whether this is a receiver of the toggle event, this param
    /// is used in multiplayer sync.</param>
    public void ToggleItem(ItemKey itemKey, bool receiver)
    {
        if (Snapshot != null)
            return;

        if (this.Accepts(itemKey))
            this.RemoveAccepted(itemKey);
        else
            this.AddAccepted(itemKey);

        if (receiver) return;
        if (Context.IsMultiplayer && _chestRef.TryGetTarget(out var chest))
            MultiplayerServer.SendChestData(chest, itemKey);
    }

    /// <summary>
    /// Change the alias of this chest.
    /// 编辑这个箱子的备注。
    /// </summary>
    /// <param name="alias">The new alias.</param>
    /// <param name="receiver">Whether this is a receiver of the edit event, this param
    /// is used in multiplayer sync.</param>
    public void SetAlias(string alias, bool receiver = false)
    {
        Alias = string.IsNullOrWhiteSpace(alias) ? null : alias.Trim();

        if (receiver) return;
        if (Context.IsMultiplayer && _chestRef.TryGetTarget(out var chest))
            MultiplayerServer.SendChestData(chest, alias: Alias);
    }

    /// <summary>
    /// Change the icon of this chest.
    /// 编辑这个箱子的图标。
    /// </summary>
    /// <param name="item">The new icon item.</param>
    /// <param name="receiver">Whether this is a receiver of the edit event, this param
    /// is used in multiplayer sync.</param>
    public void SetIcon(Item? item, bool receiver = false)
    {
        ItemIcon = item;

        if (receiver) return;
        if (Context.IsMultiplayer && _chestRef.TryGetTarget(out var chest))
            MultiplayerServer.SendChestData(chest, itemId: ItemIcon?.QualifiedItemId);
    }

    public void MigrateDataFromOldChest(Chest oldChest)
    {
        var oldData = oldChest.GetChestData();

        Snapshot = oldData.Snapshot;
        _acceptedItemKinds = oldData._acceptedItemKinds;
    }
}