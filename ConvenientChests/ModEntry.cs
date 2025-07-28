using Common;
using ConvenientChests.CategorizeChests;
using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.CraftFromChests;
using ConvenientChests.Framework;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.MultiplayerService;
using ConvenientChests.Framework.SaveService;
using ConvenientChests.Framework.UserInterfaceService;
using ConvenientChests.StashToChests;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientChests;

/// <summary>
/// The mod entry class loaded by SMAPI.
/// </summary>
[UsedImplicitly]
// If your IDE cannot recognize this attribute above, just delete it, and the using namespace in line 11.
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/
    #region Properties
    public static bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;
    public static ModConfig Config { get; private set; }
    public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    /// <summary>
    /// <see cref="CategorizeChestsModule"/> mod.
    /// </summary>
    internal static CategorizeChestsModule CategorizeModule { get; private set; }

    /// <summary>
    /// <see cref="CraftFromChestsModule"/> mod.
    /// </summary>
    internal static CraftFromChestsModule CraftModule { get; private set; }

    /// <summary>
    /// <see cref="StashToChestsModule"/> mod.
    /// </summary>
    internal static StashToChestsModule StashModule { get; private set; }

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
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.Display.MenuChanged += OnMenuChanged;

        Config = helper.ReadConfig<ModConfig>();
    }

    /****
     ** 私有方法
     ** Private Methods
     ****/
    #region Private Methods
    /// <summary>
    /// 读取模组配置更新并重新载入配置。
    /// Read the update of modconfig and reload them.
    /// </summary>
    private static void ReloadConfig()
    {
        ModHelper.WriteConfig(Config);
        Config = ModHelper.ReadConfig<ModConfig>();
        if (!Game1.hasLoadedGame) return;

        UpdateModule(Config.CategorizeChests, CategorizeModule);
        UpdateModule(Config.CraftFromChests, CraftModule);
        UpdateModule(Config.StashToNearby || Config.StashAnywhere, StashModule);
        StashModule.CreateJudgementFunction();

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
        CategoryDataManager.Initialize();
        CategorizeModule = new CategorizeChestsModule();
        if (Config.CategorizeChests)
            CategorizeModule.Activate();

        CraftModule = new CraftFromChestsModule();
        if (Config.CraftFromChests)
            CraftModule.Activate();

        StashModule = new StashToChestsModule();
        if (Config.StashAnywhere || Config.StashToNearby)
            StashModule.Activate();

        SaveManager.Load();

        MultiplayerServer = new MultiplayerServer();
        if (Context.IsMultiplayer)
            MultiplayerServer.Activate();
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private static void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        CategorizeModule.Deactivate();
        CategorizeModule = null;

        CraftModule.Deactivate();
        CraftModule = null;

        StashModule.Deactivate();
        StashModule = null;

        if (MultiplayerServer.IsActive)
            MultiplayerServer.Deactivate();

        ChestManager.ClearChestData();
        InventoryManager.ClearInventoryData();
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

        if (!ModHelper.ModRegistry.IsLoaded("DLX.QuickSave")) return;
        var api = ModHelper.ModRegistry.GetApi<IQuickSaveAPI>("DLX.QuickSave");
        if (api != null) api.SavingEvent += Api_SavingEvent;
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
    /// 在游戏向存档文件写入数据前触发（除了新建存档时）。
    /// Raised before the game begins writes data to the save file (except the initial save creation).
    /// </summary>
    private static void OnSaving(object sender, SavingEventArgs e)
    {
        SaveManager.Save();
    }

    /// <summary>
    /// 兼容 Quick Save
    /// Compatible to Quick Save
    /// </summary>
    private static void Api_SavingEvent(object sender, ISavingEventArgs e)
    {
        SaveManager.Save();
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