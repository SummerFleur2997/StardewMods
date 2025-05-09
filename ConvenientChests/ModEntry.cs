using ConvenientChests.CategorizeChests;
using ConvenientChests.CraftFromChests;
using ConvenientChests.Framework;
using ConvenientChests.Framework.ConfigurationService;
using ConvenientChests.Framework.SaveService;
using ConvenientChests.Framework.UserInterfacService;
using ConvenientChests.StashToChests;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests;

/// <summary>
/// The mod entry class loaded by SMAPI.
/// </summary>
public class ModEntry : Mod
{
    internal static ModConfig Config { get; private set; }
    internal static IManifest Manifest { get; private set; }
    internal static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    private static IMultiplayerHelper MultiplayerHelper { get; set; }
    
    private Saver Saver 
        => new (ModManifest.Version, CategorizeModule.ChestManager, StashModule.InventoryManager);

    internal static void Log(string s, LogLevel l = LogLevel.Trace)
    {
        ModMonitor.Log(s, l);
    }

    private readonly PerScreen<WidgetHost> _screenWidgetHost = new();

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
    public static void ReloadConfig(bool configStatus, IModule module)
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
    private void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        if (CategorizeModule is not null)
            ReloadConfig(Config.CategorizeChests, CategorizeModule);

        if (e.NewMenu == e.OldMenu)
            return;

        switch (e.OldMenu)
        {
            case GameMenu:
                ModHelper.Events.Input.ButtonsChanged -= OnButtonChanged;
                ClearMenu();
                break;
            case ItemGrabMenu:
                ClearMenu();
                break;
        }

        switch (e.NewMenu)
        {
            case GameMenu gameMenu:
                CreateMenu(gameMenu);
                ModHelper.Events.Input.ButtonsChanged += OnButtonChanged;
                break;
            case ItemGrabMenu itemGrabMenu:
                CreateMenu(itemGrabMenu);
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

    private void OnButtonChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (Game1.activeClickableMenu is not GameMenu { currentTab: 0 } && _screenWidgetHost.Value != null)
            ClearMenu();
        else if (Game1.activeClickableMenu is GameMenu { currentTab: 0 } && _screenWidgetHost.Value == null)
            CreateMenu(Game1.activeClickableMenu as GameMenu);
    }
    
    private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
    {
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;
        
        var saveData = Saver.GetSerializableData();
        MultiplayerHelper.SendMessage(saveData, "MultiplayerSync", 
            new []{ ModManifest.UniqueID }, new []{ e.Peer.PlayerID });
    }

    private void CreateMenu(ItemGrabMenu itemGrabMenu)
    {
        if (itemGrabMenu.context is not Chest chest) return;
        _screenWidgetHost.Value = new WidgetHost(CategorizeModule.Events, ModHelper.Input, ModHelper.Reflection);
        var overlay = new ChestOverlay(itemGrabMenu, chest, Config.CategorizeChests);
        _screenWidgetHost.Value.RootWidget.AddChild(overlay);
    }

    private void CreateMenu(GameMenu gameMenu)
    {
        _screenWidgetHost.Value = new WidgetHost(CategorizeModule.Events, ModHelper.Input, ModHelper.Reflection);
        var overlay = new MenuOverlay(gameMenu);
        _screenWidgetHost.Value.RootWidget.AddChild(overlay);
    }

    private void ClearMenu()
    {
        _screenWidgetHost.Value?.Dispose();
        _screenWidgetHost.Value = null;
    }
}