using StardewModdingAPI.Events;
using StardewValley.Monsters;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public partial class HatRelyOnEvents
{
    private static IReflectionHelper Reflection => ModEntry.ModHelper.Reflection;

    private static int _cachedMaxAttack = -2;
    private static int _cachedMaxMtp = -2;

    /// <summary>
    /// When entering a new location with mummy mask worn,
    /// the mummies will be pacified.
    /// </summary>
    private static void MummyMaskLocationChanged(object s, WarpedEventArgs e)
    {
        if (!PlayerHatIs(MummyMaskID))
        {
            ModEntry.ModHelper.Events.Player.Warped -= MummyMaskLocationChanged;
            return;
        }

        var monsters = e.NewLocation.characters.OfType<Mummy>().ToList();
        _cachedMaxAttack = 0;
        _cachedMaxMtp = 0;

        foreach (var mummy in monsters)
        {
            _cachedMaxAttack = Math.Max(_cachedMaxAttack, mummy.DamageToFarmer);
            _cachedMaxMtp = Math.Max(_cachedMaxMtp, mummy.moveTowardPlayerThreshold.Value);
            mummy.DamageToFarmer = 0;
            mummy.moveTowardPlayerThreshold.Value = -2;
            Reflection.GetField<int>(mummy, "_damageToFarmer", false).SetValue(0);
        }
    }

    /// <summary>
    /// For FTM compatible, set the value of new spawned mummies
    /// </summary>
    private static void MummyMaskMonsterSpawned(object s, NpcListChangedEventArgs e)
    {
        if (!PlayerHatIs(MummyMaskID))
        {
            ModEntry.ModHelper.Events.World.NpcListChanged -= MummyMaskMonsterSpawned;
            return;
        }

        var monsters = e.Added.OfType<Mummy>().ToList();

        foreach (var mummy in monsters)
        {
            _cachedMaxAttack = Math.Max(_cachedMaxAttack, mummy.DamageToFarmer);
            _cachedMaxMtp = Math.Max(_cachedMaxMtp, mummy.moveTowardPlayerThreshold.Value);
            mummy.DamageToFarmer = 0;
            mummy.moveTowardPlayerThreshold.Value = -2;
            Reflection.GetField<int>(mummy, "_damageToFarmer", false).SetValue(0);
        }
    }

    /// <summary>
    /// When the player takes off the hat, the mummies will be enraged,
    /// gain double damage.
    /// </summary>
    private static void UpdateForThisLocationWhenDisable()
    {
        if (_cachedMaxAttack <= 0 || _cachedMaxMtp <= 0)
            return;

        var location = Game1.currentLocation;
        var monsters = location.characters.OfType<Mummy>().ToList();

        foreach (var mummy in monsters)
        {
            mummy.DamageToFarmer = _cachedMaxAttack * 2;
            mummy.moveTowardPlayerThreshold.Value = _cachedMaxMtp;
            Reflection.GetField<int>(mummy, "_damageToFarmer", false).SetValue(_cachedMaxAttack * 2);
        }
    }
}