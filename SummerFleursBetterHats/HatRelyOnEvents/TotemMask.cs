using StardewModdingAPI.Events;
using SummerFleursBetterHats.Framework;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public partial class HatRelyOnEvents
{
    private static int _timer;

    /// <summary>
    /// Event for the totem mask, long press to warp home, once per day.
    /// </summary>
    private static void TotemMaskButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        // 仅当长按左键时
        // only when the player is holding the left mouse button
        if (e.Held.FirstOrDefault() != SButton.MouseLeft)
        {
            _timer = 0;
            return;
        }

        // 如果计时器未到 60 帧，则继续计时
        // if the timer has not reached 60 frames, continue counting
        if (_timer < 60)
        {
            _timer++;
            return;
        }

        // 如果玩家帽子不是图腾面具，则直接注销本事件
        // if the player's hat is not a totem mask, unregister this event
        if (!PlayerHatIs(TotemMaskID))
        {
            ModEntry.ModHelper.Events.Input.ButtonsChanged -= TotemMaskButtonsChanged;
            return;
        }

        // 如果玩家手上有东西、不可移动或已经使用过了图腾面具，退出
        // return if the player is holding something, cannot move, or has already used the mask today
        var player = Game1.player;
        if (player.ActiveItem is not null || !player.canMove || player.TryGetWorldStatus(TotemMaskMask))
            return;

        // 检查是否有正在活动的菜单或节日
        // Check if there is an active menu or festival
        if (Game1.activeClickableMenu is not null || Game1.currentLocation.currentEvent is not null)
            return;

        // 传送回家
        // Warp home
        var home = Utility.getHomeOfFarmer(player);
        if (home is null)
            return;

        player.playNearbySoundAll("wand");
        Game1.displayFarmer = false;
        player.temporarilyInvincible = true;
        player.temporaryInvincibilityTimer = -2000;
        player.Halt();
        player.faceDirection(2);
        player.CanMove = false;
        player.freezePause = 2000;
        Game1.flashAlpha = 1f;
        DelayedAction.fadeAfterDelay(Warp, 1000);
        SaveManager.TryEditWorldStatus(player.UniqueMultiplayerID, TotemMaskMask);

        return;

        void Warp()
        {
            var position = home.getFrontDoorSpot();
            Game1.warpFarmer("Farm", position.X, position.Y, false);
            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            player.temporarilyInvincible = false;
            player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
            player.CanMove = true;
        }
    }
}