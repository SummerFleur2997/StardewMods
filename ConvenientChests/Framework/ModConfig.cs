using StardewModdingAPI;

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

    public SButton StashToNearbyKey { get; set; } = SButton.Q;
    public SButton StashAnywhereKey { get; set; } = SButton.Z;
    public SButton? StashButton = SButton.RightStick;

    public bool StashToExistingStacks { get; set; } = true;
    public bool StashAnywhereToFridge { get; set; } = true;
    public bool NeverStashTools { get; set; } = true;
}