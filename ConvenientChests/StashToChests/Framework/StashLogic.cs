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
        var stashableItems = Game1.player.Items
            .Where(item => item is not null && !rf(item))
            .ToList();

        if (!DumpItemsToChest(chest, stashableItems, af, out var moved))
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
    /// <param name="chest">The chest to put the items in.</param>
    /// <param name="stashableItems">Items not locked.</param>
    /// <param name="af">Accepting rules.</param>
    /// <param name="moved">The items that successfully moved to chests.</param>
    /// <returns>True if at least some of the items were moved, false otherwise</returns>
    private static bool DumpItemsToChest(Chest chest, List<Item> stashableItems, AcceptingFunc af,
        out List<(string Name, int Stack)> moved)
    {
        moved = new List<(string Name, int Stack)>();

        for (var i = stashableItems.Count - 1; i >= 0; i--)
        {
            var item = stashableItems[i];
            if (!Game1.player.Items.Contains(item) || !af(chest, item))
                continue;

            var result = item.TryMoveTo(chest);
            if (result.Stack <= 0)
                continue;

            moved.Add(result);

            if (!Game1.player.Items.Contains(item))
                stashableItems.RemoveAt(i);
        }

        return moved.Count > 0;
    }

    /// <summary>
    /// Attempt to move as much as possible of the given item stack into the chest.
    /// </summary>
    /// <param name="chest">The chest to put the items in.</param>
    /// <param name="item">The items to put in the chest.</param>
    /// <returns>True if at least some of the stack was moved into the chest.</returns>
    private static (string Name, int Stack) TryMoveTo(this Item item, Chest chest)
    {
        var original = item.Stack;
        var remainder = chest.addItem(item);

        // nothing remains -> remove item
        if (remainder == null)
        {
            var index = Game1.player.Items.IndexOf(item);
            Game1.player.Items[index] = null;
            // item.Stack = original;
            return (item.Name, item.Stack);
        }

        // nothing changed
        if (remainder.Stack == item.Stack)
            return ("", -1);

        // update stack count
        item.Stack = remainder.Stack;

        // return copy for moved item
        return (item.Name, original - remainder.Stack);
    }
}