﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace UI.UserInterface;

internal class TooltipManager
{
    private Widget Tooltip { get; set; }

    public void ShowTooltipThisFrame(Widget tooltip)
    {
        Tooltip = tooltip;
    }

    public void Draw(SpriteBatch batch)
    {
        if (Tooltip == null)
            return;

        var mousePosition = Game1.getMousePosition(true);

        Tooltip.Position = new Point(
            mousePosition.X + 8 * Game1.pixelZoom,
            mousePosition.Y + 8 * Game1.pixelZoom
        );

        Tooltip.Draw(batch);
        Tooltip = null;
    }
}