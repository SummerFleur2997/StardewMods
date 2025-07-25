﻿#nullable enable
using System;
using System.Collections.Generic;
using BetterRetainingSoils.DirtService;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Patcher;

internal static class PatchOtherMods
{
    /// <summary>
    /// Patch UIInfoSuite2.UIElements.ShowCropAndBarrelTime.DetailRenderers.CropRender method.
    /// 添加自定义文本。Add custom strings.
    /// </summary>
    public static void Patch_DetailRenderers(TerrainFeature? terrain, List<string> entries, bool __result)
    {
        // 若当前地块不是耕地，或者当前耕地没有使用保湿土壤，直接返回。
        // When the terrian is not a hoedirt, or this dirt is not using a retaining soil, return
        if (!__result || terrain is not HoeDirt hoeDirt) return;
        // 添加自定义文本。
        // Add custom strings.
        var waterRemain = hoeDirt.GetHoeDirtData().WaterRemainDays;
        if (waterRemain < 1) return;
        entries.Add(I18n.String_WaterRemain(waterRemain));
    }

    /// <summary>
    /// Patch BetterSprinklersPlus.BetterSprinklersPlus.WaterTile method.
    /// 更新耕地信息。 Refresh dirt status.
    /// </summary>
    public static void Patch_WaterTile(GameLocation location, Vector2 tile)
    {
        if (location.terrainFeatures.TryGetValue(tile, out var feature) && feature is HoeDirt hoeDirt)
            hoeDirt.GetHoeDirtData().RefreshStatus();
    }

    public static void RegisterHarmonyPatchesToUI2(Harmony harmony)
    {
        try
        {
            if (ModEntry.ModHelper.ModRegistry.Get("Annosz.UiInfoSuite2") is null) return;
            var type = Type.GetType("UIInfoSuite2.UIElements.ShowCropAndBarrelTime+DetailRenderers, UIInfoSuite2");
            var originalM1 = AccessTools.Method(type, "CropRender");
            var postfixM1 = AccessTools.Method(typeof(PatchOtherMods), nameof(Patch_DetailRenderers));
            harmony.Patch(original: originalM1, postfix: new HarmonyMethod(postfixM1));
            ModEntry.Log("Patched UIInfoSuite2 successfully.", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch UIInfoSuite2: {ex.Message}", LogLevel.Warn);
        }
    }

    public static void RegisterHarmonyPatchesToBetterSprinkler(Harmony harmony)
    {
        try
        {
            if (ModEntry.ModHelper.ModRegistry.Get("com.CodesThings.BetterSprinklersPlus") is null) return;
            var type = Type.GetType("BetterSprinklersPlus.BetterSprinklersPlus, BetterSprinklersPlus");
            var originalM2 = AccessTools.Method(type, "WaterTile");
            var postfixM2 = AccessTools.Method(typeof(PatchOtherMods), nameof(Patch_WaterTile));
            harmony.Patch(original: originalM2, postfix: new HarmonyMethod(postfixM2));
            ModEntry.Log("Patched BetterSprinklersPlus successfully.", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch BetterSprinklersPlus: {ex.Message}", LogLevel.Warn);
        }
    }
}