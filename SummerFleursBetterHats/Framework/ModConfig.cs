using StardewModdingAPI.Utilities;

namespace SummerFleursBetterHats.Framework;

[Serializable]
public class ModConfig
{
    public bool ExperimentalFeatures { get; set; }
    public KeybindList HatActionKeybind { get; set; } = KeybindList.Parse("LeftControl + Space");
}