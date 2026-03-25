using ConvenientChests.Framework.DataStructs;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ConvenientChests.Framework.Extensions;

internal static class ItemExtensions
{
    /// <summary>
    /// Whether the item is a craftable item
    /// </summary>
    public static bool IsCraftable(this Item item)
    {
        return CraftingRecipe.craftingRecipes.ContainsKey(item.Name);
    }

    /// <summary>
    /// Get a copy of this item.
    /// </summary>
    public static Item Copy(this Item item)
    {
        return ItemRegistry.Create(item.QualifiedItemId, item.Stack, item.Quality);
    }

    public static bool LockedInInventory(this Item item) => item.modData.TryGetValue(ModEntry.Manifest.UniqueID, out _);

    public static void ChangeLockStatus(this Item item)
    {
        var id = ModEntry.Manifest.UniqueID;
        var locked = item.modData.ContainsKey(id);

        if (locked) item.modData.Remove(id);
        else item.modData[id] = "t";
    }

    /// <summary>
    /// Used for tools and scythe, returns the base item.
    /// </summary>
    public static Item ToBase(this Item item)
    {
        return item switch
        {
            MeleeWeapon m when m.isScythe() => new MeleeWeapon(MeleeWeapon.scytheId),
            Axe => new Axe(),
            FishingRod => new FishingRod(),
            Hoe => new Hoe(),
            Pickaxe => new Pickaxe(),
            WateringCan => new WateringCan(),
            _ => item
        };
    }

    /// <summary>
    /// Convert the item to an item key.
    /// </summary>
    public static ItemKey ToItemKey(this Item item)
    {
        return new ItemKey(item.TypeDefinitionId, item.ItemId);
    }

    /// <summary>
    /// Get all registered items in game.
    /// </summary>
    public static IEnumerable<Item> GetAllItems(this IItemDataDefinition registry)
    {
        return registry.GetAllData().Select(registry.CreateItem);
    }

    public static ItemType GetItemType(this ItemKey key)
    {
        return key.TypeDefinition switch
        {
            "(B)" => ItemType.Boots,
            "(BC)" => ItemType.BigCraftable,
            "(F)" => ItemType.Furniture,
            "(FL)" => ItemType.Flooring,
            "(H)" => ItemType.Hat,
            "(S)" => ItemType.Shirt,
            "(P)" => ItemType.Pants,
            "(M)" => ItemType.Mannequin,
            "(T)" => ItemType.Tool,
            "(WP)" => ItemType.Wallpaper,
            "(W)" => ItemType.Weapon,
            "(TR)" => ItemType.Trinket,
            "(O)" when key.ItemId == "325" => ItemType.Gate,
            "(O)" => key.GetOne().Category
                switch
                {
                    Object.FishCategory => ItemType.Fish,
                    Object.ringCategory => ItemType.Ring,
                    _ => ItemType.Object
                },

            _ => throw new ArgumentOutOfRangeException(nameof(key), key.TypeDefinition)
        };
    }


    public static string GetTypeDefinition(this ItemType type)
    {
        return type
            switch
            {
                ItemType.Boots => "(B)",
                ItemType.BigCraftable => "(BC)",
                ItemType.Furniture => "(F)",
                ItemType.Flooring => "(FL)",
                ItemType.Hat => "(H)",
                ItemType.Shirt => "(S)",
                ItemType.Pants => "(P)",
                ItemType.Mannequin => "(M)",
                ItemType.Tool => "(T)",
                ItemType.Wallpaper => "(WP)",
                ItemType.Weapon => "(W)",
                ItemType.Trinket => "(TR)",
                ItemType.Fish => "(O)",
                ItemType.Ring => "(O)",
                ItemType.Object => "(O)",
                ItemType.Gate => "(O)",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
    }
}

internal enum ItemType
{
    BigCraftable,
    Boots,
    Fish,
    Flooring,
    Furniture,
    Hat,
    Object,
    Ring,
    Tool,
    Wallpaper,
    Weapon,
    Gate,
    Trinket,
    Shirt,
    Pants,
    Mannequin
}