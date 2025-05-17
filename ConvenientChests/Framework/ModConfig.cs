#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ConvenientChests.Framework;

public class ModConfig : IEquatable<ModConfig>
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

    public bool Equals(ModConfig? other)
    {
        if (other is null) return false;
        
        if (ReferenceEquals(this, other)) return true;
        
        return CategorizeChests == other.CategorizeChests
               && EnableSort == other.EnableSort
               && CraftFromChests == other.CraftFromChests
               && CraftRadius == other.CraftRadius
               && StashToNearby == other.StashToNearby
               && StashAnywhere == other.StashAnywhere
               && StashRadius == other.StashRadius
               && StashToExistingStacks == other.StashToExistingStacks
               && StashAnywhereToFridge == other.StashAnywhereToFridge
               && NeverStashTools == other.NeverStashTools
               && StashToNearbyKey.ToString().Equals(other.StashToNearbyKey.ToString())
               && StashAnywhereKey.ToString().Equals(other.StashAnywhereKey.ToString());
    }
    
    public override bool Equals(object? obj) 
        => Equals(obj as ModConfig);
    
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            
            hash = hash * 23 + (CategorizeChests ? 1 : 0);
            hash = hash * 23 + (EnableSort ? 1 : 0);
            hash = hash * 23 + (CraftFromChests ? 1 : 0);
            hash = hash * 23 + (StashToNearby ? 1 : 0);
            hash = hash * 23 + (StashAnywhere ? 1 : 0);
            hash = hash * 23 + (StashToExistingStacks ? 1 : 0);
            hash = hash * 23 + (StashAnywhereToFridge ? 1 : 0);
            hash = hash * 23 + (NeverStashTools ? 1 : 0);
            hash = hash * 23 + CraftRadius;
            hash = hash * 23 + StashRadius;
            hash = hash * 23 + StashToNearbyKey.ToString().GetHashCode();
            hash = hash * 23 + StashAnywhereKey.ToString().GetHashCode();

            return hash;
        }
    }
}