#nullable disable
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using StardewValley.Tools;

namespace ConvenientChests.CategorizeChests.Framework;

internal static class CategoryDataManager
{
    /// <summary>
    /// A mapping of category names to the item keys belonging to that category.
    /// </summary>
    public static readonly Dictionary<ItemCategoryName, List<ItemKey>> Categories;

    public static readonly List<ItemCategoryName> ItemCategories;

    static CategoryDataManager()
    {
        Categories = DiscoverItems()
            .Select(item => item.ToItemKey())
            .Where(key => !CategoryItemBlacklist.Includes(key))
            .GroupBy(key => key.GetCategory())
            .ToDictionary(
                g => g.Key,
                g => g.ToList()
            );
        ItemCategories = Categories.Keys.ToList();

        foreach (var category in ItemCategories)
        {
            var items = string.Join(", ", Categories[category]);
            ModEntry.Log($"Registered ItemCategory {category.BaseName}: [{items}]");
        }
    }

    public static List<ItemCategoryName> GetCategories()
    {
        List<ItemCategoryName> categories;

        // 根据配置文件决定列表排序方式
        // Determine list sorting method based on configuration settings
        if (ModEntry.Config.EnableSort)
        {
            // 按字母顺序排序
            // Sort in alphabetical order
            categories = ItemCategories
                .OrderBy(c => c.DisplayName)
                .ToList();
        }
        else
        {
            // 定义自定义排序顺序的基准名称列表
            // Define custom sorting order using base names
            var customOrder = new List<string>
            {
                "Vegetable", "Fruit", "Flower", "Animal Product", "Artisan Goods", "Seed", "Fertilizer", "Fish",
                "Bait", "Fishing Tackle", "Forage", "Artifact", "Resource", "Mineral", "Monster Loot", "Crafting",
                "Machine", "BigCrafts", "Cooking", "Consumable", "Book", "Skill Book", "Tool", "Weapons", "Ring",
                "Trinket", "Hats", "Shirts", "Pants", "Footwear", "Mannequin", "Decor", "Wallpaper", "Flooring",
                "Trash", "Miscellaneous"
            };

            // 创建基准名称到排序索引的字典
            // Create lookup dictionary: base name -> predefined sorting index
            var orderDictionary = customOrder
                .Select((name, index) => new { name, index })
                .ToDictionary(item => item.name, item => item.index);

            // 根据自定义顺序排序
            // Sort in custom rules
            categories = ItemCategories
                .OrderBy(c => orderDictionary.GetValueOrDefault(c.BaseName, int.MaxValue))
                .ToList();
        }

        return categories;
    }

    public static ItemCategoryName CalculateMostRelevantCategory(this IEnumerable<ItemKey> acceptedItemKinds)
    {
        var category = ModEntry.Config.EnableSort
            ? ItemCategories.OrderBy(c => c.DisplayName).First()
            : ItemCategories.FirstOrDefault(c => c.BaseName == "Vegetable");

        var factor = 0.0;

        foreach (var group in acceptedItemKinds.GroupBy(key => key.GetCategory()))
        {
            var name = group.Key;
            var accepts = group.Count();

            if (!Categories.TryGetValue(name, out var itemsInCategory))
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

    /// <summary>
    /// Generate every item in the games ItemRegistry
    /// </summary>
    private static IEnumerable<Item> DiscoverItems()
    {
        return ItemRegistry.ItemTypes
            .SelectMany(ItemExtensions.GetAllItems)
            .Where(FilterTools);
    }

    private static bool FilterTools(Item item)
    {
        switch (item)
        {
            case GenericTool:
            case MeleeWeapon { ItemId: not MeleeWeapon.scytheId } m when m.isScythe():
            case Tool { UpgradeLevel: not 0 } and (Axe or Pickaxe or Hoe or FishingRod or WateringCan):
            case Tool { UpgradeLevel: > 1 }:
                return false;

            default:
                return true;
        }
    }
}