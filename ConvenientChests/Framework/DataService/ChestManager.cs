using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.MigrateService;
using StardewValley.Objects;

namespace ConvenientChests.Framework.DataService;

/// <summary>
/// The chest manager responsible for handling chest data.
/// 负责处理箱子数据的箱子管理器。
/// </summary>
internal static class ChestManager
{
    private static readonly Dictionary<Chest, ChestData> Table = new();
    private static readonly object Lock = new();

    public static void ModifyChest(ChestAddress chestAddress, string? itemKey)
    {
        lock (Lock)
        {
            if (!chestAddress.GetChestByAddress(out var chest, out var error))
            {
                ModEntry.Log(error, LogLevel.Error);
                return;
            }

            var data = chest.GetChestData();
            if (itemKey != null) data.Toggle(itemKey);
        }
    }

    /// <summary>
    /// Gets the <see cref="ChestData"/> for the specified chest.
    /// 获取指定箱子的 <see cref="ChestData"/> 数据。
    /// </summary>
    internal static ChestData GetChestData(this Chest chest)
    {
        // Lazy load the chest data
        if (Table.TryGetValue(chest, out var data))
            return data;

        var id = ModEntry.Manifest.UniqueID;
        var acceptedItems = chest.modData.TryGetValue($"{id}.acceptedItems", out var k)
            ? k.Split(',')
            : Array.Empty<string>();
        var snapshot = chest.modData.TryGetValue($"{id}.snapshot", out var s) && long.TryParse(s, out var snapshotId)
            ? SnapshotManager.GetValueOrDefault(snapshotId)
            : null;

        var chestData = new ChestData(chest, acceptedItems, snapshot);
        Table.Add(chest, chestData);
        return chestData;
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