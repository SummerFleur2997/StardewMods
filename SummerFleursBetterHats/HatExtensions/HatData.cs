using System;

namespace SummerFleursBetterHats.HatExtensions;

[Serializable]
public class HatData
{
    /// <summary>
    /// Combat level bonus.
    /// 战斗技能等级加成。
    /// </summary>
    public float CombatLevel { get; set; } = 0;

    /// <summary>
    /// Farmer level bonus.
    /// 耕种技能等级加成。
    /// </summary>
    public float FarmingLevel { get; set; } = 0;

    /// <summary>
    /// Fishing level bonus.
    /// 钓鱼技能等级加成。
    /// </summary>
    public float FishingLevel { get; set; } = 0;

    /// <summary>
    /// Minig level bonus.
    /// 采矿技能等级加成。
    /// </summary>
    public float MiningLevel { get; set; } = 0;

    /// <summary>
    /// Luck level bonus.
    /// 运气等级加成。
    /// </summary>
    public float LuckLevel { get; set; } = 0;

    /// <summary>
    /// Foraging level bonus.
    /// 采集技能等级加成。
    /// </summary>
    public float ForagingLevel { get; set; } = 0;

    /// <summary>
    /// Max stamina bonus.
    /// 最大体力加成。
    /// </summary>
    public float MaxStamina { get; set; } = 0;

    /// <summary>
    /// Magnetic radius bonus.
    /// 磁力半径加成。
    /// </summary>
    public float MagneticRadius { get; set; } = 0;

    /// <summary>
    /// Movement speed bonus.
    /// 移动速度加成。
    /// </summary>
    public float Speed { get; set; } = 0;

    /// <summary>
    /// Attack bonus.
    /// 攻击加成。
    /// </summary>
    public float Attack { get; set; } = 0;

    /// <summary>
    /// Defense bonus.
    /// 防御加成。
    /// </summary>
    public float Defense { get; set; } = 0;

    /// <summary>
    /// Immunity bonus.
    /// 免疫加成。
    /// </summary>
    public float Immunity { get; set; } = 0;

    /// <summary>
    /// A game query context that determines whether the buff is applied.
    /// </summary>
    public string QueryContext { get; set; }
}