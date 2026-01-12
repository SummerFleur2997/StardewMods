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
    public event HatUnequippedDelegate OnHatUnequipped;

    /// <summary>
    /// An event handler called when a hat is equipped.
    /// </summary>
    public event HatEquippedDelegate OnHatEquipped;

    /// <summary>
    /// Gets all buffs associated with the given hat.
    /// </summary>
    /// <param name="hat">The hat to query buffs for.</param>
    /// <returns>
    /// A new collection containing newly instantiated <see cref="Buff"/> objects.
    /// Each call creates fresh instances; modifications to the returned buffs or
    /// collection will not affect the original hat data or other callers.
    /// </returns>
    /// <remarks>
    /// This method allocates new objects on each call. If you need to check
    /// buffs frequently, consider caching the result. The returned buffs
    /// reflect the hat's data at the time of the call and will not update if
    /// the underlying data changes.
    /// </remarks>
    public IEnumerable<Buff> GetBuffForThisHat(Hat hat);

    /// <summary>
    /// Set a custom condition checker for a specific hat.
    /// </summary>
    /// <param name="qualifiedHatID">The target hat's <see cref="Hat.QualifiedItemId"/>.</param>
    /// <param name="packID">The unique ID of which content pack the data belongs to.</param>
    /// <param name="customConditionChecker">Your custom condition checker.</param>
    public void SetCustomConditionChecker(string qualifiedHatID, string packID, Func<bool> customConditionChecker);

    /// <summary>
    /// Set a custom action trigger for a specific hat.
    /// </summary>
    /// <param name="qualifiedHatID">The target hat's <see cref="Hat.QualifiedItemId"/>.</param>
    /// <param name="packID">The unique ID of which content pack the data belongs to.</param>
    /// <param name="customActionTrigger">Your custom action trigger.</param>
    public void SetCustomActionTrigger(string qualifiedHatID, string packID, Action customActionTrigger);

    /// <summary>
    /// Set a custom modifier to the buff for a specific hat.
    /// </summary>
    /// <param name="qualifiedHatID">The target hat's <see cref="Hat.QualifiedItemId"/>.</param>
    /// <param name="packID">The unique ID of which content pack the data belongs to.</param>
    /// <param name="customModifier">Your custom modifier.</param>
    public void SetCustomBuffModifier(string qualifiedHatID, string packID, Action<Buff> customModifier);

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