using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SummerFleursBetterHats.API;
using static SummerFleursBetterHats.HatWithConditions.HatWithConditions;
using static SummerFleursBetterHats.HatWithPatches.HatWithPatches;

namespace SummerFleursBetterHats;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    // public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static Harmony Harmony { get; set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    public static readonly HatManager Manager = new();

    #endregion

    public override void Entry(IModHelper helper)
    {
        // Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;
        Harmony = new Harmony(ModManifest.UniqueID);

        Helper.Events.GameLoop.SaveLoaded += RegisterHatChangeEvents;
        Helper.Events.GameLoop.ReturnedToTitle += UnRegisterHatChangeEvents;
        Helper.Events.GameLoop.ReturnedToTitle += UnRegisterAllConditionCheckers;
        Helper.Events.GameLoop.DayStarted += OnDayStart;
        Manager.OnHatEquipped += RegisterConditionChecker;
        Manager.OnHatUnequipped += UnRegisterConditionChecker;
        RegisterPatchForChefHat(Harmony);
        RegisterPatchForGarbageHat(Harmony);
        RegisterPatchForTruckerHat(Harmony);
        RegisterPatchForGoblinMask(Harmony);
    }

    private static void OnDayStart(object s, DayStartedEventArgs e)
    {
        var hat = Game1.player.hat;
        if (hat?.Value == null) return;

        // Simulate a hat change event to apply all codes.
        Manager.OnHatChange(hat, hat.Value, hat.Value);
        ManualInvokeActionIfNecessary();
    }

    private static void RegisterHatChangeEvents(object s, SaveLoadedEventArgs e)
        => Game1.player.hat.fieldChangeEvent += Manager.OnHatChange;

    private static void UnRegisterHatChangeEvents(object s, ReturnedToTitleEventArgs e)
        => Game1.player.hat.fieldChangeEvent -= Manager.OnHatChange;

    public override object GetApi(IModInfo mod) => Manager;
}