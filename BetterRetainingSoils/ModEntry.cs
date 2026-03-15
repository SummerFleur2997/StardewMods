using BetterRetainingSoils.API;
using BetterRetainingSoils.Framework;
using BetterRetainingSoils.Patcher;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace BetterRetainingSoils;

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
    private static BrsApi Api { get; } = new();
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

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

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;

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
            Log
        );

        var harmony = new Harmony(Manifest.UniqueID);
        VanillaPatcher.RegisterHarmonyPatches(harmony);
        OtherModsPatcher.RegisterHarmonyPatchesToUI2(harmony);

        ModDataManager.TryCleanLegacyCache();
    }

    #endregion

    public override object GetApi() => Api;
}