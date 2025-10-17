using System;
using CategoryRecipeIcons.Framework;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

namespace CategoryRecipeIcons;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    public static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    private static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    #endregion

    public override void Entry(IModHelper helper)
    {
        ModMonitor = Monitor;
        ModHelper = Helper;

        var harmony = new Harmony(ModManifest.UniqueID);
        I18n.Init(Helper.Translation);
        RegisterHarmonyPatches(harmony);
    }

    private static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalM1 = AccessTools.Method(
                typeof(CraftingRecipe), nameof(CraftingRecipe.getSpriteIndexFromRawIndex));
            var postfixM1 = AccessTools.Method(
                typeof(CategoryDataHelper), nameof(CategoryDataHelper.Patch_getSpriteIndexFromRawIndex));
            harmony.Patch(originalM1, postfix: new HarmonyMethod(postfixM1));
            Log("Patched CraftingRecipe.getSpriteIndexFromRawIndex successfully.");

            var originalM2 = AccessTools.Method(
                typeof(CraftingRecipe), nameof(CraftingRecipe.getNameFromIndex));
            var postfixM2 = AccessTools.Method(
                typeof(CategoryDataHelper), nameof(CategoryDataHelper.Patch_getNameFromIndex));
            harmony.Patch(originalM2, postfix: new HarmonyMethod(postfixM2));
            Log("Patched CraftingRecipe.getNameFromIndex successfully.");
        }
        catch (Exception ex)
        {
            Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }
}