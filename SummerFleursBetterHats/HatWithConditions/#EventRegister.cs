#nullable enable
using System;
using StardewModdingAPI.Events;
using StardewValley;
using SummerFleursBetterHats.API;

namespace SummerFleursBetterHats.HatWithConditions;

/// <summary>
/// A condition system for those hats that have special conditions.
/// </summary>
public static partial class HatWithConditions
{
    private static Func<bool>? _conditionChecker;
    private static Action? _action;

    public static void ManualInvokeActionIfNecessary() => _action?.Invoke();

    /// <summary>
    /// Registers the condition checker for the hat based on its QualifiedItemId.
    /// </summary>
    public static void RegisterConditionChecker(object? sender, IHatEquippedEventArgs e)
    {
        var hat = e.NewHat;
        // Unregister the condition checker to prevent multiple registrations.
        UnRegisterConditionCheckers();
        switch (hat.QualifiedItemId)
        {
            case EarmuffsID:
                e.ShouldApply = CheckConditionForEarmuffs();
                _conditionChecker = CheckConditionForEarmuffs;
                ModEntry.ModHelper.Events.GameLoop.DayStarted += CheckAndApplyWhenDayStarted;
                break;
            case GoodOlCapID:
                e.ShouldApply = CheckConditionForGoodOlCap();
                _action = GoodOlCap_AddMoney;
                _conditionChecker = CheckConditionForGoodOlCap;
                ModEntry.ModHelper.Events.GameLoop.DayStarted += CheckAndApplyWhenDayStarted;
                break;
            case PaperHatID:
                e.ShouldApply = CheckConditionForPaperHat();
                _conditionChecker = CheckConditionForPaperHat;
                ModEntry.ModHelper.Events.Player.Warped += CheckAndApplyWhenWarped;
                break;
            case PartyHatRedID:
            case PartyHatBlueID:
            case PartyHatGreenID:
                e.ShouldApply = CheckConditionForPartyHat();
                _conditionChecker = CheckConditionForPartyHat;
                ModEntry.ModHelper.Events.Player.Warped += CheckAndApplyWhenWarped;
                break;
            case SantaHatID:
                e.ShouldApply = CheckConditionForSantaHat();
                _action = SantaHat_AddMysteryBox;
                _conditionChecker = CheckConditionForSantaHat;
                ModEntry.ModHelper.Events.GameLoop.DayStarted += CheckAndApplyWhenDayStarted;
                break;
            case SouwesterID:
                e.ShouldApply = CheckConditionForSouwester();
                _conditionChecker = CheckConditionForSouwester;
                ModEntry.ModHelper.Events.Player.Warped += CheckAndApplyWhenWarped;
                break;
            default:
                return;
        }
    }

    /// <summary>
    /// Unregisters the condition checker when taking off the hat.
    /// </summary>
    public static void UnRegisterConditionChecker(object? sender, IHatUnequippedEventArgs e)
        => UnRegisterConditionCheckers();

    /// <summary>
    /// Unregister all condition checkers when returning to title to prevent potential bugs.
    /// </summary>
    public static void UnRegisterAllConditionCheckers(object? sender, ReturnedToTitleEventArgs e)
        => UnRegisterConditionCheckers();

    private static void CheckAndApplyWhenWarped(object? sender, WarpedEventArgs e)
    {
        if (_conditionChecker?.Invoke() == true) ApplyBuffInstantly();
        else Game1.player.applyBuff(ModEntry.Manager.EmptyBuff);
    }

    private static void CheckAndApplyWhenDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (_conditionChecker?.Invoke() != true) return;

        ApplyBuffInstantly();
        _action?.Invoke();
    }

    private static void ApplyBuffInstantly()
    {
        var hat = Game1.player.hat.Value;
        var hatBuff = ModEntry.Manager.GetBuffFromHatData(hat);
        Game1.player.applyBuff(hatBuff);
    }

    private static void UnRegisterConditionCheckers()
    {
        ModEntry.ModHelper.Events.Player.Warped -= CheckAndApplyWhenWarped;
        ModEntry.ModHelper.Events.GameLoop.DayStarted -= CheckAndApplyWhenDayStarted;
        _action = null;
        _conditionChecker = null;
    }
}