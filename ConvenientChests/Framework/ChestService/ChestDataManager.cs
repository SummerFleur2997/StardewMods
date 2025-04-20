using System.Runtime.CompilerServices;
using StardewValley.Objects;

namespace ConvenientChests.Framework.ChestService;

internal class ChestDataManager
{
    private readonly ConditionalWeakTable<Chest, ChestData> Table = new();

    public ChestData GetChestData(Chest chest)
    {
        return Table.GetValue(chest, c => new ChestData(c));
    }
}