using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using WhyNotJumpInThatMineShaft.Framework;

namespace WhyNotJumpInThatMineShaft;

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
    public static Harmony Harmony { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);
    
    /// <summary>
    /// <see cref="ShaftIndicator"/> mod.
    /// </summary>
    private static ShaftIndicator ShaftIndicator { get; set; }

    #endregion

    /// <summary>
    /// 模组入口点，在模组首次加载后调用。
    /// The mod entry point, called after the mod is first loaded.
    /// </summary>
    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;
        Harmony = new Harmony(Manifest.UniqueID);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnGameLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnGameUnload;

        I18n.Init(Helper.Translation);
        Config = helper.ReadConfig<ModConfig>();
    }

    /// <summary>
    /// 读取模组配置更新并重新载入配置。
    /// Read the update of modconfig and reload them.
    /// </summary>
    private static void ReloadConfig()
    {
        ModHelper.WriteConfig(Config);
        Config = ModHelper.ReadConfig<ModConfig>();
    }

    /****
     ** 事件处理函数
     ** Event handlers
     ****/

    #region Event handlers

    /// <summary>
    /// 在游戏加载时读取配置选项。
    /// Read configs when loading the game.
    /// </summary>
    private static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        GenericModConfigMenuIntegration.Register(Manifest, ModHelper.ModRegistry,
            () => Config,
            () => Config = new ModConfig(),
            ReloadConfig,
            ModMonitor
        );
    }

    /// <summary>
    /// 在游戏加载时配置补丁。
    /// Read configs when loading the game.
    /// </summary>
    private static void OnGameLoaded(object sender, SaveLoadedEventArgs e)
    {
        CommonPatcher.Initialize(Harmony);

        ShaftIndicator = new ShaftIndicator(); 
        ShaftIndicator.Activate();

        ModHelper.Events.Player.Warped += MapScanner.OnMineLevelChanged;
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private static void OnGameUnload(object sender, ReturnedToTitleEventArgs e)
    {
        Harmony.UnpatchAll(Manifest.UniqueID);

        ShaftIndicator.Deactivate();
        ShaftIndicator = null;

        ModHelper.Events.Player.Warped -= MapScanner.OnMineLevelChanged;
    }

    #endregion
}