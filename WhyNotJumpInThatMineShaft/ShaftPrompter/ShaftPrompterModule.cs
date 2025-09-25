using System;
using System.Collections.Generic;
using Common;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using WhyNotJumpInThatMineShaft.Framework;

namespace WhyNotJumpInThatMineShaft.ShaftPrompter;

internal class ShaftPrompterModule : IModule
{
    private const int TileSize = Game1.tileSize;
    private readonly Harmony _harmony = new (ModEntry.Manifest.UniqueID + ".ShaftPrompter");
    private int _sleepTime;
    public bool IsActive { get; private set; }

    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.Display.RenderedHud += OnRenderedHud;
        RegisterHarmonyPatches(_harmony);
    }

    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Display.RenderedHud -= OnRenderedHud;
        _harmony.UnpatchAll(_harmony.Id);
    }

    public void RefreshSleepTime() => _sleepTime = 50;

    /// <summary>
    /// 获取最近的洞或梯子的地块坐标与距离。
    /// Get the coordinate of the nearest stair or shaft and its distance
    /// to the player.
    /// </summary>
    /// <param name="feature">是否是竖井 Whether this is a shaft</param>
    /// <returns>洞或梯子的地块坐标与距离 The coordinate and distance</returns>
    private static (Vector2 Coordinate, float Distance) GetNearestHolePosition(Feature feature)
    {
        var minDistance = float.MaxValue;
        var holeCoordinate = new Vector2();
        var playerPixelPosition = Game1.player.getStandingPosition();

        // Validate the data
        var whichList = feature switch
        {
            Feature.Shaft => MapScanner.Shafts,
            Feature.Stair => MapScanner.Stairs,
            Feature.Statue => MapScanner.Statue,
            _ => throw new ArgumentOutOfRangeException(feature.ToString())
        };

        if (whichList.Count == 0) return (Vector2.Zero, -1);

        // Find the nearest shaft
        foreach (var hole in whichList)
        {
            var holePosition = new Vector2(hole.X, hole.Y);
            var holePixelPosition = holePosition * TileSize + new Vector2(TileSize / 2f, TileSize / 2f);
            var distance = Vector2.Distance(holePixelPosition, playerPixelPosition) / TileSize;

            if (distance > minDistance) continue;
            minDistance = distance;
            holeCoordinate = holePosition;
        }

        return (holeCoordinate, minDistance);
    }

    /// <summary>
    /// 获取玩家到目标点的八方向描述。
    /// Get the direction toward the target point.
    /// </summary>
    private static string GetDirection(Vector2 from, Vector2 to)
    {
        var delta = to - from;
        var angle = MathF.Atan2(delta.Y, delta.X) * 180 / MathF.PI;
        while (angle < 0) angle += 360;

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

    private static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalM3 = AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkAction));
            var prefixM3 = AccessTools.Method(
                typeof(ShaftPrompterPatches), nameof(ShaftPrompterPatches.Patch_checkAction));
            harmony.Patch(original: originalM3, prefix: new HarmonyMethod(prefixM3));
            ModEntry.Log("Patched MineShaft.checkAction successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// 在游戏屏幕上绘制 HUD 时调用，用于显示洞或梯子的提示信息。
    /// </summary>
    private void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        // Confirm current location is the Mine or Skull Cavern
        // 确保当前位置是矿井或骷髅洞穴
        if (Game1.currentLocation is not MineShaft { mineLevel: not 77377 } mineShaft) return;
        if (_sleepTime > 0)
        {
            _sleepTime --;
            return;
        }

        // Cached data
        var config = ModEntry.Config;
        var data = (Coordinate: Vector2.Zero, Distance: -1f);

        // Draw stair indicator
        if (MapScanner.HasAStairHere)
        {
            data = GetNearestHolePosition(Feature.Stair);
            if (config.StairIndicator && data.Distance >= config.HideDistance)
                DrawIndicator(data.Coordinate, Feature.Stair);
        }

        // Draw shaft indicator
        if (MapScanner.HasAShaftHere)
        {
            data = GetNearestHolePosition(Feature.Shaft);
            if (config.ShaftIndicator && data.Distance >= config.HideDistance)
                DrawIndicator(data.Coordinate, Feature.Shaft);
        }

        // Draw calico statue indicator
        if (MapScanner.HasAStatueHere)
        {
            data = GetNearestHolePosition(Feature.Statue);
            if (config.StatueIndicator && data.Distance >= config.HideDistance)
                DrawIndicator(data.Coordinate, Feature.Statue);
        }

        // Draw text prompt
        if (config.TextPrompter && data.Distance > 0)
        {
            var message = GetText(data.Coordinate, data.Distance, MapScanner.HasAShaftHere);
            var textPosition = new Vector2(config.TextPositionX, config.TextPositionY);
            var color = MapScanner.HasAShaftHere ? Color.Red : Color.White;
            var scale = config.TextScale;
            // Draw with common font without a shadow
            e.SpriteBatch.DrawString(
                Game1.smallFont,
                message,
                textPosition,
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        // Draw shaft generatable indicator (above text prompt)
        if (config.ShaftGeneratableIndicator)
        {
            var generatableText = mineShaft.ladderHasSpawned
                ? I18n.String_ShaftNotGeneratable()
                : I18n.String_ShaftGeneratable();
            var generatableColor = mineShaft.ladderHasSpawned ? Color.Gray : Color.LimeGreen;
            var generatablePosition = new Vector2(config.TextPositionX, config.TextPositionY - 40 * config.TextScale);
            var scale = config.TextScale;

            // 使用普通字体且无阴影
            e.SpriteBatch.DrawString(
                Game1.smallFont,
                generatableText,
                generatablePosition,
                generatableColor,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        return;

        string GetText(Vector2 holeCoordinate, float distance, bool shaft)
        {
            // Basic prompt
            var direction = GetDirection(Game1.player.Tile, holeCoordinate);
            var message = shaft ? I18n.String_ShaftFound() : I18n.String_StairFound();

            // Detailed prompt
            var details = new List<string>();
            if (config.ShowDirection) details.Add(direction);
            if (config.ShowDistance) details.Add(distance.ToString("F1") + " " + I18n.String_Tiles());
            if (details.Count > 0) message += $" ({string.Join(", ", details)})";

            return message;
        }

        void DrawIndicator(Vector2 holeCoordinate, Feature feature)
        {
            // Calculate the central pixel point of the player and hole
            var fixedPlayerPosition = Game1.player.getStandingPosition() + new Vector2(0, -TileSize / 2f);
            var fixedHolePosition = holeCoordinate * TileSize + new Vector2(TileSize / 2f, TileSize / 2f);

            // Calculate the pixel position of the indicator
            var delta = Vector2.Normalize(fixedHolePosition - fixedPlayerPosition);
            var indicatorPosition = fixedPlayerPosition + delta * TileSize * 2;
            var indicatorPixelPosition = indicatorPosition - new Vector2(Game1.viewport.X, Game1.viewport.Y);
            var rotation = MathF.Asin(delta.X / 1);
            if (delta.Y > 0) rotation = MathF.PI - rotation;
            var scale = Game1.pixelZoom * Game1.options.uiScale * config.IndicatorScale * 
                        (Constants.TargetPlatform == GamePlatform.Android ? 1 / 3f : 1);

            var color = feature switch
            {
                Feature.Shaft => new Color(221, 97, 251),
                Feature.Stair => new Color(56, 227, 242),
                Feature.Statue => new Color(254, 210, 11),
                _ => throw new ArgumentOutOfRangeException(feature.ToString())
            };

            e.SpriteBatch.Draw(
                Assets.IndicatorTexture,
                indicatorPixelPosition * Game1.options.zoomLevel / Game1.options.uiScale,
                null,
                color,
                rotation,
                new Vector2(6, 6),
                scale,
                SpriteEffects.None,
                0f);
        }
    }

    private enum Feature
    {
        Shaft,
        Stair,
        Statue
    }
}