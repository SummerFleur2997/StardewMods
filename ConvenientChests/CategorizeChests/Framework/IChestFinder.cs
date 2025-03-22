using ConvenientChests.Framework.CategorizeChests.Framework.Persistence;
using StardewValley.Objects;

namespace ConvenientChests.Framework.CategorizeChests.Framework
{
    /// <summary>
    /// A helper for finding the chest object corresponding to a given chest address.
    /// </summary>
    interface IChestFinder
    {
        Chest GetChestByAddress(ChestAddress address);
    }
}