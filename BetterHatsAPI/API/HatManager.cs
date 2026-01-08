#nullable enable
using Netcode;
using StardewValley.Objects;

namespace BetterHatsAPI.API;

public class HatManager : ISummerFleurBetterHatsAPI
{
    /// <inheritdoc/>
    public event ISummerFleurBetterHatsAPI.HatUnequippedDelegate? OnHatUnequipped;

    /// <inheritdoc/>
    public event ISummerFleurBetterHatsAPI.HatEquippedDelegate? OnHatEquipped;

    internal List<HatData>? CachedHatData { get; set; }

    public void OnHatChange(NetRef<Hat> field, Hat? oldHat, Hat? newHat)
    {
        if (oldHat != null)
        {
            // Get the hat data.
            var allData = oldHat.GetHatData();
            var args = new HatUnequippedEventArgs(oldHat);

            // Clear the cached data and invoke the event
            CachedHatData = null;
            OnHatUnequipped?.Invoke(this, args);

            // Remove the buff from player.
            foreach (var data in allData)
                Game1.player.applyBuff(data.ConvertToEmptyBuff());
        }

        if (newHat != null)
        {
            // Get the hat data and convert it to a buff.
            var allData = newHat.GetHatData();
            var args = new HatEquippedEventArgs(newHat);

            // Cache the data and invoke the event 
            CachedHatData = allData;
            OnHatEquipped?.Invoke(this, args);

            // Apply the buff to player.
            foreach (var data in allData)
                if (data.CheckCondition())
                    Game1.player.applyBuff(data.ConvertToBuff());
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Buff> GetBuffForThisHat(Hat hat) => hat.GetHatData().Select(d => d.ConvertToBuff()).ToList();
}

public class HatUnequippedEventArgs : IHatUnequippedEventArgs
{
    public Hat OldHat { get; }

    public HatUnequippedEventArgs(Hat oldHat) => OldHat = oldHat;
}

public class HatEquippedEventArgs : IHatEquippedEventArgs
{
    public Hat NewHat { get; }

    public HatEquippedEventArgs(Hat newHat) => NewHat = newHat;
}