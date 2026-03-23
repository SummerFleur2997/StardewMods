using Common;
using ConvenientChests.Framework.DataService;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace ConvenientChests.AliasForChests;

public class AliasForChestsModule : IModule
{
    /// <inheritdoc/>
    public bool IsActive { get; private set; }

    public bool ForceUpdateOnce;
    private ChestInfoBubble ChestInfoBubble = new(Game1.smallFont);

    private bool _shouldDraw;
    private Chest? _cachedChest;

    /// <inheritdoc/>
    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.Display.RenderedWorld += RenderChestInfoBubble;
    }

    /// <inheritdoc/>
    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Display.RenderedWorld -= RenderChestInfoBubble;
    }

    /// <summary>
    /// When move the mouse cursor above a chest, draw a bubble to show
    /// its alias and item icon.
    /// </summary>
    private void RenderChestInfoBubble(object? sender, RenderedWorldEventArgs e)
    {
        var tileX = (Game1.getMouseX() + Game1.viewport.X) / 64;
        var tileY = (Game1.getMouseY() + Game1.viewport.Y) / 64;
        if (Game1.currentLocation.getObjectAtTile(tileX, tileY) is not Chest chest)
            return;

        if (chest != _cachedChest || ForceUpdateOnce)
        {
            _cachedChest = chest;
            var data = _cachedChest.GetChestData();
            if (data.ItemIcon is null && string.IsNullOrEmpty(data.Alias))
            {
                _shouldDraw = false;
                return;
            }

            ChestInfoBubble.Set(data.ItemIcon, data.Alias);
            _shouldDraw = true;
            ForceUpdateOnce = false;
        }

        if (!_shouldDraw)
            return;

        var pixelPosition = chest.getLocalPosition(Game1.viewport);
        ChestInfoBubble.UpdatePosition(pixelPosition);
        ChestInfoBubble.Draw(e.SpriteBatch);
    }
}