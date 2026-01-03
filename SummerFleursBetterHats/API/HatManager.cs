#nullable enable
using Netcode;
using StardewValley.Objects;
using SummerFleursBetterHats.HatExtensions;

namespace SummerFleursBetterHats.API;

public class HatManager : ISummerfleursBetterHatsAPI
{
    /// <summary>
    /// The constant buff id used for applying buffs.
    /// </summary>
    public string DefaultBuffID => IdHelper.DefaultBuffID;

    /// <inheritdoc/>
    public Buff EmptyBuff => new(IdHelper.DefaultBuffID) { millisecondsDuration = 100 };

    /// <inheritdoc/>
    public event ISummerfleursBetterHatsAPI.HatUnequippedDelegate? OnHatUnequipped;

    /// <inheritdoc/>
    public event ISummerfleursBetterHatsAPI.HatEquippedDelegate? OnHatEquipped;

    public HatData? CachedHatData;

    public void OnHatChange(NetRef<Hat> field, Hat? oldHat, Hat? newHat)
    {
        if (oldHat != null)
        {
            var args = new HatUnequippedEventArgs(oldHat);
            OnHatUnequipped?.Invoke(this, args);

            // Apply an empty buff to override the current hat buff.
            Game1.player.applyBuff(EmptyBuff);
        }

        if (newHat != null)
        {
            // Get the hat data and convert it to a buff.
            var hatBuff = GetBuffAndHatData(newHat, out var data);
            var args = new HatEquippedEventArgs(newHat, hatBuff);
            CachedHatData = data;

            // Apply the buff to player.
            OnHatEquipped?.Invoke(this, args);
            if (data.CheckCondition())
                Game1.player.applyBuff(hatBuff);
        }
    }

    /// <inheritdoc/>
    public Buff GetBuffForThisHat(Hat hat, string? buffId = null)
        => GetBuffAndHatData(hat, out _, buffId);

    /// <summary>
    /// Internal version of <see cref="GetBuffForThisHat"/>,
    /// provided to allow for the out parameter.
    /// </summary>
    internal static Buff GetBuffAndHatData(Hat hat, out HatData data, string? buffId = null)
    {
        data = hat.GetHatData() ?? new HatData();
        return data.ConvertToBuff(buffId ?? IdHelper.DefaultBuffID);
    }
}

public class HatUnequippedEventArgs : IHatUnequippedEventArgs
{
    public Hat OldHat { get; }

    public HatUnequippedEventArgs(Hat oldHat) => OldHat = oldHat;
}

public class HatEquippedEventArgs : IHatEquippedEventArgs
{
    public Hat NewHat { get; }
    public Buff BuffToApply { get; }

    public HatEquippedEventArgs(Hat newHat, Buff buff)
    {
        NewHat = newHat;
        BuffToApply = buff;
    }
}