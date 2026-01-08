using JetBrains.Annotations;
using StardewValley.Objects;

namespace BetterHatsAPI.Framework;

public static class HatDataHelper
{
    /// <summary>
    /// A dictionary of all hat data. The format is
    /// <see cref="StardewValley.Item.QualifiedItemId"/> -> List[<see cref="HatData"/>].
    /// </summary>
    private static Dictionary<string, List<HatData>> AllHatData { get; set; }

    /// <summary>
    /// Gets the <see cref="HatData"/> of the specified hat.
    /// 获取指定帽子的 <see cref="HatData"/> 数据。
    /// </summary>
    [NotNull]
    public static List<HatData> GetHatData(this Hat hat) =>
        AllHatData.TryGetValue(hat.QualifiedItemId, out var data) ? data : new List<HatData>();

    /// <summary>
    /// Initialize the hat data from content packs when the mod is loaded.
    /// </summary>
    public static void Initialize()
    {
        var log = ModEntry.Log;
        log("Loading content packs and local data files.");

        var allHatData = new Dictionary<string, List<HatData>>();

        /*****
         * Some logic below is copied from Esca-MMC's FTM mod, purported under the MIT license.
         ****/
        foreach (var pack in ModEntry.ModHelper.ContentPacks.GetOwned())
        {
            log($"Loading content pack: {pack.Manifest.UniqueID}");
            Dictionary<string, HatData> data;
            try
            {
                // load the content pack's farm config (null if it doesn't exist)
                data = pack.ReadJsonFile<Dictionary<string, HatData>>("content.json");
            }
            catch (Exception ex)
            {
                log($"Warning: This content pack could not be parsed correctly: {pack.Manifest.Name}", LogLevel.Warn);
                log("Please edit the content.json file or reinstall the content pack. ", LogLevel.Warn);
                log("The auto-generated error message is displayed below:", LogLevel.Warn);
                log("----------", LogLevel.Warn);
                log($"{ex.Message}", LogLevel.Warn);
                continue; // skip to the next content pack
            }

            // no config file found for this farm
            if (data == null)
            {
                log($"Warning: The content.json file for this content pack could not be found: {pack.Manifest.Name}",
                    LogLevel.Warn);
                log("Please reinstall the content pack. If you are its author, " +
                    "please create a config file named content.json in the pack's main folder.", LogLevel.Warn);
                continue; // skip to the next content pack
            }

            foreach (var (key, hatInfo) in data)
            {
                // set some required fields if they are not set
                hatInfo.ContentPackID = pack.Manifest.UniqueID;
                hatInfo.ContentPackName = pack.Manifest.Name;
                if (string.IsNullOrWhiteSpace(hatInfo.UniqueBuffID))
                    hatInfo.UniqueBuffID = pack.Manifest.UniqueID;
                if (string.IsNullOrWhiteSpace(hatInfo.Description))
                    hatInfo.Description = null;

                // add the data to the dictionary
                allHatData.TryAdd(key, hatInfo);
            }
        }

        AllHatData = allHatData;
    }

    /// <summary>
    /// Try to add a HatData object to the dictionary. 
    /// </summary>
    /// <param name="dict"> The target dictionary, where the key is
    /// the item ID and the value is a list of HatData. </param>
    /// <param name="key">The unique item ID of the hat. </param>
    /// <param name="value">The HatData object to be added. </param>
    private static void TryAdd(this Dictionary<string, List<HatData>> dict, string key, HatData value)
    {
        // Check if the key already exists in the dictionary
        if (!dict.ContainsKey(key))
            dict[key] = new List<HatData>();

        // Add the value to the list corresponding to the key
        dict[key].Add(value);
    }

}