using System.IO;
using Microsoft.Xna.Framework.Graphics;
using UI.Sprite;

namespace BetterHatsAPI.GuideBook;

/// <summary>
/// Static class holding all texture regions for hat stat icons.
/// </summary>
public static class Texture
{
    public static TextureRegion GetAttrIconByIndex(int index) => index switch
    {
        0 => FarmingIcon,
        1 => FishingIcon,
        2 => ForagingIcon,
        3 => MiningIcon,
        4 => CombatIcon,
        5 => LuckIcon,
        6 => SpeedIcon,
        7 => DefenseIcon,
        8 => ImmunityIcon,
        9 => MaxStaminaIcon,
        10 => MagneticRadiusIcon,
        11 => AttackIcon,
        12 => AttackMultiplierIcon,
        13 => CriticalChanceMultiplierIcon,
        14 => CriticalPowerMultiplierIcon,
        15 => WeaponSpeedMultiplierIcon,
        16 => WeaponPrecisionMultiplierIcon,
        17 => KnockbackMultiplierIcon,
        _ => throw new IndexOutOfRangeException()
    };

    public static readonly Texture2D MenuBackground
        = ModEntry.ModHelper.ModContent.Load<Texture2D>(Path.Combine("assets", "Menu.png"));

    private static readonly Texture2D IconSheet
        = ModEntry.ModHelper.ModContent.Load<Texture2D>(Path.Combine("assets", "AttrIcons.png"));

    private static readonly TextureRegion FarmingIcon =
        new(IconSheet, 0, 0, 20, 20);

    private static readonly TextureRegion FishingIcon =
        new(IconSheet, 20, 0, 20, 20);

    private static readonly TextureRegion ForagingIcon =
        new(IconSheet, 40, 0, 20, 20);

    private static readonly TextureRegion MiningIcon =
        new(IconSheet, 60, 0, 20, 20);

    private static readonly TextureRegion CombatIcon =
        new(IconSheet, 80, 0, 20, 20);

    private static readonly TextureRegion LuckIcon =
        new(IconSheet, 100, 0, 20, 20);

    private static readonly TextureRegion SpeedIcon =
        new(IconSheet, 0, 20, 20, 20);

    private static readonly TextureRegion DefenseIcon =
        new(IconSheet, 20, 20, 20, 20);

    private static readonly TextureRegion ImmunityIcon =
        new(IconSheet, 40, 20, 20, 20);

    private static readonly TextureRegion MaxStaminaIcon =
        new(IconSheet, 60, 20, 20, 20);

    private static readonly TextureRegion MagneticRadiusIcon =
        new(IconSheet, 80, 20, 20, 20);

    private static readonly TextureRegion AttackIcon =
        new(IconSheet, 100, 20, 20, 20);

    private static readonly TextureRegion AttackMultiplierIcon =
        new(IconSheet, 0, 40, 20, 20);

    private static readonly TextureRegion CriticalChanceMultiplierIcon =
        new(IconSheet, 20, 40, 20, 20);

    private static readonly TextureRegion CriticalPowerMultiplierIcon =
        new(IconSheet, 40, 40, 20, 20);

    private static readonly TextureRegion WeaponSpeedMultiplierIcon =
        new(IconSheet, 60, 40, 20, 20);

    private static readonly TextureRegion WeaponPrecisionMultiplierIcon =
        new(IconSheet, 80, 40, 20, 20);

    private static readonly TextureRegion KnockbackMultiplierIcon =
        new(IconSheet, 100, 40, 20, 20);
}