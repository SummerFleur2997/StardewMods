#nullable enable
using BetterHatsAPI.Framework;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using static BetterHatsAPI.Framework.Utilities;

namespace BetterHatsAPI.API;

public class HatManager : ISummerFleurBetterHatsAPI
{
    public static readonly HatManager Instance = new();

    /// <inheritdoc/>
    public event ISummerFleurBetterHatsAPI.HatUnequippedDelegate? OnHatUnequipped;

    /// <inheritdoc/>
    public event ISummerFleurBetterHatsAPI.HatEquippedDelegate? OnHatEquipped;

    private List<HatData> CachedHatData { get; } = new();
    private List<HatData> CachedHatDataForOneSecondUpdateTicked { get; } = new();
    private List<HatData> CachedHatDataForUpdateTicked { get; } = new();

    private HatManager()
    {
        ModEntry.ModHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        ModEntry.ModHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        ModEntry.ModHelper.Events.GameLoop.DayStarted += TriggerWhenDayStarted;
        ModEntry.ModHelper.Events.Player.Warped += TriggerWhenWarped;
        ModEntry.ModHelper.Events.GameLoop.TimeChanged += TriggerWhenTimeChanged;
    }

    internal static void Initialize() => _ = Instance;

    /// <inheritdoc/>
    public IEnumerable<Buff> GetBuffForThisHat(Hat hat) => hat.GetHatData().Select(d => d.ConvertToBuff()).ToList();

    /// <inheritdoc/>
    public void SetCustomConditionChecker(string qualifiedHatID, string packID, Func<bool> customConditionChecker)
    {
        try
        {
            // check if any hat data exists
            if (!HatDataHelper.AllHatData.TryGetValue(qualifiedHatID, out var data) || data is null)
                throw new Exception($"Can't find any data of {qualifiedHatID}!");

            // check if any data belongs to the target pack
            var targetData = data.FirstOrDefault(d => d.ID == packID);
            if (targetData is null)
                throw new Exception($"Can't find the data of {qualifiedHatID} in {packID}!");

            targetData.SetCustomCondition(customConditionChecker);
            targetData.Condition = HatData.CustomConditionSign;
            Log($"Successfully set custom condition checker for {packID} - {qualifiedHatID}.");
        }
        catch (Exception e)
        {
            Error($"An error occured when trying to set custom condition checker for {packID}!");
            Warn(e.Message);
            Warn("If you are the content pack author, please check your content! ");
        }
    }

    /// <inheritdoc/>
    public void SetCustomActionTrigger(string qualifiedHatID, string packID, Action customActionTrigger)
    {
        try
        {
            // check if any hat data exists
            if (!HatDataHelper.AllHatData.TryGetValue(qualifiedHatID, out var data) || data is null)
                throw new Exception($"Can't find any data of {qualifiedHatID}!");

            // check if any data belongs to the target pack
            var targetData = data.FirstOrDefault(d => d.ID == packID);
            if (targetData is null)
                throw new Exception($"Can't find the data of {qualifiedHatID} in {packID}!");

            targetData.SetCustomAction(customActionTrigger);
            targetData.Action = HatData.CustomActionSign;
            Log($"Successfully set custom action trigger for {packID} - {qualifiedHatID}.");
        }
        catch (Exception e)
        {
            Error($"An error occured when trying to set custom action trigger for {packID}!");
            Warn(e.Message);
            Warn("If you are the content pack author, please check your content! ");
        }
    }

    /// <inheritdoc/>
    public void SetCustomBuffModifier(string qualifiedHatID, string packID, Action<Buff> customModifier)
    {
        try
        {
            // check if any hat data exists
            if (!HatDataHelper.AllHatData.TryGetValue(qualifiedHatID, out var data) || data is null)
                throw new Exception($"Can't find any data of {qualifiedHatID}!");

            // check if any data belongs to the target pack
            var targetData = data.FirstOrDefault(d => d.ID == packID);
            if (targetData is null)
                throw new Exception($"Can't find the data of {qualifiedHatID} in {packID}!");

            targetData.CustomModifier = customModifier;
            Log($"Successfully set custom buff modifier for {packID} - {qualifiedHatID}.");
        }
        catch (Exception e)
        {
            Error($"An error occured when trying to set custom buff modifier for {packID}!");
            Warn(e.Message);
            Warn("If you are the content pack author, please check your content! ");
        }
    }

    private void OnHatChange(NetRef<Hat> field, Hat? oldHat, Hat? newHat)
    {
        Dispose();
        if (oldHat != null)
        {
            // Get the hat data.
            var allData = oldHat.GetHatData();
            var args = new HatUnequippedEventArgs(oldHat);

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

            OnHatEquipped?.Invoke(this, args);
            InitializeHatData(allData);
        }
    }

    /// <summary>
    /// Initialize the hat data. Remember to call <see cref="Dispose"/>
    /// before calling this method.
    /// </summary>
    private void InitializeHatData(List<HatData> allData)
    {
        // Fill the cache and invoke the event
        foreach (var data in allData)
        {
            switch (data.Trigger)
            {
                case Trigger.TickUpdated when ModEntry.Config.DisableTickUpdateChecker:
                case Trigger.SecondUpdated:
                    CachedHatDataForOneSecondUpdateTicked.Add(data);
                    break;
                case Trigger.TickUpdated:
                    CachedHatDataForUpdateTicked.Add(data);
                    break;
                default:
                    CachedHatData.Add(data);
                    break;
            }

            // Apply the buff to player.
            if (data.TryCheckCondition()) Game1.player.applyBuff(data.ConvertToBuff());
        }

        // Register events if needed
        if (CachedHatDataForUpdateTicked.Any())
        {
            ModEntry.ModHelper.Events.GameLoop.UpdateTicked += TriggerWhenUpdateTicked;
            Log("Registered UpdateTicked event for these packs:");
            foreach (var data in CachedHatDataForUpdateTicked)
                Log($" - {data.ID}");
        }

        if (CachedHatDataForOneSecondUpdateTicked.Any())
        {
            ModEntry.ModHelper.Events.GameLoop.OneSecondUpdateTicked += TriggerWhenOneSecondUpdateTicked;
            Log("Registered OneSecondUpdateTicked event for these packs:");
            foreach (var data in CachedHatDataForUpdateTicked)
                Log($" - {data.ID}");
        }
    }

    private void OnSaveLoaded(object? s, SaveLoadedEventArgs e)
    {
        Game1.player.hat.fieldChangeEvent += OnHatChange;

        var hat = Game1.player.hat.Value;
        if (hat is null) return;

        var allData = hat.GetHatData();
        InitializeHatData(allData);
        OnHatEquipped?.Invoke(this, new HatEquippedEventArgs(hat, true));
    }

    private void OnReturnedToTitle(object? s, ReturnedToTitleEventArgs e)
    {
        Game1.player.hat.fieldChangeEvent -= OnHatChange;
        Dispose();
    }

    /// <summary>
    /// Raised after the game state is updated (≈60 times per second).
    /// This event is marked as a high-cost event, only if there are at
    /// least 1 content packs that use this event, it will be triggered.
    /// </summary>
    /// <remarks>
    /// If the user toggled <see cref="ModConfig.DisableTickUpdateChecker"/>
    /// in the config, this event will *convert to*
    /// <see cref="Trigger.SecondUpdated"/>
    /// </remarks>
    private void TriggerWhenUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        foreach (var data in CachedHatDataForUpdateTicked)
        {
            var id = data.UniqueBuffID;
            if (data.TryCheckCondition())
            {
                if (!Game1.player.hasBuff(id) || data.Dynamic)
                    Game1.player.applyBuff(data.ConvertToBuff());

                if (!string.IsNullOrWhiteSpace(data.Action))
                    data.TryPerformAction();
            }
            else
            {
                Game1.player.applyBuff(data.ConvertToEmptyBuff());
            }
        }
    }

    /// <summary>
    /// Raised once per second after the game state is updated.
    /// This event is marked as a high-cost event, only if there are at
    /// least 1 content packs that use this event, it will be triggered.
    /// </summary>
    private void TriggerWhenOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        foreach (var data in CachedHatDataForOneSecondUpdateTicked)
        {
            var id = data.UniqueBuffID;
            if (data.TryCheckCondition())
            {
                if (!Game1.player.hasBuff(id) || data.Dynamic)
                    Game1.player.applyBuff(data.ConvertToBuff());

                if (!string.IsNullOrWhiteSpace(data.Action))
                    data.TryPerformAction();
            }
            else
            {
                Game1.player.applyBuff(data.ConvertToEmptyBuff());
            }
        }
    }

    /// <summary>
    /// Raised after the in-game clock time changes.
    /// </summary>
    private void TriggerWhenTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        foreach (var data in CachedHatData)
        {
            if (data.Trigger != Trigger.TimeChanged)
                continue;

            var id = data.UniqueBuffID;
            if (data.TryCheckCondition())
            {
                if (!Game1.player.hasBuff(id) || data.Dynamic)
                    Game1.player.applyBuff(data.ConvertToBuff());

                if (string.IsNullOrWhiteSpace(data.Action))
                    continue;

                data.TryPerformAction();
                Log($"Successfully performed action for {data.ID} in TimeChanged event.");
            }
            else
            {
                Game1.player.applyBuff(data.ConvertToEmptyBuff());
            }
        }
    }

    /// <summary>
    /// Raised after the player warps to a new location. 
    /// </summary>
    private void TriggerWhenWarped(object? sender, WarpedEventArgs e)
    {
        foreach (var data in CachedHatData)
        {
            if (data.Trigger != Trigger.LocationChanged)
                continue;

            var id = data.UniqueBuffID;
            if (data.TryCheckCondition())
            {
                if (!Game1.player.hasBuff(id) || data.Dynamic)
                    Game1.player.applyBuff(data.ConvertToBuff());

                if (string.IsNullOrWhiteSpace(data.Action))
                    continue;

                data.TryPerformAction();
                Log($"Successfully performed action for {data.ID} in LocationChanged event.");
            }
            else
            {
                Game1.player.applyBuff(data.ConvertToEmptyBuff());
            }
        }
    }

    private void TriggerWhenDayStarted(object? sender, DayStartedEventArgs e)
    {
        foreach (var data in CachedHatData)
        {
            if (!data.TryCheckCondition())
                continue;

            Game1.player.applyBuff(data.ConvertToBuff());
            if (data.Trigger != Trigger.DayStarted)
                continue;

            data.TryPerformAction();
            Log($"Successfully performed action for {data.ID} in DayStarted event.");
        }
    }

    /// <summary>
    /// Private use only, not inherit from IDisposable.
    /// </summary>
    private void Dispose()
    {
        // Clear cache
        CachedHatData.Clear();
        CachedHatDataForUpdateTicked.Clear();
        CachedHatDataForOneSecondUpdateTicked.Clear();

        // Unregister events
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked -= TriggerWhenUpdateTicked;
        ModEntry.ModHelper.Events.GameLoop.OneSecondUpdateTicked -= TriggerWhenOneSecondUpdateTicked;
        Log("High-cost events are all unregistered.");
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
    public bool InvokedWhenSaveLoaded { get; }

    public HatEquippedEventArgs(Hat newHat, bool invokedWhenSaveLoaded = false)
    {
        NewHat = newHat;
        InvokedWhenSaveLoaded = invokedWhenSaveLoaded;
    }
}