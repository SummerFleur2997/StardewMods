using StardewValley.Menus;

namespace BinThereSavedThat;

public static class ItemSaver
{
    private static ShopData[] _savedStorage = new ShopData[10];

    private static int _savedStorageIndex;

    public static void AddToSavedStorage(this Item item)
    {
        var index = _savedStorageIndex % ModEntry.Config.MaxStorageSize;
        var price = Math.Max(0, Utility.getTrashReclamationPrice(item, Game1.player));
        var info = new ItemStockInformation(price, 1);
        var kvp = new ShopData(item, info);
        _savedStorage[index] = kvp;
        _savedStorageIndex++;
    }

    public static void ClearSavedStorage()
    {
        _savedStorage = new ShopData[10];
        _savedStorageIndex = 0;
    }

    public static bool TryToCreateShopMenu(out ShopMenu shopMenu)
    {
        var items = new Dictionary<ISalable, ItemStockInformation>();
        for (var j = 1; j <= ModEntry.Config.MaxStorageSize; j++)
        {
            var index = _savedStorageIndex - j;
            if (index < 0) break;
            var data = _savedStorage[index % ModEntry.Config.MaxStorageSize];
            if (data != null && data.StockInfo.Stock > 0) items.Add(data.Item, data.StockInfo);
        }

        shopMenu = new ShopMenu("BTST.ItemRecallShop", items, on_purchase: OnPurchase);
        return items.Any();
    }

    private static bool OnPurchase(ISalable salable, Farmer who, int countTaken, ItemStockInformation stock)
    {
        if (salable is not Item item) return false;
        var price = (uint)Math.Max(0, Utility.getTrashReclamationPrice(item, who));
        who.totalMoneyEarned -= price;
        return false;
    }

    private class ShopData
    {
        public readonly ISalable Item;
        public readonly ItemStockInformation StockInfo;

        public ShopData(ISalable item, ItemStockInformation stockInfo)
        {
            Item = item;
            StockInfo = stockInfo;
        }
    }
}