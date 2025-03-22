using ConvenientChests.Framework.CategorizeChests;
using ConvenientChests.Framework.CraftFromChests;
using ConvenientChests.Framework.StashToChests;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ConvenientChests.Framework;
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
    
    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        GenericModConfigMenuIntegration.Register(ModManifest, Helper.ModRegistry, Monitor,
            getConfig: () => Config,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );
    }
}
