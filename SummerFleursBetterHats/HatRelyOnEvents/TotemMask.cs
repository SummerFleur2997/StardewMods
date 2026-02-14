using StardewModdingAPI.Events;
using StardewValley.Tools;
using SummerFleursBetterHats.Framework;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public partial class HatRelyOnEvents
{
    /// <summary>
    /// Event for the totem mask, long press to warp home, once per day.
    /// </summary>
    private static void TotemMaskButtonsChanged(object s, ButtonsChangedEventArgs e)
    {
        // 仅当按下快捷键时
        // only when press the keybind
        if (!ModEntry.Config.HatActionKeybind.JustPressed())
            return;

        // 如果玩家帽子不是图腾面具，则直接注销本事件
        // if the player's hat is not a totem mask, unregister this event
        if (!PlayerHatIs(TotemMaskID))
        {
            ModEvents.Input.ButtonsChanged -= TotemMaskButtonsChanged;
            return;
        }

        // 如果玩家手上有东西、不可移动，退出
        // return if the player is holding something, cannot move
        var player = Game1.player;
        if (player.ActiveItem is Wand || !player.canMove)
            return;

        // 检查是否有正在活动的菜单或节日
        // Check if there is an active menu or festival
        if (Game1.activeClickableMenu is not null || Game1.currentLocation.currentEvent is not null)
            return;

        // 如果玩家已经使用过了图腾面具，退出并显示消息
        // return and show a message if the player has already used the mask today
        if (player.TryGetWorldStatus(TotemMaskMask))
        {
            Game1.showRedMessage(I18n.String_Hat_Used(PlayerHat()!.DisplayName));
            return;
        }

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