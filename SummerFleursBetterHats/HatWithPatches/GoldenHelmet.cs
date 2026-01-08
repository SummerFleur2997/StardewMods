using StardewValley.GameData.WildTrees;
using StardewValley.TerrainFeatures;

namespace SummerFleursBetterHats.HatWithPatches;

public static partial class HatWithPatches
{
    private static void RegisterPatchForGoldenHelmet(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Tree), nameof(Tree.TryGetDrop));
            var prefix = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_GoldenHelmet_TryGetDrop));
            harmony.Patch(original, new HarmonyMethod(prefix));
            ModEntry.Log("Patched Tree.TryGetDrop for golden helmet successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for golden helmet: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Add a prefix to the <see cref="Tree.TryGetDrop"/> method
    /// to add the chance of golden coconut.
    /// </summary>
    public static bool Patch_GoldenHelmet_TryGetDrop(WildTreeItemData drop)
    {
        if (drop.ItemId == "(O)791" && PlayerHatIs(GoldenHelmetId))
            drop.Chance += 0.1f;
        return true;
    }
}