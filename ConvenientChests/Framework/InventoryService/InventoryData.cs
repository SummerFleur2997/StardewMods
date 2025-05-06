using System.Collections.Generic;
using ConvenientChests.Framework.ItemService;
using StardewValley;

namespace ConvenientChests.Framework.InventoryService;

public class InventoryData
{
    public Farmer Player { get; }
    public HashSet<ItemKey> LockedItemKinds { get; set; } = new();

    public InventoryData(Farmer playerName)
    {
        Player = playerName;
    }

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
    public void Toggle(ItemKey itemKey)
    {
        if (Locks(itemKey))
            RemoveLocked(itemKey);

        else
            AddLocked(itemKey);
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