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
    public Dictionary<string, IList<ItemKey>> Categories { get; } = CreateCategoriesByDisplayName();

    public List<ItemCategoryName> ItemCategories => NewCategories();

    private List<ItemCategoryName> NewCategories()
    {
        var displayName = Categories.Keys.ToList();
        var baseName = CreateCategoriesByBaseName();
        if (displayName.Count != baseName.Count) return null;

        var categories = displayName
            .Zip(baseName, (dn, bn) => new ItemCategoryName(dn, bn))
            .ToList();

        return categories;
    }


    private static Dictionary<string, IList<ItemKey>> CreateCategoriesByDisplayName()
    {
        return DiscoverItems()
            .Select(item => item.ToItemKey())
            .Where(key => !CategoryItemBlacklist.Includes(key))
            .GroupBy(key => key.GetCategory().CategoryDisplayName)
            .ToDictionary(
                g => g.Key,
                g => (IList<ItemKey>)g.ToList()
            );
    }

    private static List<string> CreateCategoriesByBaseName()
    {
        return DiscoverItems()
            .Select(item => item.ToItemKey())
            .Where(key => !CategoryItemBlacklist.Includes(key))
            .GroupBy(key => key.GetCategory().CategoryBaseName)
            .Select(g => g.Key)
            .ToList();
    }

    /// <summary>
    /// Generate every item in the games ItemRegistry
    /// </summary>
    private static IEnumerable<Item> DiscoverItems()
    {
        return ItemRegistry.ItemTypes.SelectMany(ItemHelper.GetAllItems)
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