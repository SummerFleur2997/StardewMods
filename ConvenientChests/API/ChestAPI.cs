using ConvenientChests.Framework.ChestService;
using StardewValley.Objects;

namespace ConvenientChests.API;

public class ChestAPI : IConvenientChestAPI
{
    /// <inheritdoc/>
    public List<string> GetSelectedChestData(Chest chest) =>
        chest.GetChestData().AcceptedItemKinds.Select(k => k.QualifiedItemId).ToList();

    /// <inheritdoc/>
    public Dictionary<Chest, List<string>> GetAllChestData() => AllChests()
        .ToDictionary(
            c => c,
            c => c.GetChestData().AcceptedItemKinds.Select(k => k.QualifiedItemId).ToList());

    /// <inheritdoc/>
    public Dictionary<string, List<string>> GetAllChestDataWithStringFormat() => AllChests()
        .ToDictionary(
            FormatChestLocation,
            c => c.GetChestData().AcceptedItemKinds.Select(k => k.QualifiedItemId).ToList());

    /// <summary>
    /// Get all chests in all locations.
    /// </summary>
    private static List<Chest> AllChests()
    {
        var chests = new List<Chest>();
        foreach (var location in Game1.locations)
        {
            chests.AddRange(location.Objects.Values.OfType<Chest>());
            foreach (var building in location.buildings.Where(b => b.indoors.Value != null))
                chests.AddRange(building.indoors.Value.Objects.Values.OfType<Chest>());
        }

        return chests;
    }

    /// <summary>
    /// Formats a chest to a string.
    /// </summary>
    private static string FormatChestLocation(Chest chest) =>
        $"{chest.Location.NameOrUniqueName} {chest.TileLocation.X} {chest.TileLocation.Y}";
}