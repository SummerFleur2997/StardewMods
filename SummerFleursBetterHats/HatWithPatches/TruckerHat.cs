using StardewValley.Locations;

namespace SummerFleursBetterHats.HatWithPatches;

public partial class HatWithPatches
{
    private static void RegisterPatchForTruckerHat(Harmony harmony)
    {
        try
        {
            var original1 = AccessTools.Method(typeof(BusStop), nameof(BusStop.answerDialogue));
            var original2 = AccessTools.Method(typeof(BusStop), nameof(BusStop.checkAction));
            var transpiler = AccessTools.Method(
                typeof(HatWithPatches), nameof(Patch_TruckerHat));
            harmony.Patch(original1, transpiler: new HarmonyMethod(transpiler));
            harmony.Patch(original2, transpiler: new HarmonyMethod(transpiler));
            ModEntry.Log("Patched BusStop for trucker hat successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch BusStop: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Set the ticket price to 1 when the player wears the trucker hat.
    /// </summary>
    public static int ModifyTicketPrice(int price) => PlayerHatIs(TruckerHatID) ? 1 : price;

    /// <summary>
    /// Add a transpiler to the <see cref="BusStop.answerDialogue"/> method
    /// to set the ticket price.
    /// </summary>
    public static IEnumerable<CodeInstruction> Patch_TruckerHat(IEnumerable<CodeInstruction> ci)
    {
        var codes = new List<CodeInstruction>(ci);

        foreach (var code in codes)
        {
            yield return code;
            // Inject an instruction to modify the ticket price
            if (code.opcode == OpCodes.Call && code.operand is MethodInfo { Name: "get_TicketPrice" })
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(HatWithPatches), nameof(ModifyTicketPrice)));
        }
    }

    /*  A previous try to use event to change the ticket price of bus.
        But it doesn't work in multiplayer.

    /// <summary>
    /// When equip the trucker hat, set the ticket price of bus to 0.
    /// </summary>
    private static void OnTruckerHatEquipped(object s, IHatEquippedEventArgs e)
    {
        var busStop = Game1.locations.OfType<BusStop>().FirstOrDefault();
        if (e.NewHat.QualifiedItemId != TruckerHatID || busStop is null)
            return;

        // Set the ticket price of bus to 1.
        // 需要解决多人游戏时设置票价无效的问题
        busStop.TicketPrice = 1;
    }

    /// <summary>
    /// When take off the trucker hat, reset the ticket price of bus to 500.
    /// </summary>
    private static void OnTruckerHatUnequipped(object s, IHatUnequippedEventArgs e)
    {
        var busStop = Game1.locations.OfType<BusStop>().FirstOrDefault();
        if (e.OldHat.QualifiedItemId != TruckerHatID || busStop is null)
            return;

        // Reset the ticket price of bus to 500.
        busStop.TicketPrice = 500;
    }*/
}