using System.Linq;
using Common;
using ConvenientChests.Framework.ChestService;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests;

internal class CategorizeChestsModule : IModule
{
    public bool IsActive { get; private set; }

    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.World.ObjectListChanged += OnChestSwapped;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.World.ObjectListChanged -= OnChestSwapped;
    }

    /// <summary>
    /// 当手持一个新的箱子替换现有的箱子时，传递 <see cref="ChestData"/>。
    /// Transfer <see cref="ChestData"/> when use a new chest to swap the old chest.
    /// </summary>
    private static void OnChestSwapped(object sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation || !e.Removed.Any() || !e.Added.Any()) return;

        var oldChest = e.Removed
            .Select(kvp => kvp.Value)
            .OfType<Chest>()
            .FirstOrDefault();

        var newChest = e.Added
            .Select(kvp => kvp.Value)
            .OfType<Chest>()
            .FirstOrDefault();

        if (oldChest is null || newChest is null) return;
        if (newChest.TileLocation == oldChest.TileLocation)
            newChest.GetChestData().AcceptedItemKinds = oldChest.GetChestData().AcceptedItemKinds;
    }
}