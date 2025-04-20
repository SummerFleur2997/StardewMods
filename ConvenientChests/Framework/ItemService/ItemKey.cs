using System.Diagnostics.Contracts;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;

namespace ConvenientChests.Framework.ItemService;

public class ItemKey
{
    /// <summary>
    /// 物品的 ID，例如 279。
    /// The ID of the item, e.g., 279.
    /// </summary>
    public string ItemId { get; }

    /// <summary>
    /// 物品的分类标识符，例如 (O)。
    /// The classification identifier of the item, e.g., (O).
    /// </summary>
    public string TypeDefinition { get; }

    /// <summary>
    /// 获取完整的物品唯一标识符，例如 (O)279。
    /// Get the fully qualified unique identifier for the item, e.g., (O)279.
    /// </summary>
    public string QualifiedItemId => $"{TypeDefinition}{ItemId}";

    /// <summary>
    /// 构造函数，使用类型定义和物品 ID 初始化 ItemKey。
    /// Constructor to initialize ItemKey with type definition and item ID.
    /// </summary>
    public ItemKey(string typeDefinition, string itemId)
    {
        TypeDefinition = typeDefinition;
        ItemId = itemId;
    }

    /// <summary>
    /// 构造函数，使用 Item 初始化 ItemKey。
    /// Constructor to initialize ItemKey using a Item instance.
    /// </summary>
    public ItemKey(Item item)
    {
        TypeDefinition = item.TypeDefinitionId;
        ItemId = item.ItemId;
    }

    /// <summary>
    /// 获取对象的哈希值。
    /// Get the hash code for the object.
    /// </summary>
    public override int GetHashCode()
    {
        return QualifiedItemId.GetHashCode();
    }

    /// <summary>
    /// 返回对象的字符串表示形式。
    /// Return the string representation of the object.
    /// </summary>
    public override string ToString()
    {
        return QualifiedItemId;
    }

    /// <summary>
    /// 比较两个 ItemKey 是否相等。
    /// Compare two ItemKey objects for equality.
    /// </summary>
    public override bool Equals(object obj)
    {
        return obj is ItemKey itemKey && itemKey.TypeDefinition == TypeDefinition && itemKey.ItemId == ItemId;
    }

    /// <summary>
    /// 获取一个新的物品实例。
    /// Get a new instance of the item.
    /// </summary>
    [Pure]
    public Item GetOne()
    {
        return ItemRegistry.Create(QualifiedItemId);
    }

    /// <summary>
    /// 获取一个指定类型的新物品实例。
    /// Get a new instance of the item with the specified type.
    /// </summary>
    public T GetOne<T>() where T : Item
    {
        return ItemRegistry.Create<T>(QualifiedItemId);
    }

    /// <summary>
    /// 获取解析后的物品数据。
    /// Get the parsed data for the item.
    /// </summary>
    private ParsedItemData GetParsedData()
    {
        return ItemRegistry.GetData(QualifiedItemId);
    }

    /// <summary>
    /// 获取物品的原始数据。
    /// Get the raw data of the item.
    /// </summary>
    public T GetRawData<T>()
    {
        return (T)GetParsedData().RawData;
    }

    /// <summary>
    /// 获取物品的分类信息。
    /// Get the category information for the item.
    /// </summary>
    /// <returns>物品的 <see cref="ItemCategoryName"/> 结构。</returns>
    public ItemCategoryName GetCategory()
    {
        switch (TypeDefinition)
        {
            case "(T)":
            // 将镰刀和马笛归类为工具
            // Move scythes and horse flute to the tools category
            case "(O)" when QualifiedItemId == "(O)911":
            case "(W)" when MeleeWeapon.IsScythe(QualifiedItemId):
                return new ItemCategoryName(Object.GetCategoryDisplayName(Object.toolCategory), "Tool");

            case "(W)":
                return new ItemCategoryName(I18n.Categorize_Weapons(), "Weapons");

            case "(H)":
                return new ItemCategoryName(I18n.Categorize_Hats(), "Hats");

            case "(P)":
                return new ItemCategoryName(I18n.Categorize_Pants(), "Pants");

            case "(S)":
                return new ItemCategoryName(I18n.Categorize_Shirts(), "Shirts");

            case "(FL)":
                return new ItemCategoryName(I18n.Categorize_Flooring(), "Flooring");

            case "(WP)":
                return new ItemCategoryName(I18n.Categorize_Wallpaper(), "Wallpaper");

            case "(BC)":
                return GetOne<Object>().GetMachineData() != null
                    ? new ItemCategoryName(I18n.Categorize_Machine(), "Machine")
                    : new ItemCategoryName(I18n.Categorize_Crafting(), "Crafting");

            case "(M)":
                return new ItemCategoryName(I18n.Categorize_Mannequin(), "Mannequin");

            case "(F)":
                return new ItemCategoryName(I18n.Categorize_Furniture(), "Furniture");

            case "(TR)":
                return new ItemCategoryName(I18n.Categorize_Trinket(), "Trinket");
        }

        // 尝试使用游戏内的分类逻辑
        // Try to use the in-game category logic
        var item = GetOne();
        var categoryDisplayName = item.getCategoryName();
        var categoryBaseName = Utility.GetCategoryBaseName(item.Category);
        if (!string.IsNullOrEmpty(categoryDisplayName))
            return new ItemCategoryName(categoryDisplayName, categoryBaseName);

        // 如果物品是可食用的，将其归类为消耗品，否则归类为杂项
        // If the item is edible, categorize it as consumable; otherwise, categorize it as miscellaneous
        return TypeDefinition == "(O)" && ((Object)item).Edibility > 0
            ? new ItemCategoryName(I18n.Categorize_Consumable(), "Consumable")
            : new ItemCategoryName(I18n.Categorize_Miscellaneous(), "Miscellaneous");
    }
}