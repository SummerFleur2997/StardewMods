using ConvenientChests.CategorizeChests;
using ConvenientChests.Framework.ExceptionService;
using ConvenientChests.StashToChests;
using StardewModdingAPI;
using StardewValley;

namespace ConvenientChests.Framework.SaveService;

/// <summary>
/// The class responsible for saving and loading the mod state.
/// </summary>
internal class SaveManager
{
    private readonly ModEntry ModEntry;
    private readonly CategorizeChestsModule CategorizeModule;
    private readonly StashToChestsModule StashModule;
    private readonly ISemanticVersion Version;

    public SaveManager(ISemanticVersion version, ModEntry modEntry,
        CategorizeChestsModule categorizeModule, StashToChestsModule stashModule)
    {
        Version = version;
        ModEntry = modEntry;
        CategorizeModule = categorizeModule;
        StashModule = stashModule;
    }

    /// <summary>
    /// Generate save data and write it to the given file path.
    /// </summary>
    /// <param name="relativePath">The path of the save file relative to the mod folder.</param>
    /// <param name="saveData">The save data file relative to the mod folder.</param>
    public void Save(string relativePath, SaveData saveData = null)
    {
        var saver = new Saver(Version, CategorizeModule.ChestDataManager, StashModule.InventoryDataManager);
        saveData ??= saver.GetSerializableData();
        ModEntry.Helper.Data.WriteJsonFile(relativePath, saveData);
    }

    /// <summary>
    /// Load save data from the given file path.
    /// </summary>
    /// <param name="relativePath">The path of the save file relative to the mod folder.</param>
    public void Load(string relativePath)
    {
        var saveData = ModEntry.Helper.Data.ReadJsonFile<SaveData>(relativePath) ?? new SaveData();

        foreach (var entry in saveData.ChestEntries)
        {
            try
            {
                var chest = CategorizeModule.ChestFinder.GetChestByAddress(entry.Address);
                var chestData = CategorizeModule.ChestDataManager.GetChestData(chest);

                chestData.AcceptedItemKinds = entry.GetItemSet();
            }
            catch (InvalidSaveDataException e)
            {
                ModEntry.Monitor.Log(e.Message, LogLevel.Warn);
            }
        }

        foreach (var entry in saveData.InventoryEntries)
        {
            try
            {
                var playerName = Game1.player.Name;
                var invtyData = StashModule.InventoryDataManager.GetInventoryData(playerName);

                invtyData.LockedItemKinds = entry.GetItemSet();
            }
            catch (InvalidSaveDataException e)
            {
                ModEntry.Monitor.Log(e.Message, LogLevel.Warn);
            }
        }
    }
}