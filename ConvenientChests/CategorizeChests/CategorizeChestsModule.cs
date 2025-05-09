using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.Framework;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.ItemService;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests;

public class CategorizeChestsModule : IModule
{
    public bool IsActive { get; private set; }
    internal CategoryDataManager CategoryDataManager { get; } = new();
    internal ChestManager ChestManager { get; } = new();
    public IModEvents Events => ModEntry.ModHelper.Events;

    public bool ChestAcceptsItem(Chest chest, Item item)
    {
        var itemKey = item.ToBase().ToItemKey();
        return !CategoryItemBlacklist.Includes(itemKey) && ChestManager.GetChestData(chest).Accepts(itemKey);
    }

    public void Activate()
    {
        IsActive = true;

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.Log(
                "Due to limitations in the network code, CHEST CATEGORIES CAN NOT BE SAVED as farmhand, sorry :(",
                LogLevel.Warn);
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}