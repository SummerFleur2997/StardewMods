using StardewModdingAPI.Events;
using StardewValley.Menus;
using SummerFleursBetterHats.Framework;

namespace SummerFleursBetterHats.HatRelatedToShops;

public static partial class HatRelatedToShops
{
    public static void RegisterShopRelatedEvents() => ModEntry.ModHelper.Events.Display.MenuChanged += OnMenuChanged;

    /// <summary>
    /// Event handler for changes in game menus. Used to handle
    /// shop menu interactions with specific hats.
    /// </summary>
    private static void OnMenuChanged(object s, MenuChangedEventArgs e)
    {
        // Check if the new menu is a ShopMenu
        if (e.NewMenu is not ShopMenu shopMenu)
            return;

        int price;
        uint mask;
        Func<Item> itemGetter;

        // Determine the shop type and corresponding hat requirements
        switch (shopMenu.ShopId)
        {
            case "Raccoon" when PlayerHatIs(RaccoonHatID):
                price = 0;
                mask = RaccoonHatMask;
                itemGetter = RollRandomItemForRaccoonHat;
                break;
            case "ShadowShop" when PlayerHatIs(DarkVelvetBowID):
                price = 0;
                mask = DarkVelvetBowMask;
                itemGetter = RollRandomItemForDarkVelvetBow;
                break;
            case "Dwarf" when PlayerHatIs(WearableDwarfHelmID):
                price = 0;
                mask = WearableDwarfHelmMask;
                itemGetter = RollRandomItemForWearableDwarfHelm;
                break;
            case "DesertTrade" when PlayerHatIs(MagicTurbanID):
                price = 0;
                mask = MagicTurbanMask;
                itemGetter = RollRandomItemForMagicTurban;
                break;
            case "IslandTrade" when PlayerHatIs(BluebirdMaskID):
                price = 0;
                mask = BluebirdMaskMask;
                itemGetter = RollRandomItemForBluebirdMask;
                break;
            case "BoxOffice" when PlayerHatIs(JesterHatID):
                price = DiscountedTicketPrice;
                mask = JesterHatMask;
                itemGetter = GetDiscountedTicketForJesterHat;
                break;
            case "Traveler" when PlayerHatIs(RedFezID):
                price = 0;
                mask = RedFezMask;
                itemGetter = RollRandomItemForRedFezHat;
                break;
            case "QiGemShop" when PlayerHatIs(MrQisHatID):
                price = 0;
                mask = MrQisHatMask;
                itemGetter = RollRandomItemForMrQisHat;
                break;
            default:
                return;
        }

        // Check if player has already visited this shop in the current period
        if (Game1.player.TryGetWorldStatus(mask))
            return;

        // Generate a random item for the shop, then add the item to the shop for sale
        var item = itemGetter.Invoke();
        var actionOnPurchase = new List<string>
            { $"SFBH_{nameof(GameExtensions.ModifyWorldStatus)} {mask} {shopMenu.ShopId}" };
        var info = new ItemStockInformation(price, 1, actionsOnPurchase: actionOnPurchase);
        shopMenu.AddForSale(item, info);
    }
}