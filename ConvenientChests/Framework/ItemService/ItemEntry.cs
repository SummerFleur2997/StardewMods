using System;
using StardewValley;

namespace ConvenientChests.Framework.ItemService;

/// <summary>
/// 
/// </summary>
internal readonly struct ItemEntry : IComparable<ItemEntry>
{
    public ItemKey ItemKey { get; }
    public Item Item => ItemKey.GetOne();

    public ItemEntry(ItemKey itemKey)
    {
        ItemKey = itemKey;
    }

    public int CompareTo(ItemEntry other)
    {
        var otherItem = other.Item;
        return Item.CompareTo(otherItem);
    }
}