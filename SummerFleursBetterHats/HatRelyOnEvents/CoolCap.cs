using StardewModdingAPI.Events;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace SummerFleursBetterHats.HatRelyOnEvents;

public partial class HatRelyOnEvents
{
    /// <summary>
    /// Watering the crops in one click during the first 3 days of the month.
    /// </summary>
    private static void CoolCapButtonsChanged(object s, ButtonsChangedEventArgs e)
    {
        // 仅当按下快捷键时
        // only when press the keybind
        if (!ModEntry.Config.ActiveEffectKeybind.JustPressed())
            return;

        // 如果玩家帽子不是凉帽，则直接注销本事件
        // if the player's hat is not a cool cap, unregister this event
        if (!PlayerHatIs(CoolCapID))
        {
            ModEvents.Input.ButtonsChanged -= CoolCapButtonsChanged;
            return;
        }

        // 如果当前日期不符合条件，退出并显示消息
        // return and show a message if the date not match the condition
        if (Game1.dayOfMonth > 3)
        {
            Game1.showRedMessage(I18n.String_CoolCap_Unavailable());
            return;
        }

        // 获取所有作物，然后浇水
        // get all crops, then water them
        var can = new WateringCan();
        var location = Game1.currentLocation;
        var dirts = location.terrainFeatures.Values
            .OfType<HoeDirt>()
            .Concat(location.Objects.Values
                .OfType<IndoorPot>()
                .Select(p => p.hoeDirt.Value))
            .Where(d => d is { crop: not null, state.Value: 0 });

        foreach (var dirt in dirts) dirt.performToolAction(can, 0, dirt.Tile);
    }
}