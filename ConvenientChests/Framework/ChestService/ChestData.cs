using System;
using System.Collections.Generic;
using ConvenientChests.Framework.ItemService;
using ConvenientChests.Framework.MultiplayerService;
using StardewModdingAPI;
using StardewValley.Objects;

namespace ConvenientChests.Framework.ChestService;

/// <summary>
/// The extra data associated with a chest, contains the list of items it accepts.
/// 箱子的额外属性，包含该箱子所接受的物品列表。
/// </summary>
internal class ChestData
{
    private readonly WeakReference<Chest> _chestRef;
    public HashSet<ItemKey> AcceptedItemKinds { get; set; } = new();

    public ChestData(Chest chest)
    {
        _chestRef = new WeakReference<Chest>(chest);
    }

    /// <summary>
    /// Set this chest to accept the specified kind of item.
    /// 设置这个箱子接受指定类型的物品。
    /// </summary>
    private void AddAccepted(ItemKey itemKey)
    {
        AcceptedItemKinds.Add(itemKey);
    }

    /// <summary>
    /// Set this chest to not accept the specified kind of item.
    /// 移除这个箱子接受的指定类型的物品。
    /// </summary>
    private void RemoveAccepted(ItemKey itemKey)
    {
        if (AcceptedItemKinds.Contains(itemKey))
            AcceptedItemKinds.Remove(itemKey);
    }

    /// <summary>
    /// Toggle whether this chest accepts the specified kind of item.
    /// 切换这个箱子是否接受指定类型的物品。
    /// </summary>
    public void Toggle(ItemKey itemKey, bool receiver = false)
    {
        if (Accepts(itemKey))
            RemoveAccepted(itemKey);
        else
            AddAccepted(itemKey);

        if (receiver) return;
        if (Context.IsMultiplayer && _chestRef.TryGetTarget(out var chest))
            MultiplayerServer.SendChestData(chest, itemKey);
    }

    /// <summary>
    /// Return whether this chest accepts the given kind of item.
    /// 返回这个箱子是否接受指定类型的物品。
    /// </summary>
    public bool Accepts(ItemKey itemKey)
    {
        return AcceptedItemKinds.Contains(itemKey);
    }
}