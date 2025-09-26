﻿using System;
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

    private static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalM3 = AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkAction));
            var prefixM3 = AccessTools.Method(
                typeof(ShaftPrompterPatches), nameof(ShaftPrompterPatches.Patch_checkAction));
            harmony.Patch(originalM3, new HarmonyMethod(prefixM3));
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
        var config = ModEntry.Config;

        // Confirm current location is the Mine or Skull Cavern
        // 确保当前位置是矿井或骷髅洞穴
        if (Game1.currentLocation is not MineShaft { mineLevel: not 77377 } mineShaft) return;
        if (_sleepTime > 0)
        {
            _sleepTime--;
            return;
        }

        var data = new FeatureData();

        // Draw stair indicator
        if (MapScanner.HasAStairHere)
        {
            GetNearestFeaturePosition(Feature.Stair, ref data);
            if (config.StairIndicator && data.Distance >= config.HideDistance)
                DrawIndicator(Feature.Stair, e, ref data);
        }

        // Draw shaft indicator
        if (MapScanner.HasAShaftHere)
        {
            GetNearestFeaturePosition(Feature.Shaft, ref data);
            if (config.ShaftIndicator && data.Distance >= config.HideDistance)
                DrawIndicator(Feature.Shaft, e, ref data);
        }

        // Draw calico statue indicator
        if (MapScanner.HasAStatueHere)
        {
            GetNearestFeaturePosition(Feature.Statue, ref data);
            if (config.StatueIndicator && data.Distance >= config.HideDistance)
                DrawIndicator(Feature.Statue, e, ref data);
        }

        DrawText(mineShaft.ladderHasSpawned, e, ref data);
    }

    /// <summary>
    /// 获取最近的洞或梯子的地块坐标与距离。
    /// Get the coordinate of the nearest feature and its distance to the player.
    /// </summary>
    private static void GetNearestFeaturePosition(Feature feature, ref FeatureData data)
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

        if (whichList.Count == 0)
        {
            data = new FeatureData();
            return;
        }

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

        data = new FeatureData(holeCoordinate, minDistance);
    }

    /// <summary>
    /// 绘制提示文字。
    /// Draw text prompters.
    /// </summary>
    private static void DrawText(bool ladderHasSpawned, RenderedHudEventArgs e, ref FeatureData data)
    {
        var config = ModEntry.Config;
        // 获取玩家到目标点的八方向描述。
        // Get the direction toward the target point.
        var delta = data.Position - Game1.player.Tile;
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

        // Basic prompt
        var message = MapScanner.HasAShaftHere ? I18n.String_ShaftFound() : I18n.String_StairFound();

        // Detailed prompt
        var details = new List<string>();
        if (config.ShowDirection) details.Add(direction + " ");
        if (config.ShowDistance) details.Add(data.Distance.ToString("F1") + " " + I18n.String_Tiles());
        if (details.Count > 0) message += $" ({string.Join(", ", details)})";

        // Draw text prompt
        if (config.TextPrompter && data.Distance > 0)
        {
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
            var generatableText = ladderHasSpawned
                ? I18n.String_ShaftNotGeneratable()
                : I18n.String_ShaftGeneratable();
            var generatableColor = ladderHasSpawned ? Color.Gray : Color.LimeGreen;
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
    }

    /// <summary>
    /// 绘制指向指定地物的箭头。
    /// Draw an indicator point to specific feature.
    /// </summary>
    private static void DrawIndicator(Feature feature, RenderedHudEventArgs e, ref FeatureData data)
    {
        // Calculate the central pixel point of the player and hole
        var fixedPlayerPosition = Game1.player.getStandingPosition() + new Vector2(0, -TileSize / 2f);
        var fixedHolePosition = data.Position * TileSize + new Vector2(TileSize / 2f, TileSize / 2f);

        // Calculate the pixel position of the indicator
        var delta = Vector2.Normalize(fixedHolePosition - fixedPlayerPosition);
        var indicatorPosition = fixedPlayerPosition + delta * TileSize * 2;
        var indicatorPixelPosition = indicatorPosition - new Vector2(Game1.viewport.X, Game1.viewport.Y);
        var rotation = MathF.Asin(delta.X / 1);
        if (delta.Y > 0) rotation = MathF.PI - rotation;

        var scale = Game1.pixelZoom * Game1.options.uiScale * ModEntry.Config.IndicatorScale *
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

    /// <summary>
    /// 各种矿井内的地形特征。
    /// Types of features in the shaft.
    /// </summary>
    private enum Feature
    {
        Shaft,
        Stair,
        Statue
    }

    private readonly struct FeatureData
    {
        /// <summary>
        /// 目标地物的位置，单位为地块坐标。
        /// The position of the targeted feature in tiles.
        /// </summary>
        public readonly Vector2 Position = Vector2.Zero;

        /// <summary>
        /// 目标地物与玩家之间的距离，单位为地块数。
        /// The distance between the player and targeted feature in tiles.
        /// </summary>
        public readonly float Distance = -1f;

        /// <summary>
        /// 使用确定值设定结构
        /// Use certain value to init fields.
        /// </summary>
        public FeatureData(Vector2 position, float distance)
        {
            Position = position;
            Distance = distance;
        }

        /// <summary>
        /// 使用初始值设定结构
        /// Use default value to init fields.
        /// </summary>
        public FeatureData() { }
    }
}