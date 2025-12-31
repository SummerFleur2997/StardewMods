using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace BinThereSavedThat;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    public static ModConfig Config { get; private set; }

    // public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    #endregion

    public override void Entry(IModHelper helper)
    {
        // Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;
        Config = helper.ReadConfig<ModConfig>();

        var harmony = new Harmony(ModManifest.UniqueID);
        Patcher.RegisterHarmonyPatches(harmony);

        Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        Helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
    }


    private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        ModHelper.Events.GameLoop.DayEnding += OnDayEnding;
        ModHelper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    private static void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        ItemSaver.ClearSavedStorage();
        ModHelper.Events.GameLoop.DayEnding -= OnDayEnding;
        ModHelper.Events.Input.ButtonPressed -= OnButtonPressed;
    }

    private static void OnDayEnding(object sender, DayEndingEventArgs e) => ItemSaver.ClearSavedStorage();

    private static void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (!Config.OpenItemRecallMenu.JustPressed()) return;
        if (ItemSaver.TryToCreateShopMenu(out var shopMenu))
            Game1.activeClickableMenu = shopMenu;
    }
}