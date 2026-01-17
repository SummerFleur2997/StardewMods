using BetterHatsAPI.API;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public static partial class HatRelyOnEvents
{
    public static void RegisterHatRelatedEvents(ISummerFleurBetterHatsAPI api)
    {
        api.OnHatEquipped += HatEquipListener;
        api.OnHatUnequipped += HatUnequipListener;
    }

    private static void HatEquipListener(object s, IHatEquippedEventArgs e)
    {
        switch (e.NewHat.QualifiedItemId)
        {
            case IridiumPanHatID:
            case GoldPanHatID:
            case SteelPanHatID:
            case CopperPanID:
                ModEntry.ModHelper.Events.Input.ButtonPressed += PanHatButtonPressed;
                return;
            case MummyMaskID:
                ModEntry.ModHelper.Events.Player.Warped += MonsterHatLocationChanged;
                break;
            default:
                return;
        }
    }

    private static void HatUnequipListener(object s, IHatUnequippedEventArgs e)
    {
        switch (e.OldHat.QualifiedItemId)
        {
            case MummyMaskID:
                UpdateForThisLocationWhenDisable();
                break;
        }

        ModEntry.ModHelper.Events.Player.Warped -= MonsterHatLocationChanged;
        ModEntry.ModHelper.Events.Input.ButtonPressed -= PanHatButtonPressed;
        ModEntry.ModHelper.Events.Player.InventoryChanged -= WearPanHatBack;
    }
}