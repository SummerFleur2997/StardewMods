#nullable enable
using Netcode;
using StardewValley;
using StardewValley.Objects;
using SummerFleursBetterHats.HatExtensions;

namespace SummerFleursBetterHats.API;

public class HatManager : ISummerfleursBetterHatsAPI
{
    /// <summary>
    /// The constant buff id used for applying buffs.
    /// </summary>
    public string DefaultBuffID => "SummerFleur.SummerFleursBetterHats.HatBuff";

    /// <inheritdoc/>
    public Buff EmptyBuff => new(DefaultBuffID) { millisecondsDuration = 100 };

    /// <inheritdoc/>
    public event ISummerfleursBetterHatsAPI.HatUnequippedDelegate? OnHatUnequipped;

    /// <inheritdoc/>
    public event ISummerfleursBetterHatsAPI.HatEquippedDelegate? OnHatEquipped;

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
            var hatBuff = GetBuffFromHatData(newHat);
            var args = new HatEquippedEventArgs(newHat, hatBuff);

            // Apply the buff to player.
            OnHatEquipped?.Invoke(this, args);
            if (args.ShouldApply) Game1.player.applyBuff(hatBuff);
        }
    }

    public Buff GetBuffFromHatData(Hat hat, string? buffId = null)
    {
        var data = hat.GetHatData() ?? new HatData();
        var buff = new Buff(buffId ?? DefaultBuffID);

        buff.effects.CombatLevel.Value = data.CombatLevel;
        buff.effects.FarmingLevel.Value = data.FarmingLevel;
        buff.effects.FishingLevel.Value = data.FishingLevel;
        buff.effects.MiningLevel.Value = data.MiningLevel;
        buff.effects.LuckLevel.Value = data.LuckLevel;
        buff.effects.ForagingLevel.Value = data.ForagingLevel;
        buff.effects.MaxStamina.Value = data.MaxStamina;
        buff.effects.MagneticRadius.Value = data.MagneticRadius;
        buff.effects.Speed.Value = data.Speed;
        buff.effects.Attack.Value = data.Attack;
        buff.effects.Defense.Value = data.Defense;
        buff.effects.Immunity.Value = data.Immunity;
#if RELEASE
        buff.visible = false;
#endif
        buff.millisecondsDuration = Buff.ENDLESS;

        return buff;
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
    public bool ShouldApply { get; set; } = true;

    public HatEquippedEventArgs(Hat newHat, Buff buff)
    {
        NewHat = newHat;
        BuffToApply = buff;
    }
}