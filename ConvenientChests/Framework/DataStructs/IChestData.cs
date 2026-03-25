using ConvenientChests.CategorizeChests.Framework;

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

    /// <summary>
    /// An algorithm that calculate the relative factor of a category based on the accepted items.
    /// </summary>
    /// <param name="data">The relevant <see cref="ChestData"/>.</param>
    /// <returns>The most relative category.</returns>
    public static ItemCategoryName PotentialMostRelevantCategory(this IChestData data)
    {
        var acceptedItemKinds = data.AcceptedItemKinds;

        // default to the first category
        var category = ModEntry.Config.EnableSort
            ? CategoryDataManager.ItemCategories.OrderBy(c => c.DisplayName).First()
            : CategoryDataManager.ItemCategories.FirstOrDefault(c => c.BaseName == "Vegetable");

        var factor = 0.0;

        // traver all categories and leave the most relative one
        foreach (var group in acceptedItemKinds.GroupBy(key => key.GetCategory()))
        {
            var name = group.Key;
            var accepts = group.Count();

            if (!CategoryDataManager.Categories.TryGetValue(name, out var itemsInCategory))
                continue;

            var total = (double)itemsInCategory.Count;
            var newFactor = accepts * accepts / total;
            if (newFactor < factor)
                continue;

            category = name;
            factor = newFactor;
        }

        return category;
    }
}