namespace ConvenientChests.Framework.DataStructs;

internal interface IChestData
{
    string? Alias { get; }

    HashSet<ItemKey> AcceptedItemKinds { get; }

    void ToggleItem(ItemKey itemKey);
}

internal static class ChestDataExtensions
{
    /// <summary>
    /// Set this chest to accept the specified kind of item.
    /// 设置这个箱子接受指定类型的物品。
    /// </summary>
    public static void AddAccepted(this IChestData data, ItemKey itemKey) => data.AcceptedItemKinds.Add(itemKey);

    /// <summary>
    /// Set this chest to not accept the specified kind of item.
    /// 移除这个箱子接受的指定类型的物品。
    /// </summary>
    public static void RemoveAccepted(this IChestData data, ItemKey itemKey) => data.AcceptedItemKinds.Remove(itemKey);

    /// <summary>
    /// Return whether this chest accepts the given kind of item.
    /// 返回这个箱子是否接受指定类型的物品。
    /// </summary>
    public static bool Accepts(this IChestData data, ItemKey itemKey) => data.AcceptedItemKinds.Contains(itemKey);
}