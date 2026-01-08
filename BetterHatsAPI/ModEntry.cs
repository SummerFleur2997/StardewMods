using BetterHatsAPI.API;
using JetBrains.Annotations;

namespace BetterHatsAPI;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    private static readonly HatManager Manager = new();

    #endregion

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;

        HatDataHelper.Initialize();

        Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        Helper.Events.GameLoop.DayStarted += TriggerWhenDayStarted;
        Helper.Events.Player.Warped += TriggerWhenWarped;
    }

    private static void OnSaveLoaded(object s, SaveLoadedEventArgs e)
    {
        Game1.player.hat.fieldChangeEvent += Manager.OnHatChange;

        var hat = Game1.player.hat.Value;
        if (hat is null) return;

        var data = hat.GetHatData();
        Manager.CachedHatData = data;
    }

    private static void OnReturnedToTitle(object s, ReturnedToTitleEventArgs e)
    {
        Game1.player.hat.fieldChangeEvent -= Manager.OnHatChange;
        Manager.CachedHatData = null;
    }

    private static void TriggerWhenWarped(object sender, WarpedEventArgs e)
    {
        var allData = Manager.CachedHatData;
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

    private static void TriggerWhenDayStarted(object sender, DayStartedEventArgs e)
    {
        var allData = Manager.CachedHatData;
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

    public override object GetApi(IModInfo mod) => Manager;
}