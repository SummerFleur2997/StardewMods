using BetterHatsAPI.API;
using BetterHatsAPI.Framework;
using BetterHatsAPI.Integration;
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
    public static List<IContentPack> ContentPacks { get; private set; }

    #endregion

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;

        I18n.Init(Helper.Translation);
        Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    private void OnGameLaunched(object s, GameLaunchedEventArgs e)
    {
        ContentPacks = Helper.ContentPacks.GetOwned().ToList();

        HatsAPICore.Initialize();
        HatDataHelper.LoadContentPacks();
        GameExtensions.RegisterMethods();
        GuideBookHelper.RegisterEventsForGuideBook(Helper);
        GenericModConfigMenuIntegration.Register(Manifest, ModHelper.ModRegistry, ResetConfig, SaveConfig);

        ModHelper.ConsoleCommands.Add(
            "BHA_Reload",
            "Reload specified content pack by its unique id",
            Reload);
    }

    public override object GetApi(IModInfo mod) => HatsAPICore.Instance;

    private static void Reload(string command, string[] args)
    {
        if (args.Length != 1)
        {
            Log("Usage: BHA_Reload <content pack id>", LogLevel.Error);
            return;
        }

        HatDataHelper.ReloadContentPacks(args[0]);
    }

    /// <summary>
    /// 读取模组配置更新并重新载入配置。
    /// Save the update of modconfig and reload them.
    /// </summary>
    private static void SaveConfig()
    {
        ModHelper.WriteConfig(Config);
        Config = ModHelper.ReadConfig<ModConfig>();
    }

    private static void ResetConfig() => Config = new ModConfig();
}