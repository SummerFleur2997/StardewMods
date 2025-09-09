using System;
using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using WhyNotJumpInThatMineShaft.Framework;
using System.Collections.Generic;

namespace WhyNotJumpInThatMineShaft;

public class ShaftIndicator : IModule
{
    public bool IsActive { get; private set; }

    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.Display.RenderedHud += OnRenderedHud;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Display.RenderedHud -= OnRenderedHud;
    }

    /// <summary>
    /// 获取洞或梯子的地块坐标。
    /// </summary>
    /// <param name="shaft">是否是竖井</param>
    /// <returns>洞或梯子的地块坐标</returns>
    private static CoordinateAndDistance GetHolePosition(bool shaft = false)
    {
        var minDistance = float.MaxValue;
        var holeCoordinate = new Vector2();
        var playerPosition = Game1.player.Tile;

        // Find the nearest shaft
        var whichList = shaft ? MapScanner.Holes : MapScanner.Ladders;
        if (whichList.Count == 0) return new CoordinateAndDistance(Vector2.Zero, -1);
        foreach (var hole in whichList)
        {
            var holePosition = new Vector2(hole.X, hole.Y);
            var distance = Vector2.Distance(holePosition, playerPosition);

            if (distance > minDistance) continue;
            minDistance = distance;
            holeCoordinate = holePosition;
        }

        return new CoordinateAndDistance(holeCoordinate, minDistance);
    }

    /// <summary>
    /// 获取玩家到目标点的八方向描述。
    /// </summary>
    private static string GetDirection(Vector2 from, Vector2 to)
    {
        var delta = to - from;
        var angle = MathF.Atan2(delta.Y, delta.X) * 180 / MathF.PI;
        if (angle < 0) angle += 360;

        var direction = angle switch
        {
            >= 337.5f or < 22.5f => I18n.String_Right(),
            >= 22.5f and < 67.5f => I18n.String_BottomRight(),
            >= 67.5f and < 112.5f => I18n.String_Bottom(),
            >= 112.5f and < 157.5f => I18n.String_BottomLeft(),
            >= 157.5f and < 202.5f => I18n.String_Left(),
            >= 202.5f and < 247.5f => I18n.String_TopLeft(),
            >= 247.5f and < 292.5f => I18n.String_Top(),
            >= 292.5f and < 337.5f => I18n.String_TopRight(),
            _ => "???"
        };

        return direction + " ";
    }

    /// <summary>
    /// 在游戏屏幕上绘制 HUD 时调用，用于显示洞或梯子的提示信息。
    /// </summary>
    private static void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        // 确保玩家当前在矿井中时才显示
        if (Game1.currentLocation is not MineShaft) return;

        var config = ModEntry.Config;
        var message = "";
        if (MapScanner.Ladders.Count > 0)
        {
            var data = GetHolePosition();
            var direction = GetDirection(Game1.player.Tile, data.Coordinate);
            if (config.StairIndicator && data.Distance >= 3) DrawIndicator(data.Coordinate);
            message = I18n.String_StairFound();

            var details = new List<string>();
            if (config.ShowDirection) details.Add(direction);
            if (config.ShowDistance) details.Add(data.Distance + " " + I18n.String_Tiles());
            if (details.Count > 0) message += $" ({string.Join(", ", details)})";
        }
        if (MapScanner.Holes.Count > 0)
        {
            var data = GetHolePosition(true);
            var direction = GetDirection(Game1.player.Tile, data.Coordinate);
            if (config.ShaftIndicator && data.Distance >= 3) DrawIndicator(data.Coordinate, true);
            message = I18n.String_ShaftFound();

            var details = new List<string>();
            if (config.ShowDirection) details.Add(direction);
            if (config.ShowDistance) details.Add(data.Distance + " " + I18n.String_Tiles());
            if (details.Count > 0) message += $" ({string.Join(", ", details)})";
        }
        if (config.TextPrompter && message != "")
        {
            var textPosition = new Vector2(config.TextPositionX, config.TextPositionY);
            var color = MapScanner.Holes.Count > 0 ? Color.Red : Color.White;
            var scale = config.TextScale;
            Utility.drawTextWithShadow(e.SpriteBatch, message, Game1.dialogueFont, textPosition, color, scale);
        }

        return;

        void DrawIndicator(Vector2 holeCoordinate, bool shaft = false)
        {
            // Calculate the central pixel point of the player and hole
            const int tileSize = Game1.tileSize;
            var fixedPlayerPosition = Game1.player.getStandingPosition() + new Vector2(0, -tileSize / 2f);
            var fixedHolePosition = holeCoordinate * tileSize + new Vector2(tileSize / 2f, tileSize / 2f);

            // Calculate the pixel position of the indicator
            var delta = Vector2.Normalize(fixedHolePosition - fixedPlayerPosition);
            var indicatorPosition = fixedPlayerPosition + delta * tileSize * 2;
            var indicatorPixelPosition = indicatorPosition - new Vector2(Game1.viewport.X, Game1.viewport.Y);
            var rotation = MathF.Asin(delta.X / 1);
            if (delta.Y > 0) rotation = MathF.PI - rotation;

            var rectangle = shaft ? Sprites.ShaftIndicator : Sprites.StairIndicator;
            e.SpriteBatch.Draw(
                Sprites.IndicatorTexture,
                indicatorPixelPosition * Game1.options.zoomLevel / Game1.options.uiScale,
                rectangle,
                Color.White,
                rotation,
                new Vector2(6, 6),
                Game1.pixelZoom * Game1.options.uiScale,
                SpriteEffects.None,
                0f);
        }
    }
}

public readonly struct CoordinateAndDistance(Vector2 point, float distance)
{
    public Vector2 Coordinate { get; } = point;
    public int Distance { get; } = (int)distance;
}
