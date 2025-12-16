#nullable enable
using StardewValley;
using StardewValley.Objects;

namespace SummerFleursBetterHats.HatWithBuffs;

public interface IHatBuffManagerAPI
{
    /// <summary>
    /// An empty buff that can be used to override the current hat buff.
    /// </summary>
    public Buff EmptyBuff { get; }

    /// <summary>
    /// An event handler called when a hat is unequipped.
    /// </summary>
    public event HatUnequippedDelegate? OnHatUnEquipped;

    /// <summary>
    /// An event handler called when a hat is equipped.
    /// </summary>
    public event HatEquippedDelegate? OnHatEquipped;

    public delegate void HatUnequippedDelegate(object? sender, IHatUnequippedEventArgs e);

    public delegate void HatEquippedDelegate(object? sender, IHatEquippedEventArgs e);
}

public interface IHatUnequippedEventArgs
{
    /// <summary>
    /// The unequipped hat.
    /// </summary>
    Hat? OldHat { get; }
}

public interface IHatEquippedEventArgs
{
    /// <summary>
    /// The equipped hat.
    /// </summary>
    Hat? NewHat { get; }
}