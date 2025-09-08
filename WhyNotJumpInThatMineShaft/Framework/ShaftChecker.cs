using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using xTile.Dimensions;

namespace WhyNotJumpInThatMineShaft.Framework;

public static class ShaftChecker
{
    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.checkAction"/> method.
    /// 玩家与地形交互时，检测是否是一个梯子，如果是梯子，检测附近有没有洞。
    /// When the player interacts with the map, check if it is a stair. If true,
    /// check if there are any shafts nearby.
    /// </summary>
    /// <returns>是否需要使用原方法进一步处理，若为 true 则使用原方法继续处理。
    /// Whether it needs to be proceeded with the original method.</returns>
    public static bool Patch_checkAction(Location tileLocation, Farmer who)
    {
        if (!who.IsLocalPlayer) return true;

        // Check whether the tile is a stair
        var location = Game1.currentLocation;
        var index = location.getTileIndexAt(tileLocation, "Buildings", "mine");
        if (index != 173 || !location.HasAHoleHere()) return true;

        var options2 = new []
        {
            new Response("Go", I18n.Choice_Yes()).SetHotKey(Keys.Y),
            new Response("Do", I18n.Choice_No()).SetHotKey(Keys.Escape)
        };

        location.createQuestionDialogue(I18n.String_Prompt(), options2, "Dungeon");
        return false;
    }

    /// <summary>
    /// Check whether there is a hole in the map.
    /// 检查当前地图上是否存在一个竖井。
    /// </summary>
    public static bool HasAHoleHere(this GameLocation location)
    {
        if (location is not MineShaft { mineLevel: > 120 }) return false;

        var layer = location.map.RequireLayer("Buildings");
        for (var x = 0; x < layer.LayerWidth; x++)
        for (var y = 0; y < layer.LayerHeight; y++)
        {
            var tile = layer.Tiles[x, y];
            var index = tile?.TileIndex;
            if (index == 174) return true;
        }

        return false;
    }
}