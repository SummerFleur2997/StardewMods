using System.Collections.Generic;
using ConvenientChests.Framework.ItemService;

namespace ConvenientChests.Framework.InventoryService;

public class InventoryData
{
    public string PlayerName { get; }
    public HashSet<ItemKey> LockedItemKinds { get; set; } = new();

    public InventoryData(string playerName)
    {
        PlayerName = playerName;
    }

    /// <summary>
    /// Set this chest to accept the specified kind of item.
    /// </summary>
    private void AddLocked(ItemKey itemKey)
    {
        LockedItemKinds.Add(itemKey);
    }

    /// <summary>
    /// Set this chest to not accept the specified kind of item.
    /// </summary>
    private void RemoveLocked(ItemKey itemKey)
    {
        if (LockedItemKinds.Contains(itemKey))
            LockedItemKinds.Remove(itemKey);
    }

    /// <summary>
    /// Toggle whether this chest accepts the specified kind of item.
    /// </summary>
    public void Toggle(ItemKey itemKey)
    {
        if (Locks(itemKey))
            RemoveLocked(itemKey);

        else
            AddLocked(itemKey);
    }

    /// <summary>
    /// Return whether this chest accepts the given kind of item.
    /// </summary>
    public bool Locks(ItemKey itemKey)
    {
        return LockedItemKinds.Contains(itemKey);
    }
}