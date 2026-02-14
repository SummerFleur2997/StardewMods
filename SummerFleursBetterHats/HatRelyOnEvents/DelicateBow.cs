using StardewModdingAPI.Events;
using SummerFleursBetterHats.Framework;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public partial class HatRelyOnEvents
{
    private const int MaxHealTime = 30;
    private static int _healedTime;
    private static int _lastTriggeredTime;

    /// <summary>
    /// Slowly heal the player's health and stamina in the next 30 seconds.
    /// </summary>
    private static void DelicateBowButtonsChanged(object s, ButtonsChangedEventArgs e)
    {
        // 仅当按下快捷键时
        // only when press the keybind
        if (!ModEntry.Config.HatActionKeybind.JustPressed())
            return;

        // 如果玩家帽子不是精美蝴蝶结，则直接注销本事件
        // if the player's hat is not a delicate bow, unregister this event
        if (!PlayerHatIs(DelicateBowID))
        {
            ModEvents.Input.ButtonsChanged -= DelicateBowButtonsChanged;
            return;
        }

        // 如果玩家已经使用过了效果，退出并显示消息
        // return and show a message if the player has already used the delicate bow today
        var player = Game1.player;
        if (player.TryGetWorldStatus(DelicateBowMask))
        {
            Game1.showRedMessage(I18n.String_Hat_Used(PlayerHat()!.DisplayName));
            return;
        }

        _healedTime = 0;
        _lastTriggeredTime = 0;
        Game1.playSound("healSound");
        ModEvents.GameLoop.OneSecondUpdateTicked += DelicateBowHeal;
        SaveManager.TryEditWorldStatus(player.UniqueMultiplayerID, DelicateBowMask);
    }

    private static void DelicateBowHeal(object s, OneSecondUpdateTickedEventArgs e)
    {
        if (!Game1.shouldTimePass())
            return;

        if (_healedTime >= MaxHealTime || _lastTriggeredTime > Game1.timeOfDay)
        {
            ModEvents.GameLoop.OneSecondUpdateTicked -= DelicateBowHeal;
            return;
        }

        var player = Game1.player;
        var health = player.health;

        player.health = Math.Min(player.maxHealth, health + 3);
        player.Stamina += 6.7f;
        _healedTime++;
        _lastTriggeredTime = Game1.timeOfDay;
    }
}