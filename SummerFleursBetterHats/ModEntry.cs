using JetBrains.Annotations;
using StardewModdingAPI.Events;
using SummerFleursBetterHats.Framework;
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

    // public static IManifest Manifest { get; private set; }
    public static IModHelper ModHelper { get; private set; }
    private static Harmony Harmony { get; set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Trace) => ModMonitor.Log(s, l);

    #endregion

    public override void Entry(IModHelper helper)
    {
        // Manifest = ModManifest;
        ModMonitor = Monitor;
        ModHelper = Helper;
        Harmony = new Harmony(ModManifest.UniqueID);

        RegisterAllPatches(Harmony);
        helper.Events.GameLoop.GameLaunched += Test;
    }

    private static void Test(object s, GameLaunchedEventArgs e) => GameExtensions.RegisterAll();
}