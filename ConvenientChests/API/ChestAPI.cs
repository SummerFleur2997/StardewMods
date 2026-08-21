using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.Extensions;
using StardewValley.Objects;

namespace ConvenientChests.API;

public class ChestAPI : IConvenientChestAPI
{
    /// <inheritdoc/>
    public bool ChestAcceptThisItem(Chest chest, Item item) =>
        chest.GetChestData().AcceptedItems.Any(k => k == item.QualifiedItemId);

    /// <inheritdoc/>
    public bool InventoryLockThisItem(Item item) => item.LockedInInventory();

    /// <inheritdoc/>
    public List<string> GetSelectedChestData(Chest chest) => chest.GetChestData().AcceptedItems.ToList();

    /// <inheritdoc/>
    public Dictionary<Chest, List<string>> GetAllChestData()
    {
        return ChestExtension.GetLocationChests().ToDictionary(
            c => c,
            c => c.GetChestData().AcceptedItems.ToList());
    }

    /// <inheritdoc/>
    public Dictionary<string, List<string>> GetAllChestDataWithStringFormat()
    {
        return ChestExtension.GetLocationChests().ToDictionary(
            c => $"{c.Location.NameOrUniqueName} {c.TileLocation.X} {c.TileLocation.Y}",
            c => c.GetChestData().AcceptedItems.ToList());
    }
}