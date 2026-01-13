using BetterHatsAPI.API;
using BetterHatsAPI.Framework;
using BetterHatsAPI.GuideBook;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

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

    #endregion

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;

        I18n.Init(Helper.Translation);
        Config = helper.ReadConfig<ModConfig>();

        HatManager.Initialize();
        HatDataHelper.LoadContentPacks(helper);
        TooltipHelper.RegisterEventsForTooltip(helper);
        helper.Events.Input.ButtonReleased += OnButtonReleased;
    }

    private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
    {
        if (e.Button == SButton.G)
            Game1.activeClickableMenu = new GuideMenu(
                (Game1.uiViewport.Width - 1352) / 2,
                (Game1.uiViewport.Height - 792) / 2);
    }

    public override object GetApi(IModInfo mod) => HatManager.Instance;
}