using ConvenientChests.CategorizeChests;
using ConvenientChests.CategorizeChests.Interface.Widgets;
using ConvenientChests.CraftFromChests;
using ConvenientChests.StashToChests;
using StardewModdingAPI;

namespace ConvenientChests {
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        public static Config Config = null!;
        internal static IModHelper StaticHelper  { get; private set; }
        internal static IMonitor   StaticMonitor { get; private set; }

        internal static void Log(string s, LogLevel l = LogLevel.Trace) => StaticMonitor.Log(s, l);

        public static StashToNearbyChestsModule StashNearby;
        public static CategorizeChestsModule    CategorizeChests;
        public static CraftFromChestsModule     CraftFromChests;
        public static StashFromAnywhereModule   StashFromAnywhere;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            I18n.Init(Helper.Translation);
            
            StaticMonitor = Monitor;
            StaticHelper  = Helper;

            helper.Events.GameLoop.GameLaunched    += (_, _) => RegisterSettings();
            helper.Events.GameLoop.SaveLoaded      += (_, _) => LoadModules();
            helper.Events.GameLoop.ReturnedToTitle += (_, _) => UnloadModules();
            
            Config = helper.ReadConfig<Config>();
        }

        private void LoadModules() {
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

        private void UnloadModules() {
            StashNearby.Deactivate();
            StashNearby = null;

            CategorizeChests.Deactivate();
            CategorizeChests = null;

            CraftFromChests.Deactivate();
            CraftFromChests = null;
            
            StashFromAnywhere.Deactivate();
            StashFromAnywhere = null;
        }

        public override object GetApi() {
            return new ModAPI();
        }

        private void RegisterSettings() {
            {
                // get Generic Mod Config Menu's API (if it's installed)
                var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                if (configMenu is null)
                    return;

                // register mod
                var options = new ModConfigOptions(configMenu, ModManifest);
                options.Register(
                                 reset: () => Config = new Config(),
                                 save: () => Helper.WriteConfig(Config)
                                );

                options.AddSection(I18n.Config_Categorize_Title(), I18n.Config_Categorize_Desc());
                options.Add(() => Config.CategorizeChests,
                            value => Config.CategorizeChests = value,
                            I18n.Config_Active());
                options.Add(() => Config.EnableSort,
                            value => Config.EnableSort = value,
                            I18n.Config_Categorize_Sort());

                options.AddSection(I18n.Config_CraftFromChest_Title(), I18n.Config_CraftFromChest_Desc());
                options.Add(() => Config.CraftFromChests,
                            value => Config.CraftFromChests = value,
                            I18n.Config_Active());
                options.Add(() => Config.CraftRadius,
                            value => Config.CraftRadius = value,
                            I18n.Config_Radius());

                options.AddSection(I18n.Config_StashToNearby_Title(), I18n.Config_StashToNearby_Desc());
                options.Add(() => Config.StashToNearbyChests,
                            value => Config.StashToNearbyChests = value,
                            I18n.Config_Active());
                options.Add(() => Config.StashRadius,
                            value => Config.StashRadius = value,
                            I18n.Config_Radius());
                options.Add(() => Config.StashKey,
                            value => Config.StashKey = value,
                            I18n.Config_StashToNearby_Key());

                options.AddSection(I18n.Config_StashAnywhere_Title(), I18n.Config_StashAnywhere_Desc());
                options.Add(() => Config.StashAnywhere,
                            value => Config.StashAnywhere = value,
                            I18n.Config_Active());
                options.Add(() => Config.StashAnywhereToFridge,
                            value => Config.StashAnywhereToFridge = value,
                            I18n.Config_StashAnywhere_Fridge());
                options.Add(() => Config.StashToExistingStacks,
                            value => Config.StashToExistingStacks = value,
                            I18n.Config_StashAnywhere_Exist());
                options.Add(() => Config.StashAnywhereKey,
                            value => Config.StashAnywhereKey = value,
                            I18n.Config_StashAnywhere_Key());
            }
        }
    }
}