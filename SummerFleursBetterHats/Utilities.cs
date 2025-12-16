using StardewValley;

namespace SummerFleursBetterHats;

public static class Utilities
{
    public static bool PlayerHatIs(string id) => Game1.player.hat.Value.QualifiedItemId == id;
}