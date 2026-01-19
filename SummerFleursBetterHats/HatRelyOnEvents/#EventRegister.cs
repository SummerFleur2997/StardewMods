using BetterHatsAPI.API;
using StardewModdingAPI.Events;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public static partial class HatRelyOnEvents
{
    public static void RegisterHatRelatedEvents(ISummerFleurBetterHatsAPI api)
    {
        api.OnHatEquipped += HatEquipListener;
        api.OnHatUnequipped += HatUnequipListener;

        ModEntry.ModHelper.Events.GameLoop.ReturnedToTitle += UnRegisterAll;
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
            case GilsHatID:
                ModEntry.ModHelper.Events.Player.Warped += GilsHatLocationChanged;
                return;
            case BlueRibbonID:
                ModEntry.ModHelper.Events.Player.Warped += BlueRibbonLocationChanged;
                return;
            case JojaCapID:
                Game1.addMailForTomorrow("SFBH_JojaCap");
                return;
            case DarkBallcapID:
                Game1.addMailForTomorrow("SFBH_DarkBallcap");
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
            case GilsHatID:
                Game1.player.team.calicoEggSkullCavernRating.Value -= ExtraEggScore;
                break;
        }

        UnRegisterAll();
    }

    private static void UnRegisterAll(object s, ReturnedToTitleEventArgs e) => UnRegisterAll();

    private static void UnRegisterAll()
    {
        ModEntry.ModHelper.Events.Player.Warped -= BlueRibbonLocationChanged;
        ModEntry.ModHelper.Events.Player.Warped -= GilsHatLocationChanged;
        ModEntry.ModHelper.Events.Player.Warped -= MummyMaskLocationChanged;
        ModEntry.ModHelper.Events.Player.InventoryChanged -= WearPanHatBack;
        ModEntry.ModHelper.Events.Input.ButtonPressed -= PanHatsButtonPressed;
        ModEntry.ModHelper.Events.Input.ButtonsChanged -= TotemMaskButtonsChanged;
    }
}