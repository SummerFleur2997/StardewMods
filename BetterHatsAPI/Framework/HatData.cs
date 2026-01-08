using StardewValley.Triggers;

namespace BetterHatsAPI.Framework;

/// <summary>
/// This class represents the data of a hat. It should be
/// deserialized from a JSON file. All the properties are
/// initialized to 0 or an empty string by default. So you
/// only need to specify the properties you want to use.
/// </summary>
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
    /// A game state query that determines whether the buff should be applied.
    /// 游戏状态查询上下文，用于确定是否应用帽子 buff。
    /// </summary>
    /// <seealso cref="StardewValley.GameStateQuery"/>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// An action to be performed when the condition is met.
    /// 当条件满足时执行的操作。
    /// </summary>
    /// <seealso cref="StardewValley.Triggers.TriggerActionManager"/>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Determines when the condition should be checked.
    /// 确定何时检查条件。
    /// </summary>
    /// <seealso cref="Framework.Trigger"/>
    public Trigger Trigger { get; set; } = Trigger.None;

    /// <summary>
    /// Convert this hat data to a buff.
    /// </summary>
    /// <param name="buffId">The id of the buff.</param>
    public Buff ConvertToBuff(string buffId)
    {
        var buff = new Buff(buffId);

        buff.effects.CombatLevel.Value = CombatLevel;
        buff.effects.FarmingLevel.Value = FarmingLevel;
        buff.effects.FishingLevel.Value = FishingLevel;
        buff.effects.MiningLevel.Value = MiningLevel;
        buff.effects.LuckLevel.Value = LuckLevel;
        buff.effects.ForagingLevel.Value = ForagingLevel;
        buff.effects.MaxStamina.Value = MaxStamina;
        buff.effects.MagneticRadius.Value = MagneticRadius;
        buff.effects.Speed.Value = Speed;
        buff.effects.Attack.Value = Attack;
        buff.effects.Defense.Value = Defense;
        buff.effects.Immunity.Value = Immunity;
#if RELEASE
        buff.visible = false;
#endif
        buff.millisecondsDuration = Buff.ENDLESS;
        return buff;
    }

    /// <summary>
    /// Checks if the condition is met.
    /// </summary>
    /// <returns>
    /// True, if the trigger is None or the condition is met;
    /// False otherwise.
    /// </returns>
    public bool CheckCondition() => string.IsNullOrWhiteSpace(Condition) || GameStateQuery.CheckConditions(Condition);

    /// <summary>
    /// Try to perform the action. If something goes wrong, log the error.
    /// </summary>
    public void TryPerformAction()
    {
        if (string.IsNullOrWhiteSpace(Action)) return;
        TriggerActionManager.TryRunAction(Action, out var error, out var ex);
        if (ex != null)
            ModEntry.Log($"Error while performing action '{Action}': \n{error}", LogLevel.Warn);
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