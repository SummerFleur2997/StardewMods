using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Enchantments;
using StardewValley.Objects;
using StardewValley.Tools;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public partial class HatRelyOnEvents
{
    /// <summary>
    /// Event for the pan hats, make it possible to unequip it with left click
    /// when a panning spot is nearby.
    /// </summary>
    private static void PanHatsButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        // 仅当按下左键时
        // only when the player clicked the left mouse button
        if (e.Button != SButton.MouseLeft)
            return;

        // 如果玩家的帽子不是淘盘帽，则直接注销本事件
        // if the player's hat is not a pan, unregister this event
        var hat = PlayerHat();
        switch (hat?.QualifiedItemId)
        {
            case IridiumPanHatID:
            case GoldPanHatID:
            case SteelPanHatID:
            case CopperPanID:
                break;
            default:
                ModEntry.ModHelper.Events.Input.ButtonPressed -= PanHatsButtonPressed;
                return;
        }

        // 检查当前位置有没有闪光点
        // check if there is a panning spot nearby
        var location = Game1.currentLocation;
        if (location.orePanPoint == null || location.orePanPoint.Value.Equals(Point.Zero))
            return;

        // 若未能将帽子变成淘盘，注销本事件
        // if the hat can't be transformed into a pan, unregister this event
        if (Utility.PerformSpecialItemGrabReplacement(hat) is not Pan pan)
        {
            ModEntry.ModHelper.Events.Input.ButtonPressed -= PanHatsButtonPressed;
            return;
        }

        // 如果玩家手上有东西、不可移动或打开了某个菜单，退出
        // return if the player is holding something, cannot move, or has opened a menu
        var player = Game1.player;
        if (player.ActiveItem is not null || !player.canMove || Game1.activeClickableMenu is not null)
            return;

        // 若摸不到闪光点，退出
        // return if the panning spot is not reachable
        var lastClick = e.Cursor.AbsolutePixels;
        if (!IsPanningPointAvailable(pan, lastClick.X, lastClick.Y))
            return;

        // 将帽子变成淘盘，然后注册后续事件
        // transform the hat into a pan, then register follow events
        player.hat.Value = null;
        player.ActiveItem = pan;

        ModEntry.ModHelper.Events.Player.InventoryChanged += WearPanHatBack;
    }

    private static void WearPanHatBack(object s, InventoryChangedEventArgs e)
    {
        var player = e.Player;
        if (e.Added.Any(i => i is Pan) || player.UsingTool)
            return;

        if (player.ActiveItem is not Pan pan || PlayerHat() is not null)
        {
            ModEntry.ModHelper.Events.Player.InventoryChanged -= WearPanHatBack;
            return;
        }

        // Experimental feature: chain panning
        var location = Game1.currentLocation;
        if (location.orePanPoint != null && !location.orePanPoint.Value.Equals(Point.Zero))
        {
            if (ModEntry.Config.ExperimentalFeatures &&
                IsPanningPointAvailable(pan, location.orePanPoint.Value.X * 64, location.orePanPoint.Value.Y * 64))
            {
                pan.beginUsing(location, location.orePanPoint.X * 64, location.orePanPoint.Y * 64, player);
                return;
            }
        }

        if (Utility.PerformSpecialItemPlaceReplacement(pan) is not Hat hat)
        {
            ModEntry.ModHelper.Events.Player.InventoryChanged -= WearPanHatBack;
            return;
        }

        player.ActiveItem = null;
        player.hat.Value = hat;
        ModEntry.ModHelper.Events.Player.InventoryChanged -= WearPanHatBack;
    }

    /// <summary>
    /// Whether the given panning point is available
    /// </summary>
    /// <returns></returns>
    private static bool IsPanningPointAvailable(Pan pan, float x, float y)
    {
        var location = Game1.currentLocation;
        var player = Game1.player;

        // 计算淘盘的矩形范围
        // calculate the rectangle of the pan
        var reach = pan.hasEnchantmentOfType<ReachingToolEnchantment>() ? 5 : 4;
        var orePanRect = new Rectangle(
            location.orePanPoint.X * 64 - 32 * reach,
            location.orePanPoint.Y * 64 - 32 * reach,
            64 * reach, 64 * reach);

        var playerPixel = player.StandingPixel;

        // 反编译自游戏内逻辑
        // decompiled from game logic
        return
            orePanRect.Contains(x, y) &&
            Utility.distance(playerPixel.X, orePanRect.Center.X, playerPixel.Y, orePanRect.Center.Y) <= reach * 64;
    }
}