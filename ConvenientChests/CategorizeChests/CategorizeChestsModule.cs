using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.Framework;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.ItemService;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests;

public class CategorizeChestsModule : Module
{
    internal CategoryDataManager CategoryDataManager { get; } = new();
    internal ChestManager ChestManager { get; } = new();

    public bool ChestAcceptsItem(Chest chest, Item item)
    {
        var itemKey = item.ToBase().ToItemKey();
        return !CategoryItemBlacklist.Includes(itemKey) && ChestManager.GetChestData(chest).Accepts(itemKey);
    }

    public CategorizeChestsModule(ModEntry modEntry) : base(modEntry)
    {
    }

    public override void Activate()
    {
        IsActive = true;

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.Log(
                "Due to limitations in the network code, CHEST CATEGORIES CAN NOT BE SAVED as farmhand, sorry :(",
                LogLevel.Warn);
    }

    public override void Deactivate()
    {
        IsActive = false;
    }
}