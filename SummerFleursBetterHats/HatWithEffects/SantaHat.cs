using StardewValley.Constants;

namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Effect of Santa Hat: if the player has forage mastery, get a
    /// Golden Mystery Box as santa's gift, otherwise, a Mystery Box.
    /// </summary>
    private static void Action_SantaHat_AddMysteryBox()
    {
        var player = Game1.player;
        var gift = ItemRegistry.Create(player.stats.Get(StatKeys.Mastery(Farmer.foragingSkill)) != 0
            ? "(O)GoldenMysteryBox"
            : "(O)MysteryBox");
        player.addItemByMenuIfNecessary(gift);
        Game1.showGlobalMessage(I18n.String_SantaHat());
    }
}