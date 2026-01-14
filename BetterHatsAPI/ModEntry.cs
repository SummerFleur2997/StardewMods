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

    public static ModConfig Config { get; private set; }
    public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);
    public static List<IContentPack> ContentPacks { get; private set; }

    #endregion

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;

        I18n.Init(Helper.Translation);
        Config = helper.ReadConfig<ModConfig>();
        ContentPacks = helper.ContentPacks.GetOwned().ToList();

        HatManager.Initialize();
        HatDataHelper.LoadContentPacks();
        GuideBookHelper.RegisterEventsForGuideBook(helper);
    }

    public override object GetApi(IModInfo mod) => HatManager.Instance;
}