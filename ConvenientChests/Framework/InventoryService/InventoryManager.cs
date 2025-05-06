using System.Linq;
using System.Runtime.CompilerServices;
using StardewValley;

namespace ConvenientChests.Framework.InventoryService;

/// <summary>
/// The inventory manager responsible for handling inventory data.
/// 负责处理背包数据的背包管理器。
/// </summary>
public class InventoryManager
{
    private readonly ConditionalWeakTable<Farmer, InventoryData> _table = new();

    public InventoryData GetInventoryData(Farmer playerName)
    {
        return _table.GetValue(playerName, p => new InventoryData(p));
    }

    public Farmer GetPlayerByID(long playerID)
    {
        
        var player = Game1.getAllFarmers()
            .FirstOrDefault(p => p.UniqueMultiplayerID == playerID);
        
        return player;
    }
}