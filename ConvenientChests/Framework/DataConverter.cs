using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConvenientChests.Framework;

/// <summary>
/// JSON converter for <see cref="HashSet{T}"/> of <see cref="Item.QualifiedItemId"/>.
/// </summary>
internal class DataConverter : JsonConverter<HashSet<string>>
{
    public override HashSet<string> ReadJson(JsonReader r, Type t, HashSet<string>? ev, bool h, JsonSerializer s)
    {
        try
        {
            var token = JToken.Load(r);
            if (token.Type == JTokenType.Array)
                return token.ToObject<HashSet<string>>() ?? new HashSet<string>();

            if (token.Type != JTokenType.Object)
                return new HashSet<string>();

            var acceptedItemDict = token.ToObject<Dictionary<string, string>>(s);
            return acceptedItemDict.ToItemIds();
        }
        catch (Exception ex)
        {
            ModEntry.Log($"An error occured while deserializing accepted items: {ex}", LogLevel.Error);
            return new HashSet<string>();
        }
    }

    public override void WriteJson(JsonWriter writer, HashSet<string>? value, JsonSerializer serializer) => serializer.Serialize(writer, value);
}

internal static class DcUtilities
{
    public static HashSet<string> ToItemIds(this Dictionary<string, string>? acceptedItems) =>
        acceptedItems?
            .Select(kvp => new KeyValuePair<string, IEnumerable<string>>(kvp.Key, kvp.Value.Split(',')))
            .SelectMany(kvp => kvp.Value.Select(itemId => kvp.Key.GetTypeDefinition() + itemId))
            .ToHashSet() ?? new HashSet<string>();

    private static string GetTypeDefinition(this string type) =>
        type switch
        {
            "Boots" => "(B)",
            "BigCraftable" => "(BC)",
            "Furniture" => "(F)",
            "Flooring" => "(FL)",
            "Hat" => "(H)",
            "Shirt" => "(S)",
            "Pants" => "(P)",
            "Mannequin" => "(M)",
            "Tool" => "(T)",
            "Wallpaper" => "(WP)",
            "Weapon" => "(W)",
            "Trinket" => "(TR)",
            "Fish" => "(O)",
            "Ring" => "(O)",
            "Object" => "(O)",
            "Gate" => "(O)",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}