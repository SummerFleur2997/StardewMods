#nullable enable
using System.Diagnostics.Contracts;
using StardewModdingAPI;

namespace BetterHatsAPI.Framework;

public static class TranslationHelper
{
    [Pure]
    public static string? TryGetTranslation(this IContentPack pack, string? hint)
    {
        if (string.IsNullOrWhiteSpace(hint))
            return null;

        if (!hint.StartsWith("i18n:"))
            return hint;

        var key = hint[5..];
        return pack.Translation.Get(key).ToString();
    }
}