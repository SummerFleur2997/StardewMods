#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using StardewValley.Objects;

namespace BetterHatsAPI.API;

/****
 * Note:
 *    This api can be used to add custom hat buffs when a hat is equipped.
 *
 *    All the event args in IHatUnequippedEventArgs or IHatEquippedEventArgs
 *    are already checked for null, so you don't need to check them again.
 *
 *    It may change in the future, to get the newest version of the api,
 *    please check the GitHub page:
 *
 *    https://github.com/SummerFleur2997/StardewMods/tree/master/BetterHatsAPI/API
 */

public interface ISummerFleurBetterHatsAPI
{
    /// <summary>
    /// An event handler called when a hat is unequipped.
    /// </summary>
    public event HatUnequippedDelegate OnHatUnequipped;

    /// <summary>
    /// An event handler called when a hat is equipped.
    /// </summary>
    public event HatEquippedDelegate OnHatEquipped;

    /// <summary>
    /// Get the buff from the given hat. The returned value is a read-only
    /// copy of the buff. So what you do to the buff will make no effect.
    /// </summary>
    public IEnumerable<Buff> GetBuffForThisHat(Hat hat);

    /// <summary>
    /// Set a custom condition checker for a specific hat.
    /// </summary>
    /// <param name="qualifiedHatID">The target hat's <see cref="Hat.QualifiedItemId"/>.</param>
    /// <param name="packID">The unique ID of which content pack the data belongs to.</param>
    /// <param name="customConditionChecker">Your custom condition checker.</param>
    /// <param name="ex">The exception that occurred during the operation, if any.</param>
    /// <returns>True if the operation is successful, false otherwise.</returns>
    [Pure]
    public bool SetCustomConditionChecker(string qualifiedHatID, string packID, Func<bool> customConditionChecker,
        [NotNullWhen(false)] out Exception? ex);

    /// <summary>
    /// Set a custom action trigger for a specific hat.
    /// </summary>
    /// <param name="qualifiedHatID">The target hat's <see cref="Hat.QualifiedItemId"/>.</param>
    /// <param name="packID">The unique ID of which content pack the data belongs to.</param>
    /// <param name="customActionTrigger">Your custom action trigger.</param>
    /// <param name="ex">The exception that occurred during the operation, if any.</param>
    /// <returns>True if the operation is successful, false otherwise.</returns>
    [Pure]
    public bool SetCustomActionTrigger(string qualifiedHatID, string packID, Action customActionTrigger,
        [NotNullWhen(false)] out Exception? ex);

    public delegate void HatUnequippedDelegate(object? sender, IHatUnequippedEventArgs e);

    public delegate void HatEquippedDelegate(object? sender, IHatEquippedEventArgs e);
}

/// <summary>
/// Event arguments for when a hat is unequipped.
/// </summary>
public interface IHatUnequippedEventArgs
{
    /// <summary>
    /// The unequipped hat.
    /// </summary>
    Hat OldHat { get; }
}

/// <summary>
/// Event arguments for when a hat is equipped.
/// </summary>
public interface IHatEquippedEventArgs
{
    /// <summary>
    /// The equipped hat.
    /// </summary>
    Hat NewHat { get; }
}