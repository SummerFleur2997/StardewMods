using System;
using System.IO;
using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.CategorizeChests.Framework.Persistence;
using ConvenientChests.CategorizeChests.Interface;
using ConvenientChests.CategorizeChests.Interface.Widgets;
using ConvenientChests.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientChests.CategorizeChests;

public class CategorizeChestsModule : Module 
{
    internal IItemDataManager ItemDataManager { get; } = new ItemDataManager();
    internal IChestDataManager ChestDataManager { get; } = new ChestDataManager();
    internal ChestFinder ChestFinder { get; } = new ();

    private string SavePath => Path.Combine("savedata", $"{Constants.SaveFolderName}.json");
    private string AbsoluteSavePath => Path.Combine(ModEntry.Helper.DirectoryPath, SavePath);

    private SaveManager SaveManager { get; set; }
    private readonly PerScreen<WidgetHost> ScreenWidgetHost = new ();

    internal bool ChestAcceptsItem(Chest chest, Item item) => ChestAcceptsItem(chest, item.ToBase().ToItemKey());
    private bool ChestAcceptsItem(Chest chest, ItemKey itemKey)
        => !ItemBlacklist.Includes(itemKey) && ChestDataManager.GetChestData(chest).Accepts(itemKey);

    public CategorizeChestsModule(ModEntry modEntry) : base(modEntry) 
    {
        modEntry.Helper.Events.Display.MenuChanged += OnMenuChanged;
    }

    public override void Activate() {
        IsActive = true;

        if (Context.IsMultiplayer && !Context.IsMainPlayer) {
            ModEntry.Log(
                         "Due to limitations in the network code, CHEST CATEGORIES CAN NOT BE SAVED as farmhand, sorry :(",
                         LogLevel.Warn);
            return;
        }

        // Save Events
        SaveManager = new SaveManager(ModEntry.ModManifest.Version, this);
        Events.GameLoop.Saving += OnSaving;
        OnGameLoaded();
    }

    public override void Deactivate() {
        IsActive = false;

        // Save Events
        Events.GameLoop.Saving -= OnSaving;
    }

    /// <summary>
    /// Raised before the game begins writes data to the save file (except the initial save creation).
    /// </summary>
    private void OnSaving(object sender, SavingEventArgs e) {
        try {
            SaveManager.Save(SavePath);
        }
        catch (Exception ex) {
            Monitor.Log($"Error saving chest data to {SavePath}", LogLevel.Error);
            Monitor.Log(ex.ToString());
        }
    }

    private void OnGameLoaded() {
        try {
            if (File.Exists(AbsoluteSavePath))
                SaveManager.Load(SavePath);
        }
        catch (Exception ex) {
            Monitor.Log($"Error loading chest data from {SavePath}", LogLevel.Error);
            Monitor.Log(ex.ToString());
        }
    }

    /// <summary>
    /// Raised after a game menu is opened, closed, or replaced.
    /// </summary>
    private void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
        ModEntry.ReloadConfig(ModConfig.CategorizeChests, this);
        
        if (e.NewMenu == e.OldMenu)
            return;

        if (e.OldMenu is ItemGrabMenu)
            ClearMenu();

        if (e.NewMenu is ItemGrabMenu itemGrabMenu)
            CreateMenu(itemGrabMenu);
    }

    private void CreateMenu(ItemGrabMenu itemGrabMenu) {
        if (itemGrabMenu.context is not Chest chest)
            return;

        ScreenWidgetHost.Value = new WidgetHost(Events, ModEntry.Helper.Input, ModEntry.Helper.Reflection);
        var overlay = new ChestOverlay(this, chest, itemGrabMenu, 
            ModConfig.CategorizeChests , ModConfig.StashToNearbyChests);
        ScreenWidgetHost.Value.RootWidget.AddChild(overlay);
    }

    private void ClearMenu() {
        ScreenWidgetHost.Value?.Dispose();
        ScreenWidgetHost.Value = null;
    }
}
