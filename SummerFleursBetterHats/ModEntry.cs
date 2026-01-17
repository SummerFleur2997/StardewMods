using BetterHatsAPI.API;
using Common.ConfigurationServices;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using SummerFleursBetterHats.Framework;
using static SummerFleursBetterHats.HatRelatedToShops.HatRelatedToShops;
using static SummerFleursBetterHats.HatRelyOnEvents.HatRelyOnEvents;
using static SummerFleursBetterHats.HatWithEffects.HatWithEffects;
using static SummerFleursBetterHats.HatWithPatches.HatWithPatches;

namespace SummerFleursBetterHats;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    public static ModConfig Config { get; private set; }
    public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    #endregion

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;

        ModHelper.Events.GameLoop.GameLaunched += RegisterAll;
        Config = helper.ReadConfig<ModConfig>();
        I18n.Init(Helper.Translation);
    }

    private static void RegisterAll(object s, GameLaunchedEventArgs e)
    {
        var hatsAPI = IntegrationHelper.GetValidatedApi<ISummerFleurBetterHatsAPI>(
            "Better Hats API",
            "SummerFleur.BetterHatsAPI",
            "1.0.0",
            ModHelper.ModRegistry, Log);

        if (hatsAPI == null)
        {
            Log("SummerFleur.BetterHatsAPI is not installed. The mod will not work.", LogLevel.Error);
            return;
        }

        var harmony = new Harmony(Manifest.UniqueID);

        RegisterAllPatches(harmony);
        RegisterCustomMethods(hatsAPI);
        RegisterHatRelatedEvents(hatsAPI);
        RegisterShopRelatedEvents();

        SaveManager.RegisterEvents();
        GameExtensions.RegisterMethods();
        MultiplayerServer.RegisterEvents();
    }
}