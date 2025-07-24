using System.Linq;
using BetterRetainingSoils.DirtService;
using BetterRetainingSoils.Framework;
using BetterRetainingSoils.Framework.MultiplayerService;
using BetterRetainingSoils.Framework.SaveService;
using BetterRetainingSoils.Patcher;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

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
    private static Harmony Harmony { get; set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    /// <summary>
    /// <see cref="MultiplayerServer"/> module.
    /// </summary>
    private static MultiplayerServer MultiplayerServer { get; set; }
    #endregion

    /// <summary>
    /// 模组入口点，在模组首次加载后调用。
    /// The mod entry point, called after the mod is first loaded.
    /// </summary>
    public override void Entry(IModHelper helper)
    {
        I18n.Init(Helper.Translation);

        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnGameLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnGameUnload;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.Saving += OnSaving;

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
    /// 在载入游戏存档时加载模组。
    /// Load modules when loading a game save.
    /// </summary>
    private static void OnGameLoaded(object sender, SaveLoadedEventArgs e)
    {
        Harmony = new Harmony(Manifest.UniqueID);
        WaterRetentionPatches.RegisterHarmonyPatches(Harmony);
        UI2Patches.RegisterHarmonyPatches(Harmony);

        SaveManager.Load();

        MultiplayerServer = new MultiplayerServer();
        if (Context.IsMultiplayer)
            MultiplayerServer.Activate();
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
            ModMonitor
        );
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private static void OnGameUnload(object sender, ReturnedToTitleEventArgs e)
    {
        Harmony.UnpatchAll(Manifest.UniqueID);
        Harmony = null;

        if (MultiplayerServer.IsActive)
            MultiplayerServer.Deactivate();
    }

    /// <summary>
    /// 在每日开始时更新耕地信息。
    /// Update hoedirt data when a new day started.
    /// </summary>
    private static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;
        Utility.ForEachLocation(delegate(GameLocation location)
        {
            var hoeDirts = location.terrainFeatures.Pairs
                .Select(pair => pair.Value)
                .OfType<HoeDirt>()
                .Where(h => h.state.Value != 2)
                .Where(h => h.IsAvailable());

            foreach (var hoeDirt in hoeDirts) hoeDirt.DayUpdate();
            return true;
        });
    }

    /// <summary>
    /// 在游戏开始对存档进行保存之前触发。
    /// Raised before the game begins writes data to the save file (except the initial save creation).
    /// </summary>
    private static void OnSaving(object sender, SavingEventArgs e)
    {
        SaveManager.Save();
    }
    #endregion
}