#nullable enable
using BetterRetainingSoils.Framework;
using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Patcher;

internal static class OtherModsPatcher
{
    /// <summary>
    /// Patch UIInfoSuite2.UIElements.ShowCropAndBarrelTime.DetailRenderers.CropRender method.
    /// 添加自定义文本。Add custom strings.
    /// </summary>
    public static void Patch_DetailRenderers(TerrainFeature? terrain, List<string> entries, bool __result)
    {
        try
        {
            // 若当前地块不是耕地，或者当前耕地没有使用保湿土壤，直接返回。
            // When the terrain is not a hoedirt, or this dirt is not using a retaining soil, return
            if (!__result || terrain is not HoeDirt hoeDirt) return;
            // 添加自定义文本。
            // Add custom strings.
            var waterRemain = hoeDirt.GetWaterRemainDays();
            if (waterRemain < 1) return;
            entries.Add(I18n.String_WaterRemain(waterRemain));
        }
        catch (Exception ex)
        {
            ModEntry.Log("Something seems wrong when trying to add info to UIInfoSuite2.", LogLevel.Error);
            ModEntry.Log(ex.Message);
            ModEntry.Log(ex.StackTrace);
        }
    }

    public static void RegisterHarmonyPatchesToUI2(Harmony harmony)
    {
        try
        {
            if (ModEntry.ModHelper.ModRegistry.Get("Annosz.UiInfoSuite2") is null) return;
            var type = Type.GetType("UIInfoSuite2.UIElements.ShowCropAndBarrelTime+DetailRenderers, UIInfoSuite2");
            var originalM1 = AccessTools.Method(type, "CropRender");
            var postfixM1 = AccessTools.Method(typeof(OtherModsPatcher), nameof(Patch_DetailRenderers));
            harmony.Patch(original: originalM1, postfix: new HarmonyMethod(postfixM1));
            ModEntry.Log("Patched UIInfoSuite2 successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch UIInfoSuite2: {ex.Message}", LogLevel.Warn);
        }
    }
}