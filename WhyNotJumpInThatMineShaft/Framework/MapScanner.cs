using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;

namespace WhyNotJumpInThatMineShaft.Framework;

public static class MapScanner
{
    public static readonly List<Point> Holes = [];

    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.doCreateLadderDown"/> method.
    /// 游戏尝试生成梯子或洞时，检测是否生成了洞。
    /// When the game tried to generate a stair or shaft, check if it is a shaft.
    /// </summary>
    public static void Patch_doCreateLadderDown(bool shaft)
    {
        if (shaft) Game1.currentLocation.UpdateHoles();
    }

    private static void UpdateHoles(this GameLocation location)
    {
        if (location is not MineShaft { mineLevel: > 120 }) return;

        var layer = location.map.RequireLayer("Buildings");
        for (var x = 0; x < layer.LayerWidth; x++)
        for (var y = 0; y < layer.LayerHeight; y++)
        {
            var tile = layer.Tiles[x, y];
            var index = tile?.TileIndex;
            if (index == 174) Holes.Add(new Point(x, y));
        }

        // todo 调试用，记得发布时删除
        if (Holes.Count == 0) return;
        ModEntry.Log("当前层洞的列表为：", LogLevel.Debug);
        foreach (var hole in Holes)
            ModEntry.Log($"{hole.X}, {hole.Y}", LogLevel.Debug);
    }

    public static void OnMineLevelChanged(object sender, WarpedEventArgs e)
    {
        Holes.Clear();
        e.NewLocation.UpdateHoles();
    }
}