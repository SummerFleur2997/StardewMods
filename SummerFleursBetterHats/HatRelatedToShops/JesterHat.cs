namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    private const int DiscountedTicketPrice = 98;

    private static Item GetDiscountedTicketForJesterHat()
        => ItemRegistry.Create("(O)SummerFleur.BetterHats.DiscountedTicket", 2);
}