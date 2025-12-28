#nullable enable
using System;
using StardewModdingAPI.Events;
using StardewValley;
using SummerFleursBetterHats.API;

namespace SummerFleursBetterHats.HatWithConditions;

public static partial class HatWithConditions
{
    private static Func<bool>? _conditionChecker;

    public static void RegisterConditionChecker(object? sender, IHatEquippedEventArgs e)
    {
        var hat = e.NewHat;
        switch (hat.QualifiedItemId)
        {
            case PartyHatRedID:
            case PartyHatBlueID:
            case PartyHatGreenID:
                e.ShouldApply = CheckConditionForPartyHat();
                _conditionChecker = CheckConditionForPartyHat;
                ModEntry.ModHelper.Events.Player.Warped += CheckAndApplyWhenWarped;
                break;
            default:
                return;
        }
    }

    public static void UnRegisterConditionChecker(object? sender, IHatUnequippedEventArgs e)
    {
        var hat = e.OldHat;
        switch (hat.QualifiedItemId)
        {
            case PartyHatRedID:
            case PartyHatBlueID:
            case PartyHatGreenID:
                _conditionChecker = null;
                ModEntry.ModHelper.Events.Player.Warped -= CheckAndApplyWhenWarped;
                break;
            default:
                return;
        }
    }

    private static void CheckAndApplyWhenWarped(object? sender, WarpedEventArgs e)
    {
        if (_conditionChecker?.Invoke() == true) ApplyBuffInstantly();
        else Game1.player.applyBuff(ModEntry.Manager.EmptyBuff);
    }

    private static void ApplyBuffInstantly()
    {
        var hat = Game1.player.hat.Value;
        var hatBuff = ModEntry.Manager.GetBuffFromHatData(hat);
        Game1.player.applyBuff(hatBuff);
    }
}