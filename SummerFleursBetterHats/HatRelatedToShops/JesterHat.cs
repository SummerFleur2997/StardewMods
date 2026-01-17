namespace SummerFleursBetterHats.HatRelatedToShops;

public partial class HatRelatedToShops
{
    private const ushort JesterHatMask = 16; // 0b00010000
    private const int DiscountedTicketPrice = 98;

    private static Item GetDiscountedTicketForJesterHat()
        => ItemRegistry.Create("(O)SummerFleur.BetterHats.DiscountedTicket", 2);
}