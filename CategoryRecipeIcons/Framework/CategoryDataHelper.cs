using System.Collections.Generic;
using StardewValley;

namespace CategoryRecipeIcons.Framework;

public static class CategoryDataHelper
{
    public static readonly Dictionary<string, string> OverrideSpriteIndex = new();
    public static readonly Dictionary<int, string> OverrideNameIndex = new();

    public static readonly Dictionary<string, string> SpriteIndex
        = ModEntry.ModHelper.Data.ReadJsonFile<Dictionary<string, string>>("index.json");

    public static void Patch_getSpriteIndexFromRawIndex(ref string __result, string item_id)
    {
        if (item_id == null || !item_id.StartsWith('-')) return;

        __result = OverrideSpriteIndex[item_id] ?? SpriteIndex[item_id] ?? __result;
    }

    public static void Patch_getNameFromIndex(ref string __result, string item_id)
    {
        if (__result != "???" || !int.TryParse(item_id, out var id)) return;

        __result = OverrideNameIndex[id] ?? GetCategoryDisplayName(id) ?? __result;
    }

    private static string GetCategoryDisplayName(int categoryID) =>
        categoryID switch
        {
            -9 => I18n.String_BigCraftable(),
            -12 => I18n.String_GeodeMineral(),
            -14 => I18n.String_Meat(),
            -15 => I18n.String_MetalResource(),
            -16 => I18n.String_BasicResource(),
            -17 => I18n.String_RareProduct(),
            -23 => I18n.String_OtherAquaticProducts(),
            -25 => I18n.String_OtherCooking(),
            -27 => I18n.String_Syrup(),
            -95 => I18n.String_Hat(),
            -98 => I18n.String_Weapon(),
            -101 => I18n.String_Trinket(),
            _ => Object.GetCategoryDisplayName(categoryID)
        };
}