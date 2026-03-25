using ConvenientChests.Framework.DataStructs;

namespace ConvenientChests.Framework.DataService;

internal static class SnapshotManager
{
    /// <summary>
    /// The file name of the snapshot.
    /// </summary>
    private const string DataPath = "snapshots.json";

    /// <summary>
    /// The order of snapshots. This is used to determine the order of
    /// snapshots in the UI because the order in the dictionary is not
    /// guaranteed.
    /// </summary>
    private static List<long> _order = new();

    /// <summary>
    /// All the saved snapshot data, should be deserialized from file.
    /// </summary>
    private static Dictionary<long, ChestDataSnapshot> _snapshots = new();

    /// <summary>
    /// Swap the position of two snapshots.
    /// </summary>
    /// <param name="s1">The index of first target.</param>
    /// <param name="s2">The index of second target.</param>
    public static void Swap(int s1, int s2)
    {
        _order[s1] ^= _order[s2];
        _order[s2] ^= _order[s1];
        _order[s1] ^= _order[s2];
    }

    /// <summary>
    /// Wrapped AddItem method to the snapshot data.
    /// </summary>
    /// <param name="chestDataSnapshot">The snapshot to add.</param>
    public static void Add(ChestDataSnapshot chestDataSnapshot)
    {
        _snapshots.Add(chestDataSnapshot.UniqueID, chestDataSnapshot);
        _order.Add(chestDataSnapshot.UniqueID);
    }

    /// <summary>
    /// Wrapped DeleteItem method to the snapshot data.
    /// </summary>
    /// <param name="id">
    /// The <see cref="ChestDataSnapshot.UniqueID"/> of the snapshot.
    /// </param>
    public static void Remove(long id)
    {
        _snapshots.Remove(id);
        _order.Remove(id);
    }

    public static ChestDataSnapshot CreateNewSnapshot(string alias, IEnumerable<string>? acceptedItemKinds = null)
    {
        long id;
        do
        {
            id = Utility.RandomLong();
        } while (_snapshots.ContainsKey(id) || id == 0);

        return new ChestDataSnapshot(alias, id, acceptedItemKinds ?? new HashSet<string>());
    }

    public static string GetAValidAlias(string? alias, long id)
    {
        if (!_snapshots.Values.Any(x => x.Alias == alias && x.UniqueID != id) &&
            !string.IsNullOrWhiteSpace(alias))
        {
            return alias;
        }

        var i = 0;
        if (string.IsNullOrWhiteSpace(alias))
        {
            do
            {
                i++;
            } while (HaveADuplicatedAlias($"{I18n.UI_Unnamed()} {i}"));

            return $"{I18n.UI_Unnamed()} {i}";
        }

        i++;
        do
        {
            i++;
        } while (HaveADuplicatedAlias($"{alias} ({i})"));

        return $"{alias} ({i})";

        bool HaveADuplicatedAlias(string s) => _snapshots.Values.Any(x => x.Alias == s);
    }

    public static List<ChestDataSnapshot> GetSnapshots() => _order.Select(x => _snapshots[x]).ToList();

    /// <summary>
    /// Wrapped accessor to the snapshot data.
    /// </summary>
    public static ChestDataSnapshot? GetValueOrDefault(long id) => _snapshots.GetValueOrDefault(id);

    /// <summary>
    /// Generate snapshot data and write it to the disk.
    /// </summary>
    public static void Load()
    {
        try
        {
            _order.Clear();
            _snapshots.Clear();
            var snapshots = ModEntry.ModHelper.Data.ReadJsonFile<ChestDataSnapshot[]>(DataPath) ??
                            Array.Empty<ChestDataSnapshot>();
            _order = snapshots.Select(x => x.UniqueID).ToList();
            _snapshots = snapshots.ToDictionary(x => x.UniqueID, x => x);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to load snapshot data from {DataPath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }

    /// <summary>
    /// Generate snapshot data and write it to the disk.
    /// </summary>
    public static void Save()
    {
        try
        {
            var snapshots = _order.Select(x => _snapshots[x]).ToList();
            ModEntry.ModHelper.Data.WriteJsonFile(DataPath, snapshots);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to save snapshot data to {DataPath}", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
        }
    }
}