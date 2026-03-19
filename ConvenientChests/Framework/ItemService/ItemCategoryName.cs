namespace ConvenientChests.Framework.DataStructs;

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
    public string DisplayName { get; }

    /// <summary>
    /// 分类的基础名称。
    /// The base name of the category.
    /// </summary>
    public string BaseName { get; }

    /// <summary>
    /// 构造函数，初始化 ItemCategoryName。
    /// Constructor to initialize ItemCategoryName.
    /// </summary>
    public ItemCategoryName(string displayName, string baseName)
    {
        DisplayName = displayName;
        BaseName = baseName;
    }

    public bool Equals(ItemCategoryName other) => DisplayName == other.DisplayName;
}