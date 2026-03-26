using StardewValley.Objects;

namespace ConvenientChests.API;

/// <summary>
/// SummerFleur.ConvenientChests API, for Version 2.0.0 or later.
/// </summary>
/// <remarks>
/// Note: In the version 2.0.0 or higher, this mod use the <see cref="Chest.modData"/> dictionary
/// instead of separate mod data manager to manage mod data. But although you can directly access
/// them by that dictionary, it's not recommended to do so because in singleplayer mode, the data
/// will only be updated when the day is ended. And it will not work at all in case of the player
/// is using a snapshot.Use this API to get the data will ensure the data you get is accurate.
/// </remarks>
public interface IConvenientChestAPI
{
    /// <summary>
    /// Get whether the given chest accepts the given item.
    /// </summary>
    /// <returns>
    /// true if the chest accepts the item, false otherwise.
    /// </returns>
    public bool ChestAcceptThisItem(Chest chest, Item item);

    /// <summary>
    /// Get the accepted item lists of the given chest.
    /// </summary>
    /// <returns>
    /// A list of <see cref="StardewValley.Item.QualifiedItemId"/>.
    /// </returns>
    public List<string> GetSelectedChestData(Chest chest);

    /// <summary>
    /// Get the accepted item lists of all chests.
    /// </summary>
    /// <returns>
    /// A dictionary formated like
    /// {<see cref="Chest"/> -> List[<see cref="Item.QualifiedItemId"/>]}
    /// </returns>
    public Dictionary<Chest, List<string>> GetAllChestData();

    /// <summary>
    /// Another version of <see cref="GetAllChestData"/> but returns
    /// a dictionary with string keys instead of <see cref="Chest"/>.
    /// </summary>
    /// <returns>
    /// A dictionary formated like
    /// {<see cref="string"/> -> List[<see cref="Item.QualifiedItemId"/>]}
    /// </returns>
    /// <remarks> The string key's format like this:
    /// "<see cref="GameLocation.NameOrUniqueName"/> X Y" (split by white space).
    /// </remarks>
    public Dictionary<string, List<string>> GetAllChestDataWithStringFormat();
}