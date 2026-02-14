using StardewValley.Menus;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForEyePatch(Harmony harmony)
    {
        try
        {
            var paras = new[]
            {
                typeof(string), typeof(float), typeof(bool), typeof(List<string>),
                typeof(string), typeof(bool), typeof(string), typeof(bool)
            };
            var original = AccessTools.Constructor(typeof(BobberBar), paras);
            var postfix = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_EyePatch_BobberBar));
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            Log("Patched BobberBar for eye patch successfully.");
        }
        catch (Exception ex)
        {
            Error($"Failed to patch BobberBar: {ex.Message}");
        }
    }

    /// <summary>
    /// Add a postfix to the ctor of <see cref="BobberBar"/>
    /// to add the length of the bobber bar, and reduce the
    /// difficulty of fish.
    /// </summary>
    private static void Patch_EyePatch_BobberBar(BobberBar __instance)
    {
        if (PlayerHatIs(EyePatchID))
        {
            __instance.difficulty *= 0.75f;
            __instance.bobberBarHeight += 60;
        }
    }
}