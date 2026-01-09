using StardewValley.Objects;

namespace ConvenientChests.API;

public interface IConvenientChestAPI
{
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