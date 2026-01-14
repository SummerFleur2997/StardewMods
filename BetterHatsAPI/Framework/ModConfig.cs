using StardewModdingAPI.Utilities;

namespace BetterHatsAPI.Framework;

[Serializable]
public class ModConfig
{
    public bool DisableTickUpdateChecker { get; set; }
    public KeybindList OpenGuideBookKey { get; set; } = KeybindList.Parse("LeftControl + LeftAlt + H");
}