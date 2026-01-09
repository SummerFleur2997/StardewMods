using JetBrains.Annotations;

namespace _DebugMod;

[UsedImplicitly]
public class ModEntry : Mod
{
    public static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Info) => ModMonitor.Log(s, l);

    public override void Entry(IModHelper helper)
    {
        ModMonitor = Monitor;
        ModHelper = Helper;
    }
}