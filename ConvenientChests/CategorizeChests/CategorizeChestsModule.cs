using Common;
using ConvenientChests.CategorizeChests.Framework.UI;
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.DataStructs;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests;

internal class CategorizeChestsModule : IModule
{
    public bool IsActive { get; private set; }
    private ChestInfoBubble ChestInfoBubble { get; } = new();
    private Chest _cachedChest;
    private bool _shouldDraw;

    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.World.ObjectListChanged += OnChestSwapped;
        ModEntry.ModHelper.Events.Display.RenderedWorld += RenderChestInfoBubble;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.World.ObjectListChanged -= OnChestSwapped;
        ModEntry.ModHelper.Events.Display.RenderedWorld -= RenderChestInfoBubble;
    }

    private void RenderChestInfoBubble(object sender, RenderedWorldEventArgs e)
    {
        var tileX = (Game1.getMouseX() + Game1.viewport.X) / 64;
        var tileY = (Game1.getMouseY() + Game1.viewport.Y) / 64;
        if (Game1.currentLocation.getObjectAtTile(tileX, tileY) is not Chest chest)
            return;

        if (chest != _cachedChest)
        {
            _cachedChest = chest;
            var data = _cachedChest.GetChestData();
            if (data.ItemIcon is null && string.IsNullOrEmpty(data.Note))
            {
                _shouldDraw = false;
                return;
            }

            ChestInfoBubble.Item = data.ItemIcon;
            ChestInfoBubble.SetText(data.Note);
            _shouldDraw = true;
        }

        if (!_shouldDraw)
            return;

        var pixelPosition = chest.getLocalPosition(Game1.viewport);
        ChestInfoBubble.UpdatePosition(pixelPosition);
        ChestInfoBubble.Draw(e.SpriteBatch);
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
            newChest.GetChestData().MigrateDataFromOldChest(oldChest);
    }
}