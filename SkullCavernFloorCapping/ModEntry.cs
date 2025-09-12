using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SkullCavernFloorCapping;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    public static IManifest Manifest { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    #endregion

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;

        helper.Events.GameLoop.SaveLoaded += OnGameLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnGameUnload;
    }

    /****
     ** 事件处理函数
     ** Event handlers
     ****/

    #region Event handlers

    /// <summary>
    /// 在游戏加载时配置补丁。
    /// Read configs when loading the game.
    /// </summary>
    private static void OnGameLoaded(object sender, SaveLoadedEventArgs e) => ForceLanding.Activate();

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private static void OnGameUnload(object sender, ReturnedToTitleEventArgs e) => ForceLanding.Deactivate();

    #endregion
}