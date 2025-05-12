using System.Linq;
using System.Runtime.CompilerServices;
using StardewValley;

namespace ConvenientChests.Framework.InventoryService;

/// <summary>
/// The inventory manager responsible for handling inventory data.
/// 负责处理背包数据的背包管理器。
/// </summary>
internal static class InventoryManager
{
    private static readonly ConditionalWeakTable<Farmer, InventoryData> Table = new();

    public static InventoryData GetInventoryData(Farmer playerName)
    {
        return Table.GetValue(playerName, _ => new InventoryData());
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