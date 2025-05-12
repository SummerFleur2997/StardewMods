using ConvenientChests.CategorizeChests;
using ConvenientChests.CraftFromChests;
using ConvenientChests.Framework;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.ConfigurationService;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.SaveService;
using ConvenientChests.Framework.UserInterfacService;
using ConvenientChests.StashToChests;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientChests;

/// <summary>
/// The mod entry class loaded by SMAPI.
/// </summary>
public class ModEntry : Mod
{
    public static ModConfig Config { get; private set; }
    public static IManifest Manifest { get; private set; }
    internal static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    private static IMultiplayerHelper MultiplayerHelper { get; set; }

    internal static void Log(string s, LogLevel l = LogLevel.Trace)
    {
        ModMonitor.Log(s, l);
    }

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
    /// 模组入口点，在模组首次加载后调用。
    /// The mod entry point, called after the mod is first loaded.
    /// </summary>
    public override void Entry(IModHelper helper)
    {
        I18n.Init(Helper.Translation);

        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;
        MultiplayerHelper = helper.Multiplayer;

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnGameLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnGameUnload;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.Display.MenuChanged += OnMenuChanged;

        helper.Events.Multiplayer.PeerConnected += OnPeerConnected;

        Config = helper.ReadConfig<ModConfig>();
    }

    /// <summary>
    /// 读取模组配置更新并重新载入配置。
    /// Read the update of modconfig and reload them.
    /// </summary>
    /// <param name="configStatus">配置选项状态</param>
    /// <param name="module">需要重新载入配置的模组</param>
    internal static void ReloadConfig(bool configStatus, IModule module)
    {
        Config = ModHelper.ReadConfig<ModConfig>();

        switch (configStatus, module.IsActive)
        {
            case (true, false):
                module.Activate();
                if (module == CategorizeModule) StashModule.RefreshJudgementFunction();
                break;
            case (false, true):
                module.Deactivate();
                if (module == CategorizeModule) StashModule.RefreshJudgementFunction();
                break;
        }
    }

    /// <summary>
    /// 在载入游戏存档时加载模组。
    /// Load modules when loading a game save.
    /// </summary>
    private static void OnGameLoaded(object sender, SaveLoadedEventArgs e)
    {
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
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private static void OnGameUnload(object sender, ReturnedToTitleEventArgs e)
    {
        CategorizeModule.Deactivate();
        CategorizeModule = null;

        CraftModule.Deactivate();
        CraftModule = null;

        StashModule.Deactivate();
        StashModule = null;

        ChestManager.ClearChestData();
        InventoryManager.ClearInventoryData();
    }

    /// <summary>
    /// 在游戏加载时读取配置选项
    /// Read configs when loading the game.
    /// </summary>
    private static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        GenericModConfigMenuIntegration.Register(Manifest, ModHelper.ModRegistry,
            () => Config,
            () => Config = new ModConfig(),
            () => ModHelper.WriteConfig(Config)
        );
    }

    /// <summary>
    /// 当游戏菜单打开、关闭或替换时触发。
    /// Raised after a game menu is opened, closed, or replaced.
    /// </summary>
    private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        if (CategorizeModule is not null)
            ReloadConfig(Config.CategorizeChests, CategorizeModule);

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
    /// 在游戏开始对存档进行保存之前触发。
    /// Raised before the game begins writes data to the save file (except the initial save creation).
    /// </summary>
    private static void OnSaving(object sender, SavingEventArgs e)
    {
        SaveManager.Save();
    }

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

    private static void OnPeerConnected(object sender, PeerConnectedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;

        var saveData = Saver.GetSerializableData();
        MultiplayerHelper.SendMessage(saveData, "MultiplayerSync",
            new[] { Manifest.UniqueID }, new[] { e.Peer.PlayerID });
    }
}