using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConvenientChests.Framework.ItemService;
using StardewValley;
using StardewValley.Locations;

namespace ConvenientChests.Framework.InventoryService;

/// <summary>
/// The inventory manager responsible for handling inventory data.
/// 负责处理背包数据的背包管理器。
/// </summary>
internal static class InventoryManager
{
    private static readonly ConditionalWeakTable<FarmHouse, InventoryData> Table = new();
    private static readonly object Lock = new();

    public static void ModifyInventory(long playerID, ItemKey itemKey)
    {
        lock (Lock)
        {
            var data = GetPlayerByID(playerID).GetInventoryData();
            data.Toggle(itemKey, true);
        }
    }

    public static InventoryData GetInventoryData(this Farmer player)
    {
        return Table.GetValue(Utility.getHomeOfFarmer(player), p => new InventoryData(p));
    }

    public static List<ItemKey> GetBackpackItems(this Farmer player)
    {
        var itemKeyList = new List<ItemKey>();
        foreach (var item in player.Items)
        {
            if (item is Tool or null) continue;
            itemKeyList.Add(new ItemKey(item));
        }

        return itemKeyList.Distinct().ToList();
    }

    /// <summary>
    /// Clear ConditionalWeakTable
    /// 清理 ConditionalWeakTable
    /// </summary>
    public static void ClearInventoryData()
    {
        Table.Clear();
    }

    public static Farmer GetPlayerByID(long playerID)
    {
        var player = Game1.getAllFarmers()
            .FirstOrDefault(p => p.UniqueMultiplayerID == playerID);

        return player;
    }

}