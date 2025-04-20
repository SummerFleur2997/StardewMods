using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ChestService;
using StardewValley;
using StardewValley.Objects;

namespace ConvenientChests.StashToChests.Framework;

public static class StashLogic
{
    internal static string StashCueName => Game1.soundBank.GetCue("pickUpItem").Name;

    public delegate bool AcceptingFunc(Chest c, Item i);

    public delegate bool RejectingFunc(Item i);

    /// <summary>
    /// 堆叠至单个箱子。
    /// Stash to selected chest.
    /// </summary>
    /// <param name="chest">选中的箱子 Selected chest</param>
    /// <param name="af">物品接受条件 Accepting rules</param>
    /// <param name="rf">物品拒绝条件 Rejecting rules</param>
    /// <returns>是否堆叠成功 Stashed successfully?</returns>
    internal static bool StashToChest(Chest chest, AcceptingFunc af, RejectingFunc rf)
    {
        // find items to be moved
        var toBeMoved = Game1.player.Items
            .Where(i => i != null && af(chest, i) && !rf(i))
            .ToList();

        foreach (var item in Game1.player.Items)
        {
            if (item == null) continue;
            Console.WriteLine(item.DisplayName);
            Console.WriteLine(af(chest, item).ToString());
            Console.WriteLine(rf(item).ToString());
            var a = item is Tool;
            Console.WriteLine(a.ToString());
        }

        // try to move items to chest
        return toBeMoved.Any() && Game1.player.Items.DumpItemsToChest(chest, toBeMoved).Any();
    }

    /// <summary>
    /// 堆叠至当前的箱子。
    /// Stash to current chest.
    /// </summary>
    /// <param name="chest">当前的箱子 Selected chest</param>
    /// <param name="af">物品接受条件 Accepting rules</param>
    /// <param name="rf">物品拒绝条件 Rejecting rules</param>
    internal static void StashToCurrentChest(Chest chest, AcceptingFunc af, RejectingFunc rf)
    {
        if (!StashToChest(chest, af, rf)) return;
        Game1.playSound(StashCueName);
        ModEntry.Log("Stash to current chest");
    }

    /// <summary>
    /// 搜寻并堆叠物品至附近的箱子。
    /// Search and stash items to nearby chests.
    /// </summary>
    /// <param name="radius">搜寻半径 Searching radius</param>
    /// <param name="af">物品接受条件 Accepting rules</param>
    /// <param name="rf">物品拒绝条件 Rejecting rules</param>
    internal static void StashToNearbyChests(int radius, AcceptingFunc af, RejectingFunc rf)
    {
        if (!StashToGivenChests(Game1.player.GetNearbyChests(radius), af, rf)) return;
        Game1.playSound(StashCueName);
        ModEntry.Log("Stash to nearby chests");
    }

    /// <summary>
    /// 堆叠至给定的箱子。
    /// Stash to given chests.
    /// </summary>
    /// <param name="chests">选中的箱子 Selected chests</param>
    /// <param name="af">物品接受条件 Accepting rules</param>
    /// <param name="rf">物品拒绝条件 Rejecting rules</param>
    /// <returns>是否堆叠成功 Stashed successfully?</returns>
    internal static bool StashToGivenChests(IEnumerable<Chest> chests, AcceptingFunc af, RejectingFunc rf)
    {
        var movedAtLeastOne = false;

        foreach (var chest in chests)
            if (StashToChest(chest, af, rf))
                movedAtLeastOne = true;

        return movedAtLeastOne;
    }
}