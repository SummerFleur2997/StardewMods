using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ConvenientChests.Framework.Extensions;

internal static class ItemExtensions
{
    public static Item ConvertToItem(this string qualifiedItemId) => ItemRegistry.Create(qualifiedItemId);

    /// <summary>
    /// Get a copy of this item.
    /// </summary>
    public static Item Copy(this Item item)
    {
        return ItemRegistry.Create(item.QualifiedItemId, item.Stack, item.Quality);
    }

    public static bool LockedInInventory(this Item item) => item.ReadModDataAsBoolean(ModDataManager.LockedFlag);

    public static void ChangeLockStatus(this Item item)
    {
        var locked = item.ReadModDataAsBoolean(ModDataManager.LockedFlag);
        item.WriteModDataAsBoolean(ModDataManager.LockedFlag, !locked);
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
    /// 获取物品的分类信息。
    /// Get the category information for the item.
    /// </summary>
    /// <returns>物品的 <see cref="ItemCategoryName"/> 结构。</returns>
    public static ItemCategoryName GetCategory(this Item item)
    {
        var typeDefinition = item.TypeDefinitionId;
        switch (typeDefinition)
        {
            case ItemRegistry.type_tool:
            // Move scythes and horse flute to the tools category
            case ItemRegistry.type_object when item.QualifiedItemId == "(O)911":
            case ItemRegistry.type_weapon when MeleeWeapon.IsScythe(item.QualifiedItemId):
                return new ItemCategoryName(Object.GetCategoryDisplayName(Object.toolCategory), "Tool");

            case ItemRegistry.type_weapon:
                return new ItemCategoryName(I18n.Categorize_Weapons(), "Weapons");

            case ItemRegistry.type_hat:
                return new ItemCategoryName(I18n.Categorize_Hats(), "Hats");

            case ItemRegistry.type_pants:
                return new ItemCategoryName(LoadString("Pants_Name", "Pants"), "Pants");

            case ItemRegistry.type_shirt:
                return new ItemCategoryName(LoadString("Shirt_Name", "Shirts"), "Shirts");

            case ItemRegistry.type_floorpaper:
                return new ItemCategoryName(LoadString("Wallpaper.cs.13203"), "Flooring");

            case ItemRegistry.type_wallpaper:
                return new ItemCategoryName(LoadString("Wallpaper.cs.13204"), "Wallpaper");

            case ItemRegistry.type_bigCraftable:
                var obj = ItemRegistry.Create<Object>(item.QualifiedItemId);
                if (obj.GetMachineData() != null)
                    return new ItemCategoryName(I18n.Categorize_Machine(), "Machine");

                return CraftingRecipe.craftingRecipes.ContainsKey(item.Name)
                    ? new ItemCategoryName(I18n.Categorize_Crafting(), "Crafting")
                    : new ItemCategoryName(I18n.Categorize_BigCrafts(), "BigCrafts");
            case ItemRegistry.type_mannequin:
                return new ItemCategoryName(I18n.Categorize_Mannequin(), "Mannequin");

            case ItemRegistry.type_furniture:
                return new ItemCategoryName(LoadString("Object.cs.12847"), "Furniture");

            case ItemRegistry.type_trinket:
                return new ItemCategoryName(LoadString("Trinket", "1_6_Strings"), "Trinket");
        }

        // 尝试使用游戏内的分类逻辑
        // Try to use the in-game category logic
        var categoryDisplayName = item.getCategoryName();
        var categoryBaseName = GetCategoryBaseName(item.Category);
        if (!string.IsNullOrEmpty(categoryDisplayName))
            return new ItemCategoryName(categoryDisplayName, categoryBaseName);

        // 如果物品是可食用的，将其归类为消耗品，否则归类为杂项
        // If the item is edible, categorize it as consumable; otherwise, categorize it as miscellaneous
        return typeDefinition == ItemRegistry.type_object && ((Object)item).Edibility > 0
            ? new ItemCategoryName(I18n.Categorize_Consumable(), "Consumable")
            : new ItemCategoryName(I18n.Categorize_Miscellaneous(), "Miscellaneous");

        string LoadString(string key, string file = "StringsFromCSFiles")
            => Game1.content.LoadString($"Strings\\{file}:{key}");
    }

    private static string GetCategoryBaseName(int category)
    {
        switch (category)
        {
            case -103:
                return "Skill Book";
            case -102:
                return "Book";
            case -101:
            case -98:
                break;
            case -100:
                return "Clothes";
            case -99:
                return "Tool";
            case -97:
                return "Footwear";
            case -96:
                return "Ring";
            case -81:
                return "Forage";
            case -80:
                return "Flower";
            case -79:
                return "Fruit";
            case -78:
            case -77:
            case -76:
                break;
            case -75:
                return "Vegetable";
            case -74:
                return "Seed";
            case -28:
                return "Monster Loot";
            case -27:
            case -26:
                return "Artisan Goods";
            case -25:
            case -7:
                return "Cooking";
            case -24:
                return "Decor";
            case -22:
                return "Fishing Tackle";
            case -21:
                return "Bait";
            case -20:
                return "Trash";
            case -19:
                return "Fertilizer";
            case -18:
            case -14:
            case -6:
            case -5:
                return "Animal Product";
            case -16:
            case -15:
                return "Resource";
            case -12:
            case -2:
                return "Mineral";
            case -8:
                return "Crafting";
            case -4:
                return "Fish";
            case 0:
                return "Artifact";
        }

        return "";
    }
}