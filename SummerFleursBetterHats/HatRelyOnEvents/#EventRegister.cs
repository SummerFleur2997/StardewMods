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
                ModEntry.ModHelper.Events.Input.ButtonPressed += PanHatsButtonPressed;
                return;
            case TotemMaskID:
                ModEntry.ModHelper.Events.Input.ButtonsChanged += TotemMaskButtonsChanged;
                return;
            case MummyMaskID:
                ModEntry.ModHelper.Events.Player.Warped += MummyMaskLocationChanged;
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

        ModEntry.ModHelper.Events.Player.Warped -= MummyMaskLocationChanged;
        ModEntry.ModHelper.Events.Input.ButtonPressed -= PanHatsButtonPressed;
        ModEntry.ModHelper.Events.Input.ButtonsChanged -= TotemMaskButtonsChanged;
        ModEntry.ModHelper.Events.Player.InventoryChanged -= WearPanHatBack;
    }
}