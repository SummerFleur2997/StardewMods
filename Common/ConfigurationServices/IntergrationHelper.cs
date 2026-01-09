#nullable enable
namespace Common.ConfigurationServices;

/// <summary>Simplifies validated access to mod APIs.</summary>
public static class IntegrationHelper
{
    /// <summary>Get a mod API if it's installed and valid.</summary>
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modId">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log"></param>
    /// <returns>Returns the mod's API interface if valid, else null.</returns>
    public static TInterface? GetValidatedApi<TInterface>(string label, string modId, string minVersion,
        IModRegistry modRegistry, Action<string, LogLevel> log)
        where TInterface : class
    {
        // check mod installed
        var mod = modRegistry.Get(modId)?.Manifest;
        if (mod == null)
            return null;

        // check the mod version
        if (mod.Version.IsOlderThan(minVersion))
        {
            log($"Detected {label} {mod.Version}, but need {minVersion} or later. Disabled integration with that mod.",
                LogLevel.Warn);
            return null;
        }

        // get API
        var api = modRegistry.GetApi<TInterface>(modId);
        if (api != null) return api;

        log($"Detected {label}, but couldn't fetch its API. Disabled integration with it.", LogLevel.Warn);
        return null;
    }

    /// <summary>Get Generic Mod Config Menu's API if it's installed and valid.</summary>
    /// <param name="registry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log">An action to output logs in console.</param>
    /// <returns>Returns the API interface if valid, else null.</returns>
    public static IGenericModConfigMenuApi? GetGenericModConfigMenu(IModRegistry registry, Action<string, LogLevel> log)
    {
        return GetValidatedApi<IGenericModConfigMenuApi>(
            "Generic Mod Config Menu",
            "spacechase0.GenericModConfigMenu",
            "1.6.0",
            registry, log
        );
    }
}