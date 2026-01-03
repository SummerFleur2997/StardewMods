#nullable enable
using StardewValley.Objects;

namespace SummerFleursBetterHats.API;

/***
 * Note:
 *    This api can be used to add custom hat buffs when a hat is equipped.
 *
 *    All the event args in IHatUnequippedEventArgs or IHatEquippedEventArgs are already
 *    checked for null, so you don't need to check them again.
 */

public interface ISummerfleursBetterHatsAPI
{
    /// <summary>
    /// The buff id used to apply buffs. And when a hat is unequipped,
    /// we will use this id and create a 100 ms empty buff to remove the buff.
    /// </summary>
    /// <seealso cref="EmptyBuff"/>
    public string DefaultBuffID { get; }

    /// <summary>
    /// An empty buff that can be used to override the current hat buff.
    /// </summary>
    public Buff EmptyBuff { get; }

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
    public Buff GetBuffForThisHat(Hat hat, string? buffId = null);

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

    /// <summary>
    /// The equipped hat. 
    /// </summary>
    Buff BuffToApply { get; }
}