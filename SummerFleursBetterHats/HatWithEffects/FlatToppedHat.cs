namespace SummerFleursBetterHats.HatWithEffects;

public partial class HatWithEffects
{
    /// <summary>
    /// Effect of Flat Topped Hat: get a Cranberry Sauce, a Stuffing Bread,
    /// and a Farmer's Lunch
    /// </summary>
    private static void Action_FlatToppedHat_AddThanksGivingGift()
    {
        var gift = new List<Item>
        {
            ItemRegistry.Create("(O)238"),
            ItemRegistry.Create("(O)239"),
            ItemRegistry.Create("(O)240")
        };
        Game1.player.addItemsByMenuIfNecessary(gift);
        Game1.showGlobalMessage(I18n.String_FlatToppedHat());
    }
}