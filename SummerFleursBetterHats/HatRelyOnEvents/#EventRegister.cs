using BetterHatsAPI.API;
using StardewModdingAPI.Events;
using SummerFleursBetterHats.Framework;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public static partial class HatRelyOnEvents
{
    public static void RegisterHatRelatedEvents(ISummerFleurBetterHatsAPI api)
    {
        api.OnHatEquipped += HatEquipListener;
        api.OnHatUnequipped += HatUnequipListener;

        ModEvents.GameLoop.ReturnedToTitle += UnRegisterAll;
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
                ModEvents.Player.Warped += BlueRibbonLocationChanged;
                return;

            // Gil's hat, +10 egg score
            case GilsHatID:
                ModEvents.Player.Warped += GilsHatLocationChanged;
                return;

            // Mummy mask, pacify the mummies here
            case MummyMaskID:
                ModEvents.Player.Warped += MummyMaskLocationChanged;
                ModEvents.World.NpcListChanged += MummyMaskMonsterSpawned;
                return;

            // Pan hats, using directly
            case IridiumPanHatID:
            case GoldPanHatID:
            case SteelPanHatID:
            case CopperPanID:
                ModEvents.Input.ButtonPressed += PanHatsButtonPressed;
                return;

            // Cool cap, watering the crops in one click
            case CoolCapID:
                ModEvents.Input.ButtonsChanged += CoolCapButtonsChanged;
                return;

            // Delicate bow, heal the player continuously
            case DelicateBowID:
                ModEvents.Input.ButtonsChanged += DelicateBowButtonsChanged;
                return;

            // Totem mask, using as a warp totem, once per day
            case TotemMaskID:
                ModEvents.Input.ButtonsChanged += TotemMaskButtonsChanged;
                return;


            /*****
             * Content patcher related events
             *****/

            // Receive mail
            case JojaCapID:
                Game1.addMailForTomorrow("SummerFleur.BetterHats.JojaCap");
                return;
            case DarkBallcapID:
                Game1.addMailForTomorrow("SummerFleur.BetterHats.DarkBallcap");
                return;

            // NPC special dialogue

            case AbigailsBowID: // abigail
            case ArcaneHatID: // magnus
            case BeanieID when Game1.season is Season.Winter: // sam
            case BowlerHatID: // victor
            case ChefHatID: // harvey
            case ChickenMaskID when Game1.player.getFriendshipHeartLevelForNPC("Shane") >= 6: // shane
            case ConeHatID: // olivia
            case DeluxeCowboyHatID: // sophia
            case FashionHatID when Game1.player.getFriendshipHeartLevelForNPC("Claire") >= 6: // claire
            case FloppyBeanieID when Game1.season is Season.Winter: // emily
            case FrogHatID: // sebastian
            case GogglesID: // maru
            case LeprechuanHatID: // leah
            case PageboyCapID when Game1.player.achievements.Contains(35): // penny
            case PlumChapeauID: // scarlett
            case SportsCapID: // alex
            case SwashbucklerHatID: // lance
            case TricornHatID: // elliott
            case TropiclipID: // haley
                var trimmedName = e.NewHat.Name.Replace(" ", "");
                Game1.player.autoGenerateActiveDialogueEvent($"SummerFleur.BetterHats.{trimmedName}", 7);
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
            case GilsHatID when Game1.player.TryGetWorldStatus(GilsHatMask):
                Game1.player.team.calicoEggSkullCavernRating.Value -= ExtraEggScore;
                break;

            case AbigailsBowID: // abigail
            case ArcaneHatID: // magnus
            case BeanieID: // sam
            case BowlerHatID: // victor
            case ChefHatID: // harvey
            case ChickenMaskID: // shane
            case ConeHatID: // olivia
            case DeluxeCowboyHatID: // sophia
            case FashionHatID: // claire
            case FloppyBeanieID: // emily
            case FrogHatID: // sebastian
            case GogglesID: // maru
            case LeprechuanHatID: // leah
            case PageboyCapID: // penny
            case PlumChapeauID: // scarlett
            case SportsCapID: // alex
            case SwashbucklerHatID: // lance
            case TricornHatID: // elliott
            case TropiclipID: // haley
                var trimmedName = e.OldHat.Name.Replace(" ", "");
                if (Game1.player.hasOrWillReceiveMail($"SummerFleur.BetterHats.{trimmedName}"))
                    break;

                Game1.player.activeDialogueEvents.Remove($"SummerFleur.BetterHats.{trimmedName}");
                break;
        }

        UnRegisterAll();
    }

    private static void UnRegisterAll(object s, ReturnedToTitleEventArgs e) => UnRegisterAll();

    private static void UnRegisterAll()
    {
        ModEvents.Player.Warped -= BlueRibbonLocationChanged;
        ModEvents.Player.Warped -= GilsHatLocationChanged;
        ModEvents.Player.Warped -= MummyMaskLocationChanged;
        ModEvents.Player.InventoryChanged -= WearPanHatBack;
        ModEvents.Input.ButtonPressed -= PanHatsButtonPressed;
        ModEvents.Input.ButtonsChanged -= CoolCapButtonsChanged;
        ModEvents.Input.ButtonsChanged -= DelicateBowButtonsChanged;
        ModEvents.Input.ButtonsChanged -= TotemMaskButtonsChanged;
        ModEvents.World.NpcListChanged -= MummyMaskMonsterSpawned;
        ModEvents.GameLoop.OneSecondUpdateTicked -= DelicateBowHeal;
    }

    private static IModEvents ModEvents => ModEntry.ModHelper.Events;
}