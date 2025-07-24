#nullable enable
using System;
using System.Collections.Generic;
using BetterRetainingSoils.DirtService;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Patcher;

internal static class UI2Patches
{
    /// <summary>
    /// Patches UIInfoSuite2.UIElements.ShowCropAndBarrelTime.DetailRenderers.CropRender method.
    /// 添加自定义文本。Add custom strings.
    /// </summary>
    public static void Patch_DetailRenderers(TerrainFeature? terrain, List<string> entries, bool __result)
    {
        // When the terrian is not a hoedirt, or this dirt is not using a retaining soil, return
        // 若当前地块不是耕地，或者当前耕地没有使用保湿土壤，直接返回。
        if (!__result || terrain is not HoeDirt hoeDirt || !hoeDirt.IsAvailable()) return;
        // Add custom strings.
        // 添加自定义文本。
        var waterRemain = hoeDirt.GetHoeDirtData().WaterRemainDays;
        if (waterRemain < 1) return;
        entries.Add(I18n.String_WaterRemain(waterRemain));
    }

    public static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            if (ModEntry.ModHelper.ModRegistry.Get("Annosz.UiInfoSuite2") is null) return;
            var type = Type.GetType("UIInfoSuite2.UIElements.ShowCropAndBarrelTime+DetailRenderers, UIInfoSuite2");
            var originalM2 = AccessTools.Method(type, "CropRender");
            var postfixM2 =  AccessTools.Method(typeof(UI2Patches), nameof(Patch_DetailRenderers));
            harmony.Patch(original: originalM2, postfix: new HarmonyMethod(postfixM2));
            ModEntry.Log("Patched UIInfoSuite2 successfully.", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch UIInfoSuite2: {ex.Message}", LogLevel.Warn);
        }
    }
}