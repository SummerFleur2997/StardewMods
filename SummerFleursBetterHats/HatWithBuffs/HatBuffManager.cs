#nullable enable
using Netcode;
using StardewValley;
using StardewValley.Objects;
using SummerFleursBetterHats.HatExtensions;

namespace SummerFleursBetterHats.HatWithBuffs;

public class HatBuffManager : IHatBuffManagerAPI
{
    public static readonly string BuffID = ModEntry.Manifest.UniqueID + "HatBuff";
    public Buff EmptyBuff => new(BuffID) { millisecondsDuration = 100 };

    /// <inheritdoc/>
    public event IHatBuffManagerAPI.HatUnequippedDelegate? OnHatUnEquipped;

    /// <inheritdoc/>
    public event IHatBuffManagerAPI.HatEquippedDelegate? OnHatEquipped;

    public void OnHatChange(NetRef<Hat> field, Hat? oldHat, Hat? newHat)
    {
        if (oldHat != null)
            OnHatUnEquipped?.Invoke(this, new HatUnequippedEventArgs { OldHat = oldHat });

        if (newHat != null)
            OnHatEquipped?.Invoke(this, new HatEquippedEventArgs { NewHat = newHat });
    }

    public HatBuffManager()
    {
        OnHatUnEquipped += DefaultHatUnEquippedEvent;
        OnHatEquipped += DefaultHatEquippedEvent;
    }

    private void DefaultHatUnEquippedEvent(object? sender, IHatUnequippedEventArgs e)
    {
        // Apply an empty buff to override the current hat buff.
        Game1.player.applyBuff(EmptyBuff);
        OnHatUnEquipped?.Invoke(this, new HatUnequippedEventArgs { OldHat = e.OldHat });
    }

    private void DefaultHatEquippedEvent(object? sender, IHatEquippedEventArgs e)
    {
        // Get the hat data and convert it to a buff.
        var hatData = e.NewHat.GetHatData();
        if (hatData == null) return;

        // Apply the buff to player.
        var hatBuff = GetBuffFromHatData(hatData);
        Game1.player.applyBuff(hatBuff);
    }

    private static Buff GetBuffFromHatData(HatData data)
    {
        var buff = new Buff(BuffID);
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
        buff.visible = false;
        buff.millisecondsDuration = Buff.ENDLESS;

        return buff;
    }
}

public class HatUnequippedEventArgs : IHatUnequippedEventArgs
{
    public Hat? OldHat { get; set; }
}

public class HatEquippedEventArgs : IHatEquippedEventArgs
{
    public Hat? NewHat { get; set; }
}