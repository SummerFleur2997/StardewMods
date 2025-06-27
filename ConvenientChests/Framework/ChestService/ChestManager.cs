using System.Runtime.CompilerServices;
using ConvenientChests.Framework.ItemService;
using StardewValley.Objects;

namespace ConvenientChests.Framework.ChestService;

/// <summary>
/// The chest manager responsible for handling chest data.
/// 负责处理箱子数据的箱子管理器。
/// </summary>
internal static class ChestManager
{
    private static readonly ConditionalWeakTable<Chest, ChestData> Table = new();
    private static readonly object Lock = new();

    public static void ModifyChest(ChestAddress chestAddress, ItemKey itemKey)
    {
        lock (Lock)
        {
            var chest = chestAddress.GetChestByAddress();
            var data = GetChestData(chest);
            data.Toggle(itemKey, true);
        }
    }

    /// <summary>
    /// Gets the <see cref="ChestData"/> for the specified chest.
    /// 获取指定箱子的 <see cref="ChestData"/> 数据。
    /// </summary>
    internal static ChestData GetChestData(this Chest chest)
    {
        return Table.GetValue(chest, c => new ChestData(c));
    }

    /// <summary>
    /// Clear ConditionalWeakTable
    /// 清理 ConditionalWeakTable
    /// </summary>
    public static void ClearChestData()
    {
        Table.Clear();
    }
}