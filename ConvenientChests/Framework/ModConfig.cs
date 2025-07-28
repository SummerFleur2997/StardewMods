#nullable enable
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ConvenientChests.Framework;

public class ModConfig
{
    public bool CategorizeChests { get; set; } = true;
    public bool EnableSort { get; set; }

    public bool CraftFromChests { get; set; } = true;
    public int CraftRadius { get; set; } = 5;

    public bool StashToNearby { get; set; } = true;
    public bool StashAnywhere { get; set; }

    public int StashRadius { get; set; } = 5;

    public KeybindList StashToNearbyKey { get; set; } = KeybindList.Parse("Q");
    public KeybindList StashAnywhereKey { get; set; } = KeybindList.Parse("Z");
    public SButton? StashButton = SButton.RightStick;

    public bool StashToExistingStacks { get; set; } = true;
    public bool StashAnywhereToFridge { get; set; } = true;
    public bool NeverStashTools { get; set; } = true;
    public int MobileOffset { get; set; } = 40;
    
    public bool AutoStash { get; set; }
    public bool AutoStashInTheMine { get; set; }
    public bool AutoStashInSkullCavern { get; set; }
    public bool AutoStashInVolcanoDungeon { get; set; }
}