using System.Runtime.CompilerServices;

namespace ConvenientChests.Framework.InventoryService;

public class InventoryDataManager
{
    private readonly ConditionalWeakTable<string, InventoryData> Table = new();

    public InventoryData GetInventoryData(string playerName)
    {
        return Table.GetValue(playerName, p => new InventoryData(p));
    }
}