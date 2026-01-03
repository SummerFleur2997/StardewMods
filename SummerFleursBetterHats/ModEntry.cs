using HarmonyLib;
using JetBrains.Annotations;
using SummerFleursBetterHats.API;
using SummerFleursBetterHats.HatExtensions;
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

        GameExtensions.RegisterAll();
        HatDataHelper.Initialize();

        Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        Helper.Events.GameLoop.DayStarted += WhenDayStarted;
        Helper.Events.Player.Warped += WhenWarped;

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

    public override object GetApi(IModInfo mod) => Manager;
}