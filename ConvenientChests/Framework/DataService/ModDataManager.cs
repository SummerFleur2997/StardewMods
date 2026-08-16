namespace ConvenientChests.Framework.DataService;

/// <summary>
/// A static class that provides methods for reading and writing mod data for objects that implement IHaveModData interface.
/// It includes constants for various data keys and extension methods for different data types.
/// </summary>
public static class ModDataManager
{
    public const string AliasKey = "SummerFleur.ConvenientChests.Alias";
    public const string ItemIconKey = "SummerFleur.ConvenientChests.ItemIcon";
    public const string AcceptedItemsKey = "SummerFleur.ConvenientChests.AcceptedItems";
    public const string SnapshotKey = "SummerFleur.ConvenientChests.Snapshot";
    public const string LockedFlag = "SummerFleur.ConvenientChests.Locked";

    /// <summary>
    /// Reads mod data as a string value.
    /// </summary>
    /// <param name="which">The object implementing IHaveModData interface.</param>
    /// <param name="key">The key of the mod data to read.</param>
    /// <returns>The string value of the mod data, or null if not found.</returns>
    public static string? ReadModData(this IHaveModData which, string key) =>
        which.modData.TryGetValue(key, out var value) ? value : null;

    /// <summary>
    /// Writes mod data as a string value.
    /// </summary>
    /// <param name="which">The object implementing IHaveModData interface.</param>
    /// <param name="key">The key of the mod data to write.</param>
    /// <param name="value">The value to write. If null, the key will be removed.</param>
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

    /// <summary>
    /// Reads mod data as a 64-bit integer value.
    /// </summary>
    /// <param name="which">The object implementing IHaveModData interface.</param>
    /// <param name="key">The key of the mod data to read.</param>
    /// <returns>The 64-bit integer value of the mod data, or null if not found or invalid.</returns>
    public static long? ReadModDataAsInt64(this IHaveModData which, string key)
    {
        if (which.modData.TryGetValue(key, out var value) && long.TryParse(value, out var int64))
        {
            return int64;
        }

        return null;
    }

    /// <summary>
    /// Reads mod data as an Item object.
    /// </summary>
    /// <param name="which">The object implementing IHaveModData interface.</param>
    /// <param name="key">The key of the mod data to read.</param>
    /// <returns>The Item object, or null if not found or invalid.</returns>
    public static Item? ReadModDataAsItem(this IHaveModData which, string key)
    {
        if (!which.modData.TryGetValue(key, out var value))
            return null;

        var item = ItemRegistry.Create(value);
        if (item.Name == Item.ErrorItemName)
            return null;

        return item;
    }

    /// <summary>
    /// Reads mod data as an enumerable collection of strings.
    /// </summary>
    /// <param name="which">The object implementing IHaveModData interface.</param>
    /// <param name="key">The key of the mod data to read.</param>
    /// <returns>An enumerable collection of strings, or empty if not found.</returns>
    public static IEnumerable<string> ReadModDataAsEnumerable(this IHaveModData which, string key)
    {
        if (which.modData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            foreach (var v in value.Split(','))
                yield return v.Trim();
        }
    }

    /// <summary>
    /// Writes an enumerable collection of values to mod data as a comma-separated string.
    /// </summary>
    /// <param name="which">The object implementing IHaveModData interface.</param>
    /// <param name="key">The key of the mod data to write.</param>
    /// <param name="values">The enumerable collection of values to write.</param>
    public static void WriteModDataAsEnumerable(this IHaveModData which, string key, IEnumerable<object> values) =>
        which.WriteModData(key, string.Join(",", values.Select(v => v.ToString())));

    /// <summary>
    /// Reads mod data as a boolean value.
    /// </summary>
    /// <param name="which">The object implementing IHaveModData interface.</param>
    /// <param name="key">The key of the mod data to read.</param>
    /// <returns>True if the value exists and is not whitespace, otherwise false.</returns>
    public static bool ReadModDataAsBoolean(this IHaveModData which, string key) =>
        which.modData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Writes mod data as a boolean value.
    /// </summary>
    /// <param name="which">The object implementing IHaveModData interface.</param>
    /// <param name="key">The key of the mod data to write.</param>
    /// <param name="value">The boolean value to write. True will be stored as "t", false will remove the key.</param>
    public static void WriteModDataAsBoolean(this IHaveModData which, string key, bool value) =>
        which.WriteModData(key, value ? "t" : null);
}