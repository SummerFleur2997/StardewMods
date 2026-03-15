using BetterRetainingSoils.Framework;
using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace BetterRetainingSoils.Patcher;

internal static class VanillaPatcher
{
    /// <summary>
    /// Patch <see cref="StardewValley.TerrainFeatures.HoeDirt.GetFertilizerWaterRetentionChance"/> method.
    /// 若使用了保湿土壤但不是顶级保湿土壤，将保湿概率置为 0 以方便后续处理。请参阅
    /// HoeDirtManager.DayUpdate 内的逻辑以了解本模组的处理逻辑。
    /// When using a retaining-soil, but not a deluxe one, set the water
    /// retention chance to 0 to make it easy for follow-up processes.
    /// See also HoeDirtManager.DayUpdate to know my processing logic.
    /// </summary>
    /// <param name="__result">原函数的返回值。The return value of the original method.</param>
    /// <seealso cref="Patch_dayUpdate"/>
    public static void Patch_GetFertilizerWaterRetentionChance(ref float __result)
    {
        __result = Math.Abs(__result - 1.0f) < 1e-4f ? 1.0f : 0f;
    }

    public static void Patch_dayUpdate(HoeDirt __instance) => __instance.DayUpdate();

    /// <summary>
    /// Patch the ctors of <see cref="StardewValley.TerrainFeatures.HoeDirt"/> to listen
    /// the state change event.
    /// </summary>
    public static void Patch_Constructors(HoeDirt __instance) =>
        __instance.state.fieldChangeVisibleEvent += (_, _, newValue) =>
        {
            if (newValue == HoeDirt.watered)
                __instance.RefreshStatus();
        };

    public static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalM1 = AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.GetFertilizerWaterRetentionChance));
            var postfixM1 = AccessTools.Method(
                typeof(VanillaPatcher), nameof(Patch_GetFertilizerWaterRetentionChance));
            harmony.Patch(original: originalM1, postfix: new HarmonyMethod(postfixM1));
            ModEntry.Log("Patched HoeDirt.GetFertilizerWaterRetentionChance successfully.");

            var originalM2 = AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.dayUpdate));
            var postfixM2 = AccessTools.Method(typeof(VanillaPatcher), nameof(Patch_dayUpdate));
            harmony.Patch(originalM2, postfix: new HarmonyMethod(postfixM2));
            ModEntry.Log("Patched HoeDirt.dayUpdate successfully.");

            var originalM3 = AccessTools.Constructor(typeof(HoeDirt));
            var postfixM3 = AccessTools.Method(typeof(VanillaPatcher), nameof(Patch_Constructors));
            harmony.Patch(originalM3, postfix: new HarmonyMethod(postfixM3));
            ModEntry.Log("Patched HoeDirt constructors successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log("An error occured when try to patch methods, so this mod will not work.", LogLevel.Error);
            ModEntry.Log(ex.ToString(), LogLevel.Error);
            harmony.UnpatchAll(harmony.Id);
        }
    }
}