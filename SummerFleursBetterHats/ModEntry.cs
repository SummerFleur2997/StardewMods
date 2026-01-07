using HarmonyLib;
using JetBrains.Annotations;
using SummerFleursBetterHats.API;
using SummerFleursBetterHats.HatExtensions;
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

    private static readonly HatManager Manager = new();

    #endregion

    public override void Entry(IModHelper helper)
    {
        // Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;
        Harmony = new Harmony(ModManifest.UniqueID);

        GameExtensions.RegisterAll();
        HatDataHelper.Initialize();

        Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        Helper.Events.GameLoop.DayStarted += TriggerWhenDayStarted;
        Helper.Events.Player.Warped += TriggerWhenWarped;

        RegisterAllPatches(Harmony);
    }

    private static void OnSaveLoaded(object s, SaveLoadedEventArgs e)
    {
        Game1.player.hat.fieldChangeEvent += Manager.OnHatChange;

        var hat = Utilities.PlayerHat();
        if (hat is null) return;
        var hatBuff = HatManager.GetBuffAndHatData(hat, out var data);
        if (data.CheckCondition())
            Game1.player.applyBuff(hatBuff);

        Manager.CachedHatData = data;
    }

    private static void OnReturnedToTitle(object s, ReturnedToTitleEventArgs e)
    {
        Game1.player.hat.fieldChangeEvent -= Manager.OnHatChange;
        Manager.CachedHatData = null;
    }

    private static void TriggerWhenWarped(object sender, WarpedEventArgs e)
    {
        var data = Manager.CachedHatData;
        if (data?.Trigger != Trigger.LocationChanged) return;

        if (data.CheckCondition())
        {
            if (!Game1.player.hasBuff(DefaultBuffID))
                Game1.player.applyBuff(data.ConvertToBuff(DefaultBuffID));
            data.TryPerformAction();
        }
        else
        {
            Game1.player.applyBuff(Manager.EmptyBuff);
        }
    }

    private static void TriggerWhenDayStarted(object sender, DayStartedEventArgs e)
    {
        var data = Manager.CachedHatData;
        if (data is null) return;

        if (data.CheckCondition())
        {
            if (!Game1.player.hasBuff(DefaultBuffID))
                Game1.player.applyBuff(data.ConvertToBuff(DefaultBuffID));
            if (data.Trigger == Trigger.DayStarted)
                data.TryPerformAction();
        }
        else
        {
            Game1.player.applyBuff(Manager.EmptyBuff);
        }
    }

    public override object GetApi(IModInfo mod) => Manager;
}