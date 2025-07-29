using System;
using System.Collections.Generic;
using ConvenientChests.Framework.ItemService;
using ConvenientChests.Framework.MultiplayerService;
using StardewModdingAPI;
using StardewValley.Locations;

namespace ConvenientChests.Framework.InventoryService;

/// <summary>
/// The extra data associated with a player, contains the list of items they locks.
/// 玩家的额外属性，包含该玩家物品栏锁定的物品列表。
/// </summary>
internal class InventoryData(FarmHouse farmHouse)
{
    private readonly WeakReference<FarmHouse> _farmHouseRef = new(farmHouse);
    public HashSet<ItemKey> LockedItemKinds { get; set; } = new();

    /// <summary>
    /// Set this player's inventory to lock the specified kind of item.
    /// 设置这个玩家的背包锁定指定类型的物品。
    /// </summary>
    private void AddLocked(ItemKey itemKey)
    {
        LockedItemKinds.Add(itemKey);
    }

    /// <summary>
    /// Set this player's inventory to not lock the specified kind of item.
    /// 移除这个玩家的背包锁定的指定类型的物品。
    /// </summary>
    private void RemoveLocked(ItemKey itemKey)
    {
        if (LockedItemKinds.Contains(itemKey))
            LockedItemKinds.Remove(itemKey);
    }

    /// <summary>
    /// Toggle whether this player's inventory accepts the specified kind of item.
    /// 切换这个玩家的背包是否锁定指定类型的物品。
    /// </summary>
    public void Toggle(ItemKey itemKey, bool receiver = false)
    {
        if (Locks(itemKey))
            RemoveLocked(itemKey);
        else
            AddLocked(itemKey);

        if (receiver) return;
        if (Context.IsMultiplayer && _farmHouseRef.TryGetTarget(out var farmHouse))
            MultiplayerServer.SendInventoryData(farmHouse, itemKey);
    }

    /// <summary>
    /// Return whether this this player's inventory locks the given kind of item.
    /// 返回这个玩家的背包是否锁定指定类型的物品。
    /// </summary>
    public bool Locks(ItemKey itemKey)
    {
        return LockedItemKinds.Contains(itemKey);
    }
}