#nullable enable
using System.Diagnostics.CodeAnalysis;
using BetterHatsAPI.Framework;
using JetBrains.Annotations;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace BetterHatsAPI.API;

public class HatManager : ISummerFleurBetterHatsAPI
{
    public static readonly HatManager Instance = new();

    /// <inheritdoc/>
    public event ISummerFleurBetterHatsAPI.HatUnequippedDelegate? OnHatUnequipped;

    /// <inheritdoc/>
    public event ISummerFleurBetterHatsAPI.HatEquippedDelegate? OnHatEquipped;

    private List<HatData>? CachedHatData { get; set; }

    private HatManager()
    {
        ModEntry.ModHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        ModEntry.ModHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        ModEntry.ModHelper.Events.GameLoop.DayStarted += TriggerWhenDayStarted;
        ModEntry.ModHelper.Events.Player.Warped += TriggerWhenWarped;
    }

    /// <inheritdoc/>
    public IEnumerable<Buff> GetBuffForThisHat(Hat hat) => hat.GetHatData().Select(d => d.ConvertToBuff()).ToList();

    /// <inheritdoc/>
    [Pure]
    public bool SetCustomConditionChecker(string qualifiedHatID, string packID, Func<bool> customConditionChecker,
        [NotNullWhen(false)] out Exception? ex)
    {
        try
        {
            // check if any hat data exists
            if (!HatDataHelper.AllHatData.TryGetValue(qualifiedHatID, out var data) || data is null)
                throw new Exception($"Can't find any data of {qualifiedHatID}!");

            // check if any data belongs to the target pack
            var targetData = data.FirstOrDefault(d => d.Pack.Manifest.UniqueID == packID);
            if (targetData is null) throw new Exception($"Can't find the data of {qualifiedHatID} in {packID}!");

            targetData.SetCustomCondition(customConditionChecker);
            ModEntry.Log($"Successfully set custom condition checker for {packID} - {qualifiedHatID}.");
            ex = null;
            return true;
        }
        catch (Exception e)
        {
            ex = e;
            ModEntry.Log(e.Message, LogLevel.Warn);
            ModEntry.Log("If you are the content pack author, please check your content! ", LogLevel.Warn);
            return false;
        }
    }

    /// <inheritdoc/>
    [Pure]
    public bool SetCustomActionTrigger(string qualifiedHatID, string packID, Action customActionTrigger,
        [NotNullWhen(false)] out Exception? ex)
    {
        try
        {
            // check if any hat data exists
            if (!HatDataHelper.AllHatData.TryGetValue(qualifiedHatID, out var data) || data is null)
                throw new Exception($"Can't find any data of {qualifiedHatID}!");

            // check if any data belongs to the target pack
            var targetData = data.FirstOrDefault(d => d.Pack.Manifest.UniqueID == packID);
            if (targetData is null) throw new Exception($"Can't find the data of {qualifiedHatID} in {packID}!");

            targetData.SetCustomAction(customActionTrigger);
            ModEntry.Log($"Successfully set custom action trigger for {packID} - {qualifiedHatID}.");
            ex = null;
            return true;
        }
        catch (Exception e)
        {
            ex = e;
            ModEntry.Log(e.Message, LogLevel.Warn);
            ModEntry.Log("If you are the content pack author, please check your content! ", LogLevel.Warn);
            return false;
        }
    }

    private void OnHatChange(NetRef<Hat> field, Hat? oldHat, Hat? newHat)
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

    private void OnSaveLoaded(object? s, SaveLoadedEventArgs e)
    {
        Game1.player.hat.fieldChangeEvent += OnHatChange;

        var hat = Game1.player.hat.Value;
        if (hat is null) return;

        var data = hat.GetHatData();
        CachedHatData = data;
    }

    private void OnReturnedToTitle(object? s, ReturnedToTitleEventArgs e)
    {
        Game1.player.hat.fieldChangeEvent -= OnHatChange;
        CachedHatData = null;
    }

    private void TriggerWhenWarped(object? sender, WarpedEventArgs e)
    {
        var allData = CachedHatData;
        if (allData is null) return;
        foreach (var data in allData)
        {
            if (data.Trigger != Trigger.LocationChanged) return;

            var id = data.UniqueBuffID;
            if (data.CheckCondition())
            {
                if (!Game1.player.hasBuff(id))
                    Game1.player.applyBuff(data.ConvertToBuff());
                data.TryPerformAction();
            }
            else
            {
                Game1.player.applyBuff(data.ConvertToEmptyBuff());
            }
        }
    }

    private void TriggerWhenDayStarted(object? sender, DayStartedEventArgs e)
    {
        var allData = CachedHatData;
        if (allData is null) return;
        foreach (var data in allData)
        {
            if (!data.CheckCondition())
                continue;

            Game1.player.applyBuff(data.ConvertToBuff());
            if (data.Trigger == Trigger.DayStarted)
                data.TryPerformAction();
        }
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

    public HatEquippedEventArgs(Hat newHat) => NewHat = newHat;
}