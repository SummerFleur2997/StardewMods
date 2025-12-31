using StardewModdingAPI.Utilities;

namespace BinThereSavedThat;

public class ModConfig
{
    public int MaxStorageSize { get; set; } = 3;

    public KeybindList OpenItemRecallMenu { get; set; } = KeybindList.Parse("G");
}