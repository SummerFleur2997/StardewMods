using HarmonyLib;

namespace BinThereSavedThat;

public static class Patcher
{
    public static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Utility), nameof(Utility.trashItem));
            var postfix = AccessTools.Method(typeof(Patcher), nameof(Patch_trashItem));
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            ModEntry.Log("Patched Utility.trashItem successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }

    public static void Patch_trashItem(Item item) => item.AddToSavedStorage();
}