using System;
using System.IO;
using ConvenientChests.CategorizeChests;
using ConvenientChests.CraftFromChests;
using ConvenientChests.Framework;
using ConvenientChests.Framework.ConfigurationService;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.SaveService;
using ConvenientChests.Framework.UserInterfacService;
using ConvenientChests.StashToChests;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Module = ConvenientChests.Framework.Module;

namespace ConvenientChests;

/// <summary>
/// The mod entry class loaded by SMAPI.
/// </summary>
public class ModEntry : Mod
{
    /// <summary>
    /// 模组配置。
    /// Mod configuration.
    /// </summary>
    public ModConfig Config;

    internal static IModHelper StaticHelper { get; private set; }
    internal static IMonitor StaticMonitor { get; private set; }

    internal static void Log(string s, LogLevel l = LogLevel.Trace)
    {
        StaticMonitor.Log(s, l);
    }

    private readonly PerScreen<WidgetHost> _screenWidgetHost = new();

    /// <summary>
    /// <see cref="CategorizeChestsModule"/> mod.
    /// </summary>
    public static CategorizeChestsModule CategorizeChests { get; private set; }

    /// <summary>
    /// <see cref="CraftFromChestsModule"/> mod.
    /// </summary>
    private static CraftFromChestsModule CraftFromChests { get; set; }

    /// <summary>
    /// <see cref="StashToChestsModule"/> mod.
    /// </summary>
    private static StashToChestsModule StashToChests { get; set; }

    /// <summary>
    /// 存档数据的存储路径，位于 mod 文件夹下的 savedata 文件夹中。
    /// The path to the mod's save data file, relative to the mod folder.
    /// </summary>
    private static string SavePath => Path.Combine("savedata", $"{Constants.SaveFolderName}.json");

    /// <summary>
    /// 存档数据的绝对路径。
    /// The absolute path to the mod's save data file.
    /// </summary>
    private string AbsoluteSavePath => Path.Combine(Helper.DirectoryPath, SavePath);

    /// <summary>
    /// 模组存档数据的管理器。
    /// The manager responsible for handling the mod's save data.
    /// </summary>
    private SaveManager SaveManager { get; set; }

    /// <summary>
    /// 模组入口点，在模组首次加载后调用。
    /// The mod entry point, called after the mod is first loaded.
    /// </summary>
    public override void Entry(IModHelper helper)
    {
        I18n.Init(Helper.Translation);

        StaticMonitor = Monitor;
        StaticHelper = Helper;

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnGameLoaded;
        helper.Events.GameLoop.ReturnedToTitle += OnGameUnload;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.Display.MenuChanged += OnMenuChanged;

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
                if (module == CategorizeChests) StashToChests.RefreshJudgementFunction();
                break;
            case (false, true):
                module.Deactivate();
                if (module == CategorizeChests) StashToChests.RefreshJudgementFunction();
                break;
        }
    }

    /// <summary>
    /// 在载入游戏存档时加载模组。
    /// Load modules when loading a game save.
    /// </summary>
    private void OnGameLoaded(object sender, SaveLoadedEventArgs e)
    {
        CategorizeChests = new CategorizeChestsModule(this);
        if (Config.CategorizeChests)
            CategorizeChests.Activate();

        CraftFromChests = new CraftFromChestsModule(this);
        if (Config.CraftFromChests)
            CraftFromChests.Activate();

        StashToChests = new StashToChestsModule(this);
        if (Config.StashAnywhere || Config.StashToNearby)
            StashToChests.Activate();

        SaveManager = new SaveManager(ModManifest.Version, this, CategorizeChests, StashToChests);
        LoadSaveData();
    }

    /// <summary>
    /// 在返回游戏标题界面时卸载模组。
    /// Unload modules when back to the title page.
    /// </summary>
    private void OnGameUnload(object sender, ReturnedToTitleEventArgs e)
    {
        CategorizeChests.Deactivate();
        CategorizeChests = null;

        CraftFromChests.Deactivate();
        CraftFromChests = null;

        StashToChests.Deactivate();
        StashToChests = null;
    }

    /// <summary>
    /// 在游戏加载时读取配置选项
    /// Read configs when loading the game.
    /// </summary>
    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        GenericModConfigMenuIntegration.Register(ModManifest, Helper.ModRegistry, Monitor,
            () => Config,
            () => Config = new ModConfig(),
            () => Helper.WriteConfig(Config)
        );
    }

    /// <summary>
    /// 当游戏菜单打开、关闭或替换时触发。
    /// Raised after a game menu is opened, closed, or replaced.
    /// </summary>
    private void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        if (CategorizeChests is not null)
            ReloadConfig(Config.CategorizeChests, CategorizeChests);

        if (e.NewMenu == e.OldMenu)
            return;

        switch (e.OldMenu)
        {
            case GameMenu:
                Helper.Events.Input.ButtonsChanged -= OnButtonChanged;
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
                Helper.Events.Input.ButtonsChanged += OnButtonChanged;
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
    private void OnSaving(object sender, SavingEventArgs e)
    {
        try
        {
            SaveManager.Save(SavePath);
        }
        catch (Exception ex)
        {
            Monitor.Log($"Error saving chest data to {SavePath}", LogLevel.Error);
            Monitor.Log(ex.ToString());
        }
    }

    private void OnButtonChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (Game1.activeClickableMenu is not GameMenu { currentTab: 0 } && _screenWidgetHost.Value != null)
            ClearMenu();
        else if (Game1.activeClickableMenu is GameMenu { currentTab: 0 } && _screenWidgetHost.Value == null)
            CreateMenu(Game1.activeClickableMenu as GameMenu);
    }

    private void CreateMenu(ItemGrabMenu itemGrabMenu)
    {
        if (itemGrabMenu.context is not Chest chest) return;
        _screenWidgetHost.Value = new WidgetHost(CategorizeChests.Events, Helper.Input, Helper.Reflection);
        var overlay = new ChestOverlay(CategorizeChests, StashToChests, chest, itemGrabMenu, Config.CategorizeChests);
        _screenWidgetHost.Value.RootWidget.AddChild(overlay);
    }

    private void CreateMenu(GameMenu gameMenu)
    {
        _screenWidgetHost.Value = new WidgetHost(CategorizeChests.Events, Helper.Input, Helper.Reflection);
        var overlay = new MenuOverlay(StashToChests, gameMenu);
        _screenWidgetHost.Value.RootWidget.AddChild(overlay);
    }

    private void ClearMenu()
    {
        _screenWidgetHost.Value?.Dispose();
        _screenWidgetHost.Value = null;
    }

    private void LoadSaveData()
    {
        if (!File.Exists(AbsoluteSavePath)) return;
        UpdateSaveData();
        try
        {
            SaveManager.Load(SavePath);
        }
        catch (Exception ex)
        {
            Monitor.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
            Monitor.Log(ex.ToString());
        }
    }

    /// <summary>
    /// 更新 1.8.0 之前的存档文件结构。
    /// Updates legacy save files (pre-1.8.0) to the current format.
    /// </summary>
    private void UpdateSaveData()
    {
        var oldSaveData = Helper.Data.ReadJsonFile<SaveData>(SavePath);
        if (oldSaveData is null) return;

        var saveDataVersion = new SemanticVersion(oldSaveData.Version);
        if (!saveDataVersion.IsOlderThan("1.8.1")) return;
        
        var newSaveData = new SaveData();

        try
        {
            newSaveData.Version = ModManifest.Version.ToString();
            newSaveData.ChestEntries = oldSaveData.ChestEntries;
            newSaveData.InventoryEntries = Array.Empty<InventoryEntry>();
            SaveManager.Save(SavePath, newSaveData);
        }
        catch (Exception ex)
        {
            Monitor.Log($"Error update data from {SavePath}", LogLevel.Error);
            Monitor.Log(ex.ToString());
        }
    }
}