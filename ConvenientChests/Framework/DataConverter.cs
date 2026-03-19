#nullable enable
using ConvenientChests.Framework.DataStructs;
using ConvenientChests.Framework.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConvenientChests.Framework;

/// <summary>
/// JSON converter for <see cref="HashSet{T}"/> of <see cref="ItemKey"/>.
/// </summary>
internal class DataConverter : JsonConverter<HashSet<ItemKey>>
{
    public override HashSet<ItemKey> ReadJson(JsonReader r, Type t, HashSet<ItemKey>? ev, bool h, JsonSerializer s)
    {
        try
        {
            var token = JToken.Load(r);
            if (token.Type != JTokenType.Object)
                return new HashSet<ItemKey>();

            var acceptedItemDict = token.ToObject<Dictionary<ItemType, string>>(s);
            return acceptedItemDict.ToItemKeys();
        }
        catch (Exception ex)
        {
            ModEntry.Log($"An error occured while deserializing accepted items: {ex}", LogLevel.Error);
            return new HashSet<ItemKey>();
        }
    }

    public override void WriteJson(JsonWriter writer, HashSet<ItemKey>? value, JsonSerializer serializer)
    {
        var dict = value?
            .GroupBy(ItemExtensions.GetItemType)
            .ToDictionary(
                g => g.Key,
                g => string.Join(",", g.Select(i => i.ItemId))
            ) ?? new Dictionary<ItemType, string>();
        serializer.Serialize(writer, dict);
    }
}

internal static class DcUtilities
{
    public static HashSet<ItemKey> ToItemKeys(this Dictionary<ItemType, string>? acceptedItems) =>
        acceptedItems?
            .Select(kvp => new KeyValuePair<ItemType, IEnumerable<string>>(kvp.Key, kvp.Value.Split(',')))
            .SelectMany(kvp => kvp.Value.Select(itemId => new ItemKey(kvp.Key.GetTypeDefinition(), itemId)))
            .ToHashSet() ?? new HashSet<ItemKey>();
}