using Common;
using ConvenientChests.Framework.DataService;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace ConvenientChests.AliasForChests;

public class AliasForChestsModule : IModule
{
    /// <summary>
    /// Static singleton.
    /// </summary>
    public static readonly AliasForChestsModule Instance = new();

    /// <inheritdoc/>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Whether we should force update the info on the bubble once.
    /// Usually set to true when the alias or item icon is changed.
    /// </summary>
    public bool ForceUpdateOnce;

    /// <summary>
    /// Signs used to draw the info bubble, works with <see cref="_cachedChest"/>.
    /// </summary>
    private bool _shouldDraw;

    /// <summary>
    /// Cached chest for drawing the info bubble, works with <see cref="_shouldDraw"/>.
    /// </summary>
    private Chest? _cachedChest;

    /// <summary>
    /// Use only one readonly bubble can reduce unnecessary memory allocation.
    /// </summary>
    private readonly ChestInfoBubble _chestInfoBubble = new(Game1.smallFont);

    private AliasForChestsModule() { }

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
        // check whether the player is hovering a chest
        var tileX = (Game1.getMouseX() + Game1.viewport.X) / 64;
        var tileY = (Game1.getMouseY() + Game1.viewport.Y) / 64;
        if (Game1.currentLocation.getObjectAtTile(tileX, tileY) is not Chest chest)
            return;

        // check whether we should update the info bubble
        if (chest != _cachedChest || ForceUpdateOnce)
        {
            // cache the chest
            _cachedChest = chest;
            var data = _cachedChest.GetChestData();
            if (data.ItemIcon is null && string.IsNullOrEmpty(data.Alias))
            {
                _shouldDraw = false;
                return;
            }

            // update the info bubble
            _chestInfoBubble.Set(data.ItemIcon, data.Alias);
            _shouldDraw = true;
            ForceUpdateOnce = false;
        }

        if (!_shouldDraw)
            return;

        // position the info bubble and draw it
        var pixelPosition = chest.getLocalPosition(Game1.viewport);
        _chestInfoBubble.UpdatePosition(pixelPosition);
        _chestInfoBubble.Draw(e.SpriteBatch);
    }
}