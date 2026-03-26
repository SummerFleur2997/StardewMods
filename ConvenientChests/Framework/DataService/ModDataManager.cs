namespace ConvenientChests.Framework.DataService;

public static class ModDataManager
{
    public const string AliasKey = "SummerFleur.ConvenientChests.Alias";
    public const string ItemIconKey = "SummerFleur.ConvenientChests.ItemIcon";
    public const string AcceptedItemsKey = "SummerFleur.ConvenientChests.AcceptedItems";
    public const string SnapshotKey = "SummerFleur.ConvenientChests.Snapshot";
    public const string LockedFlag = "SummerFleur.ConvenientChests.Locked";

    public static string? ReadModData(this IHaveModData which, string key) =>
        which.modData.TryGetValue(key, out var value) ? value : null;

    public static void WriteModData(this IHaveModData which, string key, object? value)
    {
        if (value is null)
        {
            which.modData.Remove(key);
        }
        else
        {
            which.modData[key] = value.ToString();
        }
    }

    public static long? ReadModDataAsInt64(this IHaveModData which, string key)
    {
        if (which.modData.TryGetValue(key, out var value) && long.TryParse(value, out var int64))
        {
            return int64;
        }

        return null;
    }

    public static Item? ReadModDataAsItem(this IHaveModData which, string key)
    {
        if (!which.modData.TryGetValue(key, out var value))
            return null;

        var item = ItemRegistry.Create(value);
        if (item.Name == Item.ErrorItemName)
            return null;

        return item;
    }

    public static IEnumerable<string> ReadModDataAsEnumerable(this IHaveModData which, string key)
    {
        if (which.modData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            foreach (var v in value.Split(','))
                yield return v.Trim();
        }
    }

    public static void WriteModDataAsEnumerable(this IHaveModData which, string key, IEnumerable<object> values) =>
        which.WriteModData(key, string.Join(",", values.Select(v => v.ToString())));

    public static bool ReadModDataAsBoolean(this IHaveModData which, string key) =>
        which.modData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value);

    public static void WriteModDataAsBoolean(this IHaveModData which, string key, bool value) =>
        which.WriteModData(key, value ? "t" : null);
}