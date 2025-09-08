using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using WhyNotJumpInThatMineShaft.Framework;

namespace WhyNotJumpInThatMineShaft;

[UsedImplicitly]
public class ModEntry : Mod
{
    public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    public static Harmony Harmony { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

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

        helper.Events.GameLoop.SaveLoaded += OnGameLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnGameUnload;

        I18n.Init(Helper.Translation);
    }
    
    /// <summary>
    /// 在游戏加载时配置补丁。
    /// Read configs when loading the game.
    /// </summary>
    private static void OnGameLoaded(object sender, SaveLoadedEventArgs e)
    {
        ModHelper.Events.Player.Warped += MapScanner.OnMineLevelChanged;
        Patcher.Initialize(Harmony);
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private static void OnGameUnload(object sender, ReturnedToTitleEventArgs e)
    {
        ModHelper.Events.Player.Warped -= MapScanner.OnMineLevelChanged;

        Harmony.UnpatchAll(Manifest.UniqueID);
        Harmony = null;
    }
}