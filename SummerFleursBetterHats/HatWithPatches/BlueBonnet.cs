using Microsoft.Xna.Framework;
using StardewValley.Tools;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForBlueBonnet(Harmony harmony)
    {
        try
        {
            var original = AccessTools.Method(typeof(Hoe), nameof(Hoe.DoFunction));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_BlueBonnet_DoFunction));
            harmony.Patch(original, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched Hoe.DoFunction for blue bonnet successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch for blue bonnet: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// When the player is wearing blue bonnet, 1% chance to drop
    /// artifact trove when digging.
    /// </summary>
    public static void DropArtifactTrove(GameLocation location, Vector2 tile)
    {
        if (!PlayerHatIs(BlueBonnetID))
            return;

        var r = Utility.CreateDaySaveRandom(
            tile.X * 1024.0,
            tile.Y * 7.0,
            Game1.stats.DirtHoed);

        if (r.NextDouble() > 0.01)
            return;

        Game1.createObjectDebris("(O)275", (int)tile.X, (int)tile.Y, location);
    }

    /// <summary>
    /// Add a transpiler to the <see cref="Hoe.DoFunction"/> method
    /// to allow drop artifact trove when wearing blue bonnet and
    /// digging.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_BlueBonnet_DoFunction(IEnumerable<CodeInstruction> ci)
    {
        var matcher = new CodeMatcher(ci);

        // Find anchor instructions for the injection
        var target = new CodeMatch[]
        {
            new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.stats))),
            new(OpCodes.Dup),
            new(OpCodes.Callvirt)
        };
        matcher.MatchStartForward(target);

        // If the anchor instruction is not found, throw an exception.
        if (matcher.IsInvalid)
            throw new Exception("This method seems to have changed.");

        // Add the injection to the codes
        var injection = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_1), // location
            new(OpCodes.Ldloc_0), // initialTile
            new(OpCodes.Call, AccessTools.Method(typeof(HatWithPatches), nameof(DropArtifactTrove)))
        };
        matcher.InsertAndAdvance(injection);

        return matcher.InstructionEnumeration();
    }
}