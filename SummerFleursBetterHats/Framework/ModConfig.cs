using StardewModdingAPI.Utilities;

namespace SummerFleursBetterHats.Framework;

[Serializable]
public class ModConfig
{
    public bool ChainPanning { get; set; }
    public KeybindList ActiveEffectKeybind { get; set; } = KeybindList.Parse("LeftControl + Space");
}