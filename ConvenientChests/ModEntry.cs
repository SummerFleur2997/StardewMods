#nullable disable
using Common;
using ConvenientChests.AliasForChests;
using ConvenientChests.API;
using ConvenientChests.CategorizeChests;
using ConvenientChests.CraftFromChests;
using ConvenientChests.Framework;
using ConvenientChests.Framework.DataService;
using ConvenientChests.Framework.IntegrationService;
using ConvenientChests.Framework.MigrateService;
using ConvenientChests.Framework.MultiplayerService;
using ConvenientChests.Framework.UserInterfaceService;
using ConvenientChests.StashToChests;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace ConvenientChests;

/// <summary>
/// The mod entry class loaded by SMAPI.
/// </summary>
[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    // public static bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;
    public static ModConfig Config { get; private set; }
    public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }

    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    public static readonly ChestAPI ChestApi = new();

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
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.Display.MenuChanged += OnMenuChanged;

        I18n.Init(Helper.Translation);
        Config = helper.ReadConfig<ModConfig>();
    }

    public override object GetApi(IModInfo mod) => ChestApi;

    /****
     ** 私有方法
     ** Private Methods
     ****/

    #region Private Methods

    /// <summary>
    /// 读取模组配置更新并重新载入配置。
    /// Save the update of modconfig and reload them.
    /// </summary>
    private static void SaveConfig()
    {
        ModHelper.WriteConfig(Config);
        Config = ModHelper.ReadConfig<ModConfig>();
        if (!Game1.hasLoadedGame) return;

        UpdateModule(Config.AliasForChests, AliasForChestsModule.Instance);
        UpdateModule(Config.CategorizeChests, CategorizeChestsModule.Instance);
        UpdateModule(Config.CraftFromChests, CraftFromChestsModule.Instance);
        UpdateModule(Config.StashToNearby || Config.StashAnywhere, StashToChestsModule.Instance);
        StashToChestsModule.Instance.CreateJudgementFunction();

        return;

        void UpdateModule(bool configStatus, IModule module)
        {
            if (configStatus == module.IsActive) return;

            switch (configValue: configStatus, module.IsActive)
            {
                case (true, false):
                    module.Activate();
                    break;
                case (false, true):
                    module.Deactivate();
                    break;
            }
        }
    }

    private static void ResetConfig() => Config = new ModConfig();

    #endregion

    /****
     ** 事件处理函数
     ** Event handlers
     ****/

    #region Event handlers

    /// <summary>
    /// 在载入游戏存档时加载模组。
    /// Load modules when loading a game save.
    /// </summary>
    private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        if (Config.AliasForChests)
            AliasForChestsModule.Instance.Activate();

        if (Config.CategorizeChests)
            CategorizeChestsModule.Instance.Activate();

        if (Config.CraftFromChests)
            CraftFromChestsModule.Instance.Activate();

        if (Config.StashAnywhere || Config.StashToNearby)
            StashToChestsModule.Instance.Activate();

        SaveManager.Load();

        if (Context.IsMultiplayer)
        {
            MultiplayerServer.Instance.Activate();
        }

#if DEBUG
        Debug.Init();
#endif
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private static void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        AliasForChestsModule.Instance.Deactivate();
        CategorizeChestsModule.Instance.Deactivate();
        CraftFromChestsModule.Instance.Deactivate();
        StashToChestsModule.Instance.Deactivate();
        MultiplayerServer.Instance.Deactivate();

        ChestManager.ClearChestData();
    }

    /// <summary>
    /// 在游戏加载时读取配置选项。
    /// Read configs when loading the game.
    /// </summary>
    private static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        GenericModConfigMenuIntegration.Register(Manifest, ModHelper.ModRegistry, ResetConfig, SaveConfig);
        ConvenientInventoryIntegration.Register();
        UIHelper.Initialize();
        SnapshotManager.Load();
        StashToChestsModule.RegisterHarmonyPatch();
    }

    /// <summary>
    /// 当游戏菜单打开、关闭或替换时触发。
    /// Raised after a game menu is opened, closed, or replaced.
    /// </summary>
    private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu == e.OldMenu)
            return;

        switch (e.OldMenu)
        {
            case GameMenu:
                ModHelper.Events.Input.ButtonsChanged -= OnButtonChanged;
                MenuManager.ClearMenu();
                break;
            case ItemGrabMenu:
                MenuManager.ClearMenu();
                break;
        }

        switch (e.NewMenu)
        {
            case GameMenu gameMenu:
                MenuManager.CreateMenu(gameMenu);
                ModHelper.Events.Input.ButtonsChanged += OnButtonChanged;
                break;
            case ItemGrabMenu itemGrabMenu:
                MenuManager.CreateMenu(itemGrabMenu);
                break;
        }
    }

    /// <summary>
    /// 在玩家按下/松开键盘、鼠标或手柄上的任意按钮时触发。
    /// Raised after the player pressed/released any buttons on the keyboard, mouse, or controller. 
    /// </summary>
    private static void OnButtonChanged(object sender, ButtonsChangedEventArgs e)
    {
        switch (Game1.activeClickableMenu)
        {
            case GameMenu { currentTab: 0 } when MenuManager.ScreenWidgetHost.Value == null:
                MenuManager.CreateMenu((GameMenu)Game1.activeClickableMenu);
                break;
            case GameMenu { currentTab: not 0 } when MenuManager.ScreenWidgetHost.Value != null:
                MenuManager.ClearMenu();
                break;
        }
    }

    #endregion
}