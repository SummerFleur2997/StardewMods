using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SummerFleursBetterHats.HatWithBuffs;

namespace SummerFleursBetterHats;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static Harmony Harmony { get; set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    private static readonly HatBuffManager BuffManager = new();

    #endregion

    public override void Entry(IModHelper helper)
    {
        Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;
        Harmony = new Harmony(ModManifest.UniqueID);

        Helper.Events.GameLoop.SaveLoaded += RegisterHatChangeEvents;
        HatWithPatches.HatWithPatches.RegisterHarmonyPatchForChefHat(Harmony);
        HatWithPatches.HatWithPatches.RegisterHarmonyPatchForGarbageHat(Harmony);
    }

    private static void RegisterHatChangeEvents(object s, SaveLoadedEventArgs e)
    {
        foreach (var farmer in Game1.getAllFarmers())
            farmer.hat.fieldChangeEvent += BuffManager.OnHatChange;
    }

    public override object GetApi(IModInfo mod) => BuffManager;
}