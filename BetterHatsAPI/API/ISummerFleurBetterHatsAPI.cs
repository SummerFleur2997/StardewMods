#nullable enable
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
    public event HatUnequippedDelegate? OnHatUnequipped;

    /// <summary>
    /// An event handler called when a hat is equipped.
    /// </summary>
    public event HatEquippedDelegate? OnHatEquipped;

    /// <summary>
    /// Get the buff from the given hat. The returned value is a read-only
    /// copy of the buff. So what you do to the buff will make no effect.
    /// </summary>
    public IEnumerable<Buff> GetBuffForThisHat(Hat hat);

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