using System.Collections.Generic;
using System.Linq;
using ConvenientChests.CraftFromChests.Framework;
using ConvenientChests.Framework;
using ConvenientChests.Framework.ChestService;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Objects;
using Utility = StardewValley.Utility;

namespace ConvenientChests.CraftFromChests;

public class CraftFromChestsModule : IModule
{
    public bool IsActive { get; private set; }
    
    private readonly MenuListener _menuListener;

    public CraftFromChestsModule()
    {
        var events = ModEntry.ModHelper.Events;
        _menuListener = new MenuListener(events);
        ModEntry.ModHelper.Events.Display.MenuChanged += OnMenuChanged;
    }

    public void Activate()
    {
        IsActive = true;

        // Register Events
        _menuListener.RegisterEvents();
        _menuListener.CraftingMenuShown += CraftingMenuShown;
    }

    public void Deactivate()
    {
        IsActive = false;

        // Unregister Events
        _menuListener.CraftingMenuShown -= CraftingMenuShown;
        _menuListener.UnregisterEvents();
    }

    private void CraftingMenuShown(object sender, CraftingMenuArgs e)
    {
        var page = e.Page;
        if (page == null)
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
            page._materialContainers.AddRange(inventories);
    }

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

    private void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        ModEntry.ReloadConfig(ModEntry.Config.CraftFromChests, this);
    }
}