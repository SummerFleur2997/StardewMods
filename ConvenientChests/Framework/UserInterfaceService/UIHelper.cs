using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;
using UI.Sprite;

namespace ConvenientChests.Framework.UserInterfaceService;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
internal static class UIHelper
{
    public static Texture2D Cursors => Game1.mouseCursors;
    public static Texture2D Cursors_1_6 => Game1.mouseCursors_1_6;

    public static readonly Texture2D Texture =
        ModEntry.ModHelper.ModContent.Load<Texture2D>(Path.Combine("assets", "texture.png"));

    public static int YOffset;

    public static void Initialize()
    {
        var themes = ModEntry.ModHelper.ModContent.Load<ThemeInfo[]>(Path.Combine("assets", "themes.json"));

        foreach (var theme in themes)
        {
            foreach (var contentPack in theme.For)
            {
                if (!ModEntry.ModHelper.ModRegistry.IsLoaded(contentPack))
                    continue;

                YOffset = theme.YOffset;
                ModEntry.Log($"{contentPack} detected, use the theme {theme.Name} for the mod UI.", LogLevel.Info);
                return;
            }
        }
    }

    /// <summary>
    /// Create a button with a sprite.
    /// </summary>
    /// <param name="x">The left position of the component in pixels.</param>
    /// <param name="y">The top position of the component in pixels.</param>
    /// <param name="variant">The variant sprite of this button.</param>
    public static SpriteButton SideButton(int x, int y, SideButtonVariant variant)
    {
        var xOffset = (int)variant * 16;
        var texture = new TextureRegion(Texture, xOffset, YOffset, 16, 16);
        var button = new SpriteButton(texture, x, y);
        button.Tooltip = GetTooltipForSideButton(variant);
        return button;
    }

    public static NineSlice LightButtonBackground(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors, 405, 373, 1, 2, true),
        new TextureRegion(Cursors, 403, 375, 2, 1, true),
        new TextureRegion(Cursors, 410, 379, 2, 1, true),
        new TextureRegion(Cursors, 409, 380, 1, 2, true),
        new TextureRegion(Cursors, 403, 373, 2, 2, true),
        new TextureRegion(Cursors, 410, 373, 2, 2, true),
        new TextureRegion(Cursors, 403, 380, 2, 2, true),
        new TextureRegion(Cursors, 410, 380, 2, 2, true),
        new TextureRegion(Cursors, 405, 375, 1, 1, true),
        bounds
    );

    public static NineSlice SideButtonBackground(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors, 164, 440, 1, 2, true),
        new TextureRegion(Cursors, 162, 442, 2, 1, true),
        new TextureRegion(Cursors, 176, 453, 2, 1, true),
        new TextureRegion(Cursors, 175, 454, 1, 2, true),
        new TextureRegion(Cursors, 162, 440, 2, 2, true),
        new TextureRegion(Cursors, 176, 440, 2, 2, true),
        new TextureRegion(Cursors, 162, 454, 2, 2, true),
        new TextureRegion(Cursors, 176, 454, 2, 2, true),
        new TextureRegion(Cursors, 175, 442, 1, 1, true),
        bounds
    );

    public static NineSlice YellowButtonBackground(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors, 269, 256, 1, 2, true),
        new TextureRegion(Cursors, 267, 258, 2, 1, true),
        new TextureRegion(Cursors, 275, 263, 2, 1, true),
        new TextureRegion(Cursors, 274, 264, 1, 2, true),
        new TextureRegion(Cursors, 267, 256, 2, 2, true),
        new TextureRegion(Cursors, 275, 256, 2, 2, true),
        new TextureRegion(Cursors, 267, 264, 2, 2, true),
        new TextureRegion(Cursors, 275, 264, 2, 2, true),
        new TextureRegion(Cursors, 269, 258, 1, 1, true),
        bounds
    );

    public static NineSlice OrangeButtonBackground(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors, 258, 256, 1, 2, true),
        new TextureRegion(Cursors, 256, 258, 2, 1, true),
        new TextureRegion(Cursors, 264, 263, 2, 1, true),
        new TextureRegion(Cursors, 263, 264, 1, 2, true),
        new TextureRegion(Cursors, 256, 256, 2, 2, true),
        new TextureRegion(Cursors, 264, 256, 2, 2, true),
        new TextureRegion(Cursors, 256, 264, 2, 2, true),
        new TextureRegion(Cursors, 264, 264, 2, 2, true),
        new TextureRegion(Cursors, 258, 258, 1, 1, true),
        bounds
    );

    public static NineSlice RedButtonBackground(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors, 139, 338, 1, 2, true),
        new TextureRegion(Cursors, 137, 340, 2, 1, true),
        new TextureRegion(Cursors, 142, 344, 2, 1, true),
        new TextureRegion(Cursors, 141, 345, 1, 2, true),
        new TextureRegion(Cursors, 137, 338, 2, 2, true),
        new TextureRegion(Cursors, 142, 338, 2, 2, true),
        new TextureRegion(Cursors, 137, 345, 2, 2, true),
        new TextureRegion(Cursors, 142, 345, 2, 2, true),
        new TextureRegion(Cursors, 139, 340, 1, 1, true),
        bounds
    );

    public static NineSlice TextBubble(Rectangle bounds = new()) => new(
        new TextureRegion(Cursors_1_6, 244, 503, 3, 3, true),
        new TextureRegion(Cursors_1_6, 241, 506, 3, 3, true),
        new TextureRegion(Cursors_1_6, 247, 506, 3, 3, true),
        new TextureRegion(Cursors_1_6, 244, 509, 3, 3, true),
        new TextureRegion(Cursors_1_6, 241, 503, 3, 3, true),
        new TextureRegion(Cursors_1_6, 247, 503, 3, 3, true),
        new TextureRegion(Cursors_1_6, 241, 509, 3, 3, true),
        new TextureRegion(Cursors_1_6, 247, 509, 3, 3, true),
        new TextureRegion(Cursors_1_6, 244, 506, 3, 3, true),
        bounds
    );

    /// <summary>
    /// Hover event for side buttons
    /// </summary>
    private static Tooltip GetTooltipForSideButton(SideButtonVariant hint)
    {
        var (name, desc) = hint switch
        {
            SideButtonVariant.Alias => (I18n.UI_ChestAlias(), I18n.UI_ChestAlias_Desc()),
            SideButtonVariant.Set => (I18n.UI_QuickSet(), I18n.UI_QuickSet_Desc()),
            SideButtonVariant.Manage => (I18n.UI_Snapshot_Manage(), I18n.UI_Snapshot_Manage_Desc()),
            SideButtonVariant.Save => (I18n.UI_Snapshot_Save(), I18n.UI_Snapshot_Save_Desc()),
            SideButtonVariant.Unlink => (I18n.UI_Snapshot_Unlink(), I18n.UI_Snapshot_Unlink_Desc()),
            SideButtonVariant.Categorize => (I18n.UI_Categorize(), I18n.UI_Categorize_Desc()),
            SideButtonVariant.Lock => (null, I18n.UI_LockItems()),
            _ => (null, null)
        };
        return new Tooltip(name, desc);
    }
}

[Serializable]
public class ThemeInfo
{
    public string Name { get; set; } = "Default";

    public string[] For { get; set; } = Array.Empty<string>();

    public int YOffset { get; set; }
}

internal enum SideButtonVariant
{
    Alias = 0,
    Categorize = 1,
    Set = 2,
    Manage = 3,
    Save = 4,
    Unlink = 5,
    Lock = 6
}