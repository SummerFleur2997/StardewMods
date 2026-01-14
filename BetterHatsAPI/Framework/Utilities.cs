using System.Diagnostics.Contracts;
using StardewValley.Objects;

namespace BetterHatsAPI.Framework;

internal static class Utilities
{
    /// <summary>
    /// Checks if the given float value is approximately equal to another
    /// float value to account for floating-point precision errors.
    /// </summary>
    /// <param name="value">The number to check.</param>
    /// <param name="target">The number to compare with param *value*.</param>
    /// <param name="epsilon">The tolerance threshold for considering the numbers as equal.</param>
    public static bool ApproximatelyEquals(this float value, float target, float epsilon = 1e-4f)
        => Math.Abs(value - target) < epsilon;

    /// <summary>
    /// Formats a float value to a string with 2 decimal places, then trims
    /// the trailing zeros. For example, 1.50 will be formatted to 1.5.
    /// </summary>
    /// <param name="value">The value needs to format and trim.</param>
    /// <returns>Formated number as a string.</returns>
    public static string FormatAndTrim(this float value)
        => value.ToString("F").TrimEnd('0').TrimEnd('.');

    /// <summary>
    /// Load description from the item's data if necessary.
    /// </summary>
    /// <param name="hat">The hat of which description will be loaded.</param>
    /// <returns>The description loaded from item data. If this hat is an
    /// error hat, return an empty string instead.</returns>
    public static string LoadDescription(this Hat hat)
    {
        var itemData = ItemRegistry.GetDataOrErrorItem(hat.QualifiedItemId);
        return !itemData.IsErrorItem ? itemData.Description : "";
    }

#nullable enable

    /// <summary>
    /// Try to get a translation from a content pack.
    /// </summary>
    /// <param name="pack">The content pack to get the translation from.</param>
    /// <param name="hint">The hint used to get a translation. If starts with "i18n:",
    /// the rest of the string is used as a key to get a translation.</param>
    /// <returns> The return value is one of the following:
    /// <list type="bullet">
    /// <item><c>null</c> if the hint is null or whitespace. </item>
    /// <item>The original hint if it does not start with the "i18n:" prefix. </item>
    /// <item>Otherwise, the translated string for the provided key.</item>
    /// </list></returns>
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

#nullable disable

    #region Log Warpper

    public static void Log(string m) => ModEntry.Log(m);
    public static void Warn(string m) => ModEntry.Log(m, LogLevel.Warn);
    public static void Error(string m) => ModEntry.Log(m, LogLevel.Error);

    #endregion
}