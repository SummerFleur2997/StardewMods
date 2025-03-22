using System.Diagnostics.Contracts;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ConvenientChests.Framework.CategorizeChests.Framework {
    public readonly struct ItemKey {
        public string ItemId { get; }
        public string TypeDefinition { get; }

        public string QualifiedItemId => $"{TypeDefinition}{ItemId}";

        public ItemKey(string typeDefinition, string itemId) {
            TypeDefinition = typeDefinition;
            ItemId = itemId;
        }

        public ItemKey(string qualifiedItemId) {
            var item = ItemRegistry.Create(qualifiedItemId);
            TypeDefinition = item.TypeDefinitionId;
            ItemId = item.ItemId;
        }

        public override int GetHashCode() => QualifiedItemId.GetHashCode();
        public override string ToString() => QualifiedItemId;

        public override bool Equals(object obj)
            => obj is ItemKey itemKey &&
               itemKey.TypeDefinition == TypeDefinition &&
               itemKey.ItemId == ItemId;

        [Pure]
        public Item GetOne() => ItemRegistry.Create(QualifiedItemId);
        public T GetOne<T>() where T : Item => ItemRegistry.Create<T>(QualifiedItemId);

        public ParsedItemData GetParsedData() => ItemRegistry.GetData(QualifiedItemId);
        public T GetRawData<T>() => (T) GetParsedData().RawData;

        public string GetCategory() {
            switch (TypeDefinition) {
                case "(T)":
                case "(W)" when MeleeWeapon.IsScythe(QualifiedItemId): // move scythes to tools
                    return Object.GetCategoryDisplayName(Object.toolCategory);

                case "(W)":
                    return I18n.Categorize_Weapons();

                case "(H)":
                    return I18n.Categorize_Hats();

                case "(P)":
                    return I18n.Categorize_Pants();

                case "(S)":
                    return I18n.Categorize_Shirts();

                case "(FL)":
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13203"); // Flooring

                case "(WP)":
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13204"); // Wallpaper

                case "(BC)":
                    return GetOne<Object>().GetMachineData() != null
                               ? I18n.Categorize_Machine()
                               : Game1.content.LoadString(@"Strings\StringsFromCSFiles:Object.cs.12863"); // Crafting

                case "(M)":
                    return I18n.Categorize_Mannequin();

                case "(F)":
                    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12847"); // Furniture
            }

            // try to use the in-game category logic
            var item         = GetOne();
            var categoryName = item.getCategoryName();
            if (!string.IsNullOrEmpty(categoryName))
                return categoryName;

            return TypeDefinition == "(O)" && ((Object) item).Edibility > 0
                       ? I18n.Categorize_Consumable()
                       : I18n.Categorize_Miscellaneous();
        }
    }
}