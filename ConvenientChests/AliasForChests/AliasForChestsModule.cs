using Common;
using ConvenientChests.Framework.DataService;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
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
    /// Bumped whenever an alias or item icon changes, so every screen
    /// rebuilds its cached bubbles on the next frame.
    /// </summary>
    private int _version;

    /// <summary>
    /// The bubbles of each screen, so split-screen players don't
    /// overwrite each other's cache every frame.
    /// </summary>
    private readonly PerScreen<ScreenBubbles> _screens = new(() => new ScreenBubbles());

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
        _screens.ResetAllScreens();
    }

    /// <summary>
    /// Rebuild the info bubbles on every screen. Call it when the alias
    /// or item icon of a chest is changed.
    /// </summary>
    public void ForceUpdate() => _version++;

    /// <summary>
    /// Draw a bubble showing the alias and item icon of the chest under
    /// the mouse cursor, and of the chest the player is facing.
    /// </summary>
    private void RenderChestInfoBubble(object? sender, RenderedWorldEventArgs e)
    {
        var screen = _screens.Value;
        var busy = Game1.activeClickableMenu is not null || Game1.eventUp;

        // The chest under the mouse cursor. A controller's cursor only ever points at menu
        // buttons while a menu is up, so a chest behind one would show its bubble by accident.
        // Mouse players can still hover chests behind a menu.
        Chest? hovered = null;
        if (!busy || !Game1.options.gamepadControls)
        {
            var tileX = (Game1.getMouseX() + Game1.viewport.X) / 64;
            var tileY = (Game1.getMouseY() + Game1.viewport.Y) / 64;
            hovered = Game1.currentLocation.getObjectAtTile(tileX, tileY) as Chest;
            if (hovered is not null)
                DrawBubble(screen.Hovered, hovered, e.SpriteBatch);
        }

        // the chest in front of the player, which is how controller players point at things
        if (!ModEntry.Config.ShowAliasWhenFacing || busy)
            return;

        var tile = Game1.player.GetGrabTile();
        if (Game1.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y) is Chest faced && faced != hovered)
            DrawBubble(screen.Faced, faced, e.SpriteBatch);
    }

    /// <summary>
    /// Draw the bubble of a chest, rebuilding it only when the chest or its data changed.
    /// </summary>
    private void DrawBubble(CachedBubble cache, Chest chest, SpriteBatch b)
    {
        if (chest != cache.Chest || cache.Version != _version)
        {
            cache.Chest = chest;
            cache.Version = _version;

            var data = chest.GetChestData();
            cache.ShouldDraw = data.ItemIcon is not null || !string.IsNullOrEmpty(data.Alias);
            if (cache.ShouldDraw)
                cache.Bubble.Set(data.ItemIcon, data.Alias);
        }

        if (!cache.ShouldDraw)
            return;

        cache.Bubble.UpdatePosition(chest.getLocalPosition(Game1.viewport));
        cache.Bubble.Draw(b);
    }

    /// <summary>
    /// A bubble and the chest it was last built for.
    /// </summary>
    private sealed class CachedBubble
    {
        public readonly ChestInfoBubble Bubble = new(Game1.smallFont);
        public Chest? Chest;
        public int Version = -1;
        public bool ShouldDraw;
    }

    /// <summary>
    /// The bubbles of one screen.
    /// </summary>
    private sealed class ScreenBubbles
    {
        public readonly CachedBubble Hovered = new();
        public readonly CachedBubble Faced = new();
    }
}
