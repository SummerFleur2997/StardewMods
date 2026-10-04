using ConvenientChests.AliasForChests;
using ConvenientChests.Framework.DataStructs;
using StardewValley.Objects;
using static ConvenientChests.Framework.DataService.ModDataManager;

namespace ConvenientChests.Framework.DataService;

/// <summary>
/// The chest manager responsible for handling chest data.
/// 负责处理箱子数据的箱子管理器。
/// </summary>
internal static class ChestManager
{
    private static readonly Dictionary<Chest, ChestData> Table = new();

    /// <summary>
    /// Receive the chest data from other players when a chest was modified.
    /// </summary>
    /// <param name="chestAddress">The address of which chest was modified.</param>
    /// <param name="which">Which attribute need to update, 0 = AcceptItems, 1 = Alias.</param>
    public static void UpdateChest(ChestAddress chestAddress, int which)
    {
        if (!chestAddress.GetChestByAddress(out var chest, out var error))
        {
            ModEntry.Log(error, LogLevel.Error);
            return;
        }

        switch (which)
        {
            // Lazy update, mark the chest data as dirty
            case 0:
                chest.GetChestData().Dirty = true;
                ModEntry.Log($"Synced new accept item list for chest at {chestAddress}.");
                break;
            case 1:
                AliasForChestsModule.Instance.ForceUpdateOnce = true;
                ModEntry.Log("Forced update the chest alias once.");
                break;
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
        {
            if (data.Dirty)
                data.UpdateAcceptedItems();

            return data;
        }

        var acceptedItems = chest.ReadModDataAsEnumerable(AcceptedItemsKey).ToHashSet();
        var snapshotId = chest.ReadModDataAsInt64(SnapshotKey) ?? 0;
        var snapshot = SnapshotManager.GetValueOrDefault(snapshotId);

        var chestData = new ChestData(chest, acceptedItems, snapshot);
        Table.Add(chest, chestData);
        return chestData;
    }

    /// <summary>
    /// Save ChestData
    /// </summary>
    public static void SaveChestData()
    {
        foreach (var data in Table.Values)
        {
            // Pick up changes synced from other players before writing.
            if (data.Dirty)
                data.UpdateAcceptedItems();

            data.SetAcceptedItemAndWriteToModData();
        }
    }

    /// <summary>
    /// Clear ChestData.
    /// </summary>
    public static void ClearChestData()
    {
        Table.Clear();
    }
}