using System;
using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using WhyNotJumpInThatMineShaft.Framework;

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
        var minDistance = 0f;
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
    /// 在游戏屏幕上绘制 HUD 时调用，用于显示洞或梯子的提示信息。
    /// </summary>
    private static void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        // 确保玩家当前在矿井中时才显示
        if (Game1.currentLocation is not MineShaft) return;

        var message = "";
        if (MapScanner.Ladders.Count > 0)
        {
            var data = GetHolePosition();
            if(ModEntry.Config.StairIndicator && data.Distance >= 3) DrawIndicator(data.Coordinate);
            message = $"发现梯子！（{data.Distance} 格）";
        }
        if (MapScanner.Holes.Count > 0)
        {
            var data = GetHolePosition(true);
            if (ModEntry.Config.ShaftIndicator && data.Distance >= 3) DrawIndicator(data.Coordinate, true);
            message = $"发现竖井！（{data.Distance} 格）";
        }
        if (ModEntry.Config.TextPrompter && message != "")
        {
            var textPosition = new Vector2(32, 96); // TODO 自定义坐标
            var color = MapScanner.Ladders.Count > 0 ? Color.Red : Color.White; // TODO 自定义颜色
            Utility.drawTextWithShadow(e.SpriteBatch, message, Game1.dialogueFont, textPosition, color);
        }

        return;

        void DrawIndicator(Vector2 holeCoordinate, bool shaft = false)
        {
            // Calculate the central pixel point of the player and hole
            const int tileSize = Game1.tileSize;
            var fixedPlayerPosition = Game1.player.getStandingPosition() + new Vector2(0, -tileSize / 2f);
            var fixedHolePosition = holeCoordinate * tileSize + new Vector2(tileSize / 2f, tileSize / 2f);

            // Calculate the pixel position of the indicator
            var delta = Vector2.Normalize(fixedHolePosition - fixedPlayerPosition) * tileSize * 2;
            var indicatorPosition = fixedPlayerPosition + delta;
            var indicatorPixelPosition = indicatorPosition - new Vector2(Game1.viewport.X, Game1.viewport.Y);
            var rotation = MathF.Asin(delta.X / 1);

            var rectangle = shaft ? Sprites.ShaftIndicator : Sprites.StairIndicator;
            e.SpriteBatch.Draw(
                Sprites.IndicatorTexture, 
                indicatorPixelPosition, 
                rectangle, 
                Color.White, 
                rotation, 
                new Vector2(6, 6), 
                Game1.pixelZoom, 
                SpriteEffects.None, 
                1f);
        }
    }
}

public readonly struct CoordinateAndDistance(Vector2 point, float distance)
{
    public Vector2 Coordinate { get; } = point;
    public int Distance { get; } = (int)distance;
}
