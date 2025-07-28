using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ItemService;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace ConvenientChests.CategorizeChests.Framework;

internal static class CategoryDataManager
{
    /// <summary>
    /// A mapping of category names to the item keys belonging to that category.
    /// </summary>
    public static Dictionary<ItemCategoryName, IList<ItemKey>> Categories;

    public static List<ItemCategoryName> ItemCategories;

    public static void Initialize()
    {
        Categories = DiscoverItems()
            .Select(item => item.ToItemKey())
            .Where(key => !CategoryItemBlacklist.Includes(key))
            .GroupBy(key => key.GetCategory())
            .ToDictionary(
                g => g.Key,
                g => (IList<ItemKey>)g.ToList()
            );
        ItemCategories = Categories.Keys.ToList();

        foreach (var category in ItemCategories)
        {
            var items = string.Join(", ", Categories[category]);
            ModEntry.Log($"Registed ItemCategory {category.CategoryBaseName}: [{items}]", LogLevel.Debug);
        }
    }

    /// <summary>
    /// Generate every item in the games ItemRegistry
    /// </summary>
    private static IEnumerable<Item> DiscoverItems()
    {
        return ItemRegistry.ItemTypes
            .SelectMany(ItemHelper.GetAllItems)
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