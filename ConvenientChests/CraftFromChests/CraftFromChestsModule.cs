using Common;
using ConvenientChests.CraftFromChests.Framework;
using ConvenientChests.Framework.Extensions;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Objects;
using Utility = StardewValley.Utility;

namespace ConvenientChests.CraftFromChests;

internal class CraftFromChestsModule : IModule
{
    /// <summary>
    /// Static singleton.
    /// </summary>
    public static readonly CraftFromChestsModule Instance = new();

    /// <inheritdoc />
    public bool IsActive { get; private set; }

    private readonly MenuListener _menuListener = new();

    private CraftFromChestsModule() { }

    /// <inheritdoc />
    public void Activate()
    {
        IsActive = true;

        // Register Events
        _menuListener.RegisterEvents();
        _menuListener.CraftingMenuShown += CraftingMenuShown;
    }

    /// <inheritdoc />
    public void Deactivate()
    {
        IsActive = false;

        // Unregister Events
        _menuListener.CraftingMenuShown -= CraftingMenuShown;
        _menuListener.UnregisterEvents();
    }

    private void CraftingMenuShown(object? sender, CraftingMenuArgs e)
    {
        var page = e.Page;
        if (page == null!)
            return;

        // Find nearby chests
        var nearbyChests = GetChests(e.IsCookingPage).ToList();
        if (!nearbyChests.Any())
            return;

        // Add them as material containers to current CraftingPage
        var inventories = nearbyChests.Select(chest => chest.Items as IInventory);

        if (page._materialContainers == null)
            page._materialContainers = inventories.ToList();

        else
        {
            foreach (var inv in inventories.ToList())
                if (!page._materialContainers.Contains(inv))
                    page._materialContainers.Add(inv);
        }
    }

    /// <summary>
    /// Find nearby chests.
    /// </summary>
    /// <param name="isCookingScreen">Whether the context is cooking menu.</param>
    /// <returns>All nearby chests.</returns>
    private IEnumerable<Chest> GetChests(bool isCookingScreen)
    {
        // nearby chests
        var chests = Game1.player.GetNearbyChests(ModEntry.Config.CraftRadius).Where(c => c.Items.Any(i => i != null))
            .ToList();
        foreach (var c in chests)
            yield return c;

        // always add home fridge when on cooking screen
        if (!isCookingScreen)
            yield break;

        var house = Game1.player.currentLocation as FarmHouse ?? Utility.getHomeOfFarmer(Game1.player);
        if (house == null || house.upgradeLevel == 0)
            yield break;

        var fridge = house.fridge.Value;
        if (!chests.Contains(fridge))
            yield return fridge;
    }
}