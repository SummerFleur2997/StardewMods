#nullable enable
using System;
using StardewModdingAPI;

namespace BetterHatsAPI.Framework;

/// <summary>
/// This class represents the data of a hat. It should be
/// deserialized from a JSON file. All the properties are
/// initialized to 0 or an empty string by default. So you
/// only need to specify the properties you want to use.
/// </summary>
[Serializable]
public partial class HatData
{
    internal IContentPack Pack = null!;

    /// <summary>
    /// The unique id for the converted buff. If not specified, this
    /// will be automatically set to the content pack's unique id.
    /// 转换得到的 buff 的唯一 id。若未指定，则自动设置为 content pack 的唯一 id。
    /// </summary>
    public string UniqueBuffID { get; set; } = string.Empty;

    /// <summary>
    /// The description of the converted buff. Will be shown in the
    /// status bar.
    /// 转换得到的 buff 的描述。将会在状态栏中显示。
    /// </summary>
    public string? BuffDescription
    {
        get => Pack.TryGetTranslation(_buffDescription);
        set => _buffDescription = value;
    }

    private string? _buffDescription;

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
    /// Mining level bonus.
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
    /// Attack multiplier bonus.
    /// 伤害倍率加成。
    /// </summary>
    public float AttackMultiplier { get; set; } = 0;

    /// <summary>
    /// Critical chance multiplier bonus.
    /// 暴击倍率加成。
    /// </summary>
    public float CriticalChanceMultiplier { get; set; } = 0;

    /// <summary>
    /// Critical power multiplier bonus.
    /// 暴击力量加成。
    /// </summary>
    public float CriticalPowerMultiplier { get; set; } = 0;

    /// <summary>
    /// Weapon speed multiplier bonus.
    /// 武器速度加成。
    /// </summary>
    public float WeaponSpeedMultiplier { get; set; } = 0;

    /// <summary>
    /// Weapon precision multiplier bonus.
    /// 武器精确度加成。
    /// </summary>
    public float WeaponPrecisionMultiplier { get; set; } = 0;

    /// <summary>
    /// Knockback multiplier bonus.
    /// 武器击退加成。
    /// </summary>
    public float KnockbackMultiplier { get; set; } = 0;

    /// <summary>
    /// A game state query that determines whether the buff should be applied.
    /// 游戏状态查询上下文，用于确定是否应用帽子 buff。
    /// </summary>
    /// <seealso cref="StardewValley.GameStateQuery"/>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use a custom condition. This value should be set by the mod's
    /// API, not by the user.
    /// 是否使用自定义条件检查器。此项应由 mod 的 API 设置。
    /// </summary>
    internal bool UseCustomCondition;

    /// <summary>
    /// The custom condition to check if the condition is met. External C# mods
    /// can use API to set this value.
    /// 自定义条件检查器，外部 SMAPI mod 可以通过 API 设置此值。
    /// </summary>
    internal Func<bool>? CustomConditionChecker;

    /// <summary>
    /// An action to be performed when the condition is met.
    /// 当条件满足时执行的操作。
    /// </summary>
    /// <seealso cref="StardewValley.Triggers.TriggerActionManager"/>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use a custom action. This value should be set by the mod's
    /// API, not by the user.
    /// 是否使用自定义触发事件。此项应由 mod 的 API 设置。
    /// </summary>
    internal bool UseCustomAction;

    /// <summary>
    /// The custom action to be performed when the condition is met. External
    /// C# mods can use API to set this value.
    /// 自定义事件触发器，外部 SMAPI mod 可以通过 API 设置此值。
    /// </summary>
    internal Action? CustomAction;

    /// <summary>
    /// Determines when the condition should be checked.
    /// 确定何时检查条件。
    /// </summary>
    /// <seealso cref="Framework.Trigger"/>
    public Trigger Trigger { get; set; } = Trigger.None;

    /// <summary>
    /// For internal use only. Set custom condition.
    /// </summary>
    internal void SetCustomCondition(Func<bool> condition)
    {
        CustomConditionChecker = condition;
        UseCustomCondition = true;
    }

    /// <summary>
    /// For internal use only. Set custom action.
    /// </summary>
    internal void SetCustomAction(Action action)
    {
        CustomAction = action;
        UseCustomAction = true;
    }
}

/// <summary>
/// An enum representing the different triggers for hat conditions.
/// </summary>
public enum Trigger
{
    /// <summary>Triggered when the current location changes.</summary>
    LocationChanged,

    /// <summary>Triggered when new day starts.</summary>
    DayStarted,

    /// <summary>Trigger with no extra conditions.</summary>
    None
}