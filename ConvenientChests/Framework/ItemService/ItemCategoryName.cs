namespace ConvenientChests.Framework.ItemService;

/// <summary>
/// 记录类别在当前语言下的名称和英文名称。
/// Save the category name in current language and in English.
/// </summary>
internal readonly struct ItemCategoryName : IEquatable<ItemCategoryName>
{
    /// <summary>
    /// 分类的显示名称。
    /// The display name of the category.
    /// </summary>
    public string CategoryDisplayName { get; }

    /// <summary>
    /// 分类的基础名称。
    /// The base name of the category.
    /// </summary>
    public string CategoryBaseName { get; }

    /// <summary>
    /// 构造函数，初始化 ItemCategoryName。
    /// Constructor to initialize ItemCategoryName.
    /// </summary>
    public ItemCategoryName(string categoryDisplayName, string categoryBaseName)
    {
        CategoryDisplayName = categoryDisplayName;
        CategoryBaseName = categoryBaseName;
    }

    public bool Equals(ItemCategoryName other)
    {
        return CategoryDisplayName == other.CategoryDisplayName;
    }
}