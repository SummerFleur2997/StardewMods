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
            var player = GetPlayerByID(playerID);
            var data = GetInventoryData(player);
            data.Toggle(itemKey, true);
        }
    }

    public static InventoryData GetInventoryData(Farmer player)
    {
        return Table.GetValue(Utility.getHomeOfFarmer(player), p => new InventoryData(p));
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