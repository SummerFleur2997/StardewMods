using StardewValley.Menus;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForSquidHat(Harmony harmony)
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
                typeof(HatWithPatches), nameof(Patch_SquidHat_BobberBar));
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
            Log("Patched BobberBar for squid hat successfully.");
        }
        catch (Exception ex)
        {
            Error($"Failed to patch BobberBar: {ex.Message}");
        }
    }

    /// <summary>
    /// Add a postfix to the ctor of <see cref="BobberBar"/>
    /// to reduce the difficulty of squid during Squid Fest.
    /// </summary>
    private static void Patch_SquidHat_BobberBar(BobberBar __instance, string whichFish)
    {
        if (whichFish == "151" && PlayerHatIs(SquidHatID) && Utility.GetDayOfPassiveFestival("SquidFest") > 0)
            __instance.difficulty -= 15;
    }
}