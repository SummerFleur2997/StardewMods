using ConvenientChests.CategorizeChests;
using ConvenientChests.CraftFromChests;
using ConvenientChests.Framework;
using ConvenientChests.StashToChests;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ConvenientChests;

/// <summary>The mod entry class loaded by SMAPI.</summary>
public class ModEntry : Mod
{
    public ModConfig Config;
    
    internal static IModHelper StaticHelper  { get; private set; }
    internal static IMonitor   StaticMonitor { get; private set; }

    internal static void Log(string s, LogLevel l = LogLevel.Trace) => StaticMonitor.Log(s, l);

    public static StashToNearbyChestsModule StashNearby;
    public static CategorizeChestsModule CategorizeChests;
    public static CraftFromChestsModule CraftFromChests;
    public static StashFromAnywhereModule StashFromAnywhere;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper) {
        I18n.Init(Helper.Translation);
        
        StaticMonitor = Monitor;
        StaticHelper  = Helper;
        
        helper.Events.GameLoop.GameLaunched    += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded      += LoadModules;
        helper.Events.GameLoop.ReturnedToTitle += UnloadModules;
        
        Config = helper.ReadConfig<ModConfig>();
    }
    
    /// <summary>
    /// 读取模组配置更新并重新载入配置。
    /// Read the update of modconfig and reload them.
    /// </summary>
    /// <param name="configStatus">配置选项状态</param>
    /// <param name="module">需要重新载入配置的模组</param>
    public void ReloadConfig(bool configStatus, Module module)
    {
        Config = Helper.ReadConfig<ModConfig>();
        
        switch (configStatus, module.IsActive)
        {
            case (true, false): 
                module.Activate();
                break;
            case (false, true): 
                module.Deactivate();
                break;
        }
    }

    /// <summary>
    /// 在载入游戏存档时加载模组。
    /// Load modules when loading a game save.
    /// </summary>
    private void LoadModules(object sender, SaveLoadedEventArgs e) {
        StashNearby = new StashToNearbyChestsModule(this);
        if (Config.StashToNearbyChests)
            StashNearby.Activate();

        CategorizeChests = new CategorizeChestsModule(this);
        if (Config.CategorizeChests)
            CategorizeChests.Activate();

        CraftFromChests = new CraftFromChestsModule(this);
        if (Config.CraftFromChests)
            CraftFromChests.Activate();
        
        StashFromAnywhere = new StashFromAnywhereModule(this);
        if (Config.StashAnywhere)
            StashFromAnywhere.Activate();
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private void UnloadModules(object sender, ReturnedToTitleEventArgs e) {
        StashNearby.Deactivate();
        StashNearby = null;

        CategorizeChests.Deactivate();
        CategorizeChests = null;

        CraftFromChests.Deactivate();
        CraftFromChests = null;
        
        StashFromAnywhere.Deactivate();
        StashFromAnywhere = null;
    }
    
    /// <summary>
    /// 在游戏加载时读取配置选项
    /// Read configs when loading the game.
    /// </summary>
    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        GenericModConfigMenuIntegration.Register(ModManifest, Helper.ModRegistry, Monitor,
            getConfig: () => Config,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );
    }
}
