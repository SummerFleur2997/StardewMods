using BetterHatsAPI.API;
using BetterHatsAPI.Framework;
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

    #endregion

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;

        HatDataHelper.LoadContentPacks(helper);
        TooltipHelper.RegisterEventsForTooltip(helper);
    }

    public override object GetApi(IModInfo mod) => HatManager.Instance;
}