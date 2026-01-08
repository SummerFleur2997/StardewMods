#nullable enable
using StardewValley.Objects;

namespace SummerFleursBetterHats.Framework;

public static class Utilities
{
    public static bool PlayerHatIs(string id) => PlayerHat()?.QualifiedItemId == id;

    public static bool PlayerHatIn(HashSet<string> id)
    {
        var hat = PlayerHat();
        return hat is not null && id.Contains(hat.QualifiedItemId);
    }

    public static Hat? PlayerHat() => Game1.player.hat?.Value;
}