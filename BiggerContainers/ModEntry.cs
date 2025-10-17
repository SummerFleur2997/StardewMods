using BiggerContainers.BiggerFridges;
using BiggerContainers.BiggerJunimoChests;
using BiggerContainers.Framework;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BiggerContainers;

[UsedImplicitly]
internal class ModEntry : Mod
{
    public static ModConfig Config { get; private set; }
    public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    public static Harmony Harmony { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    internal static BiggerFridgesModule FridgesModule { get; private set; }
    internal static BiggerJunimoChestsModule JunimoChestsModule { get; private set; }

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;

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

    #region Event handlers
    /// <summary>
    /// 在载入游戏存档时加载模组。
    /// Load modules when loading a game save.
    /// </summary>
    private static void OnGameLoaded(object sender, SaveLoadedEventArgs e)
    {
        Harmony = new Harmony(Manifest.UniqueID);
        FridgesModule = new BiggerFridgesModule();
        FridgesModule.Activate();

        JunimoChestsModule = new BiggerJunimoChestsModule();
        JunimoChestsModule.Activate();

        // PatchShowMenu.RegisterHarmonyPatches(Harmony);
    }

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
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private static void OnGameUnload(object sender, ReturnedToTitleEventArgs e)
    {
        FridgesModule.Deactivate();
        FridgesModule = null;

        JunimoChestsModule.Deactivate();
        JunimoChestsModule = null;

        Harmony.UnpatchAll(Manifest.UniqueID);
        Harmony = null;
    }
    #endregion
}