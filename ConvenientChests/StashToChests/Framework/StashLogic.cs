using ConvenientChests.Framework.Extensions;
using StardewValley.Inventories;
using StardewValley.Objects;

namespace ConvenientChests.StashToChests.Framework;

internal static class StashLogic
{
    public static string StashCueName => Game1.soundBank.GetCue("pickUpItem").Name;

    public delegate bool AcceptingFunc(Chest c, Item i);

    public delegate bool RejectingFunc(Item i);

    /// <summary>
    /// 存储至单个箱子。
    /// Stash to a selected chest.
    /// </summary>
    /// <param name="chest">选中的箱子 Selected chest</param>
    /// <param name="af">物品接受条件 Accepting rules</param>
    /// <param name="rf">物品拒绝条件 Rejecting rules</param>
    /// <returns>是否存储成功 Stashed successfully?</returns>
    public static bool StashToChest(Chest chest, AcceptingFunc af, RejectingFunc rf)
    {
        // try to move items to the chest
        var moved = Game1.player.Items
            .DumpItemsToChest(chest, af, rf)
            .ToList();

        if (!moved.Any())
            return false;

        var which = string.Join(", ", moved.Select(i => $"{i.Name} * {i.Stack}"));
        ModEntry.Log($"Moved [{which}] to chest in {chest.Location.Name} at {chest.TileLocation}.");
        return true;
    }

    /// <summary>
    /// 存储至给定的箱子。
    /// Stash to given chests.
    /// </summary>
    /// <param name="chests">选中的箱子 Selected chests</param>
    /// <param name="af">物品接受条件 Accepting rules</param>
    /// <param name="rf">物品拒绝条件 Rejecting rules</param>
    /// <returns>是否存储成功 Stashed successfully?</returns>
    public static bool StashToChests(IEnumerable<Chest> chests, AcceptingFunc af, RejectingFunc rf)
    {
        var movedAtLeastOne = false;

        foreach (var chest in chests)
            movedAtLeastOne |= StashToChest(chest, af, rf);

        return movedAtLeastOne;
    }

    /// <summary>
    /// 存储至当前的箱子。
    /// Stash to the current chest.
    /// </summary>
    /// <param name="chest">当前的箱子 Selected chest</param>
    /// <param name="af">物品接受条件 Accepting rules</param>
    /// <param name="rf">物品拒绝条件 Rejecting rules</param>
    public static void StashToCurrentChest(Chest chest, AcceptingFunc af, RejectingFunc rf)
    {
        if (!StashToChest(chest, af, rf)) return;
        Game1.playSound(StashCueName);
        ModEntry.Log("Stash to current chest");
    }

    /// <summary>
    /// 搜寻并存储物品至附近的箱子。
    /// Search and stash items to nearby chests.
    /// </summary>
    /// <param name="chests">选中的箱子 Selected chests</param>
    /// <param name="af">物品接受条件 Accepting rules</param>
    /// <param name="rf">物品拒绝条件 Rejecting rules</param>
    public static void StashToNearbyChests(IEnumerable<Chest> chests, AcceptingFunc af, RejectingFunc rf)
    {
        if (!StashToChests(chests, af, rf)) return;
        Game1.playSound(StashCueName);
        ModEntry.Log("Stash to nearby chests");
    }

    /// <summary>
    /// Attempt to move as much as possible of the player's inventory into the given chest
    /// </summary>
    /// <param name="sourceInventory">The player's inventory</param>
    /// <param name="chest">The chest to put the items in.</param>
    /// <param name="af">Accepting rules.</param>
    /// <param name="rf">Rejecting rules.</param>
    /// <returns>List of Items that were successfully moved into the chest</returns>
    private static IEnumerable<Item> DumpItemsToChest(this Inventory sourceInventory, Chest chest,
        AcceptingFunc af, RejectingFunc rf)
    {
        return sourceInventory
            .Where(i => i != null && af(chest, i) && !rf(i))
            .Select(item => sourceInventory.TryMoveItemToChest(chest, item))
            .OfType<Item>();
    }

    /// <summary>
    /// Attempt to move as much as possible of the given item stack into the chest.
    /// </summary>
    /// <param name="sourceInventory">The player's inventory</param>
    /// <param name="chest">The chest to put the items in.</param>
    /// <param name="item">The items to put in the chest.</param>
    /// <returns>True if at least some of the stack was moved into the chest.</returns>
    private static Item? TryMoveItemToChest(this IInventory sourceInventory, Chest chest, Item item)
    {
        var original = item.Stack;
        var remainder = chest.addItem(item);

        // nothing remains -> remove item
        if (remainder == null)
        {
            var index = sourceInventory.IndexOf(item);
            sourceInventory[index] = null;
            // item.Stack = original;
            return item;
        }

        // nothing changed
        if (remainder.Stack == item.Stack)
            return null;

        // update stack count
        item.Stack = remainder.Stack;

        // return copy for moved item
        var copy = item.Copy();
        copy.Stack = original - remainder.Stack;
        return copy;
    }
}