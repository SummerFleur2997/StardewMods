#nullable enable
using StardewValley;

namespace SummerFleursBetterHats;

public static class Utilities
{
    public static bool PlayerHatIs(string id)
    {
        var hat = Game1.player.hat?.Value;
        if (hat == null) return false;
        return hat.QualifiedItemId == id;
    }
}