using Microsoft.Xna.Framework.Input;
using xTile.Dimensions;

namespace WhyNotJumpInThatMineShaft.ShaftPrompter;

internal static class ShaftPrompterPatches
{
    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.checkAction"/> method.
    /// 玩家与地形交互时，检测是否是一个梯子，如果是梯子，检测附近有没有洞。
    /// When the player interacts with the map, check if it is a stair. If true,
    /// check if there are any shafts nearby.
    /// </summary>
    /// <returns>是否需要使用原方法进一步处理，若为 true 则使用原方法继续处理。
    /// Whether it needs to be proceeded with the original method.</returns>
    public static bool Patch_checkAction(Location tileLocation, Farmer who, ref bool __result)
    {
        if (!who.IsLocalPlayer) return true;
        var location = Game1.currentLocation;
        var index = location.getTileIndexAt(tileLocation, "Buildings", "mine");

        switch (index)
        {
            // Check whether the tile is a stair, and there are any shafts here
            // 检查这个地块是否是一个梯子，以及此处是否有竖井
            case 173 when ModEntry.Config.ShaftPrompter && MapScanner.HasAShaftHere:
                // Generate an option menu
                // 生成选项菜单
                var options2 = new[]
                {
                    new Response("Do", I18n.Choice_No()).SetHotKey(Keys.Escape),
                    new Response("Go", I18n.Choice_Yes()).SetHotKey(Keys.Y)
                };
                location.createQuestionDialogue(I18n.String_Prompt(), options2, "Dungeon");

                __result = true;
                return false;

            // Set field MapScanner.Statue to empty
            // 将属性 MapScanner.Statue 清空
            case 284:
                MapScanner.Statue.Clear();
                return true;
            default:
                return true;
        }
    }
}