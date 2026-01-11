namespace BetterHatsAPI.Framework;

internal static class Utilities
{
    public static bool ApproximatelyEquals(this float value, float target, float epsilon = 1e-4f)
        => Math.Abs(value - target) < epsilon;

    public static string FormatAndTrim(this float value)
        => value.ToString("F").TrimEnd('0').TrimEnd('.');

    public static void Log(string m) => ModEntry.Log(m);
    public static void Warn(string m) => ModEntry.Log(m, LogLevel.Warn);
    public static void Error(string m) => ModEntry.Log(m, LogLevel.Error);
}