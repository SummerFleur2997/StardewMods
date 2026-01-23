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
        var id = e.NewHat.QualifiedItemId;
        switch (id)
        {
            /*****
             * Mod Events
             *****/

            // Blue ribbon, 998 token in SDV fair
            case BlueRibbonID:
                ModEntry.ModHelper.Events.Player.Warped += BlueRibbonLocationChanged;
                return;

            // Gil's hat, +10 egg score
            case GilsHatID:
                ModEntry.ModHelper.Events.Player.Warped += GilsHatLocationChanged;
                return;

            // Mummy mask, pacify the mummies here
            case MummyMaskID:
                ModEntry.ModHelper.Events.Player.Warped += MummyMaskLocationChanged;
                ModEntry.ModHelper.Events.World.NpcListChanged += MummyMaskMonsterSpawned;
                return;

            // Pan hats, using directly
            case IridiumPanHatID:
            case GoldPanHatID:
            case SteelPanHatID:
            case CopperPanID:
                ModEntry.ModHelper.Events.Input.ButtonPressed += PanHatsButtonPressed;
                return;

            // Totem mask, using as a warp totem, once per day
            case TotemMaskID:
                ModEntry.ModHelper.Events.Input.ButtonsChanged += TotemMaskButtonsChanged;
                return;

            /*****
             * Content patcher related events
             *****/

            // Receive mail
            case JojaCapID:
                Game1.addMailForTomorrow("SFBH_JojaCap");
                return;
            case DarkBallcapID:
                Game1.addMailForTomorrow("SFBH_DarkBallcap");
                return;

            // NPC special dialogue
            case AbigailsBowID:     // abigail
            case BeanieID:          // sam
            case BowlerHatID:       // victor
            case ChefHatID:         // harvey
            case ChickenMaskID:     // shane
            case ConeHatID:         // olivia
            case DeluxeCowboyHatID: // sophia
            case EyePatchID:        // lance
            case FashionHatID:      // claire
            case FlatToppedHatID:   // magnus
            case FloppyBeanieID:    // emily
            case FrogHatID:         // sebastian
            case GogglesID:         // maru
            case LeprechuanHatID:   // leah
            case PageboyCapID:      // penny
            case PlumChapeauID:     // scarlett
            case SportsCapID:       // alex
            case TricornHatID:      // elliott
            case TropiclipID:       // haley
                var trimmedId = id[3..];
                Game1.player.autoGenerateActiveDialogueEvent($"SFBH_{trimmedId}");
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
        ModEntry.ModHelper.Events.World.NpcListChanged -= MummyMaskMonsterSpawned;
    }
}