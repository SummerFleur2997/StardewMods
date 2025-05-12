using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ItemService;
using StardewValley;
using StardewValley.Tools;

namespace ConvenientChests.CategorizeChests.Framework;

internal class CategoryDataManager
{
    /// <summary>
    /// A mapping of category names to the item keys belonging to that category.
    /// </summary>
    public Dictionary<ItemCategoryName, IList<ItemKey>> Categories { get; } = CreateCategoriesByDisplayName();

    public List<ItemCategoryName> ItemCategories => NewCategories();

    private List<ItemCategoryName> NewCategories()
    {
        var itemCategories = Categories.Keys.ToList();

        return itemCategories;
    }

    private static Dictionary<ItemCategoryName, IList<ItemKey>> CreateCategoriesByDisplayName()
    {
        return DiscoverItems()
            .Select(item => item.ToItemKey())
            .Where(key => !CategoryItemBlacklist.Includes(key))
            .GroupBy(key => key.GetCategory())
            .ToDictionary(
                g => g.Key,
                g => (IList<ItemKey>)g.ToList()
            );
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