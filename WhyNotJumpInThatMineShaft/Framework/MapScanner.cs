using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;

namespace WhyNotJumpInThatMineShaft.Framework;

internal static class MapScanner
{
    public static readonly List<Point> Holes = [];
    public static readonly List<Point> Ladders = [];

    /// <summary>
    /// Check whether there is a hole in the map.
    /// 检查当前地图上是否存在一个竖井。
    /// </summary>
    public static bool HasAHoleHere(this GameLocation _) => Holes.Count > 0;

    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.doCreateLadderDown"/> method.
    /// 游戏尝试生成梯子或洞时，将生成的梯子和洞添加到列表中。
    /// When the game tried to generate stair or shaft, add them to the list.
    /// </summary>
    public static void Patch_doCreateLadderDown(Point point, bool shaft)
    {
        if (shaft) 
            Holes.Add(point);
        else
            Ladders.Add(point);
    }

    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.doCreateLadderAt"/> method.
    /// 游戏尝试生成梯子或洞时，将生成的梯子添加到列表中。
    /// When the game tried to generate stair, add it to the list.
    /// </summary>
    public static void Patch_doCreateLadderAt(Vector2 p)
    {
        var point = new Point((int)p.X, (int)p.Y);
        Ladders.Add(point);
    }

    public static void OnMineLevelChanged(object sender, WarpedEventArgs e)
    {
        Holes.Clear();
        Ladders.Clear();
        e.NewLocation.UpdateHoles();
    }

    /// <summary>
    /// 遍历当前地图，获取全部的竖井和梯子坐标。
    /// </summary>
    private static void UpdateHoles(this GameLocation location)
    {
        // Confirm current location is the Mine
        // 确保当前位置是骷髅洞穴
        if (location is not MineShaft) return;

        // Traverse each tile on Buildings layer to find a shaft or stair
        // 遍历 Buildings 图层上的每一个地块，查找是否有竖井或梯子
        var layer = location.map.RequireLayer("Buildings");
        for (var x = 0; x < layer.LayerWidth; x++)
        for (var y = 0; y < layer.LayerHeight; y++)
        {
            var tile = layer.Tiles[x, y];
            var index = tile?.TileIndex;
            switch (index)
            {
                case 174:
                    Holes.Add(new Point(x, y)); 
                    break;
                case 173:
                    Ladders.Add(new Point(x, y));
                    break;
            }
        }
    }
}