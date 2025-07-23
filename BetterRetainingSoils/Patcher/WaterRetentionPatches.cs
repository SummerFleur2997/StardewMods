using System;
using BetterRetainingSoils.DirtService;
using BetterRetainingSoils.Framework.MultiplayerService;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace BetterRetainingSoils.Patcher;

internal static class WaterRetentionPatches
{
    /// <summary>
    /// Patches <see cref="StardewValley.TerrainFeatures.HoeDirt.GetFertilizerWaterRetentionChance"/> method.
    /// 根据模组逻辑重新判断是否保留水分。
    /// Re-judge whether to retain waters by this mod's logic
    /// </summary>
    /// <param name="__result">原函数的返回值。The return value of the original method.</param>
    public static void Patch_GetFertilizerWaterRetentionChance(ref float __result)
    {
        __result = Math.Abs(__result - 1.0f) < 1e-4f ? 1.0f : 0f;
    }

    /// <summary>
    /// Patches <see cref="StardewValley.TerrainFeatures.HoeDirt.performToolAction"/> method.
    /// 应用自定义浇水事件。Use custom watering event.
    /// </summary>
    /// <param name="__instance">引用的耕地。 Refered soil.</param>
    /// <param name="t">使用的工具。 Used tool.</param>
    /// <returns>始终需要进一步处理，因此总是返回 true 并使用原方法。
    /// Always true to proceed with the original method.</returns>
    public static bool Patch_performToolAction(HoeDirt __instance, Tool t)
    {
        if (t is not WateringCan) return true;
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            MultiplayerServer.SendDirtData(__instance);
        else
        {
            var data = __instance.GetHoeDirtData();
            if (data == null) return true;
            data.RefreshStatus();
        }
        return true;
    }

    public static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalM1 = AccessTools.Method(typeof(HoeDirt), "GetFertilizerWaterRetentionChance");
            var postfixM1 = AccessTools.Method(
                typeof(WaterRetentionPatches), nameof(Patch_GetFertilizerWaterRetentionChance));
            harmony.Patch(original: originalM1, postfix: new HarmonyMethod(postfixM1));
            ModEntry.Log("Patched HoeDirt.GetFertilizerWaterRetentionChance successfully.", LogLevel.Debug);
            
            var originalM2 = AccessTools.Method(typeof(HoeDirt), "performToolAction");
            var prefixM2 = AccessTools.Method(
                typeof(WaterRetentionPatches), nameof(Patch_performToolAction));
            harmony.Patch(original: originalM2, prefix: new HarmonyMethod(prefixM2));
            ModEntry.Log("Patched ChestOverlay.Getoffset for fridges successfully.", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }
}