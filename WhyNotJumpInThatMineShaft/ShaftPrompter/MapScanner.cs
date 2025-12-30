using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Monsters;

namespace WhyNotJumpInThatMineShaft.ShaftPrompter;

public static class MapScanner
{
    public static readonly List<Point> Shafts = new();
    public static readonly List<Point> Stairs = new();
    public static readonly List<Point> Statue = new();
    internal static GreenSlime Slime;

    /// <summary>
    /// Check whether there is a hole in the map.
    /// 检查当前地图上是否存在一个竖井。
    /// </summary>
    public static bool HasAShaftHere => Shafts.Count > 0;

    /// <summary>
    /// Check whether there is a hole in the map.
    /// 检查当前地图上是否存在一个竖井。
    /// </summary>
    public static bool HasAStairHere => Stairs.Count > 0;

    /// <summary>
    /// Check whether there is a calico statue in the map.
    /// 检查当前地图上是否存在一个三花猫雕像。
    /// </summary>
    public static bool HasAStatueHere => Statue.Count > 0;

    /// <summary>
    /// Check whether there is a calico statue in the map.
    /// 检查当前地图上是否存在一个三花猫雕像。
    /// </summary>
    public static bool HasAPrismaticSlimeHere => Slime is not null;

    /// <summary>
    /// A shortcut to get the current level of the player.
    /// 获取玩家当前所在层数。
    /// </summary>
    private static int CurrentLevel => Game1.player.currentLocation is MineShaft mineShaft ? mineShaft.mineLevel : -1;

    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.doCreateLadderDown"/> method.
    /// 游戏尝试生成梯子或洞时，将生成的梯子和洞添加到列表中。
    /// When the game tried to generate stair or shaft, add them to the list.
    /// </summary>
    public static void Patch_doCreateLadderDown(Point point, bool shaft, ref MineShaft __instance)
    {
        if (__instance.mineLevel != CurrentLevel) return;
        if (shaft)
            Shafts.Add(point);
        else
            Stairs.Add(point);
    }

    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.doCreateLadderAt"/> method.
    /// 游戏尝试生成梯子或洞时，将生成的梯子添加到列表中。
    /// When the game tried to generate stair, add it to the list.
    /// </summary>
    public static void Patch_doCreateLadderAt(Vector2 p, ref MineShaft __instance)
    {
        if (__instance.mineLevel != CurrentLevel) return;
        var point = new Point((int)p.X, (int)p.Y);
        Stairs.Add(point);
    }

    /// <summary>
    /// After the player enters a new mine level, scan the current map.
    /// 进入新的一层时，扫描当前楼层。
    /// </summary>
    public static void OnMineLevelChanged(object sender, WarpedEventArgs e)
    {
        // Confirm current location is the Mine or Skull Cavern
        // 确保当前位置是矿井或骷髅洞穴
        if (e.NewLocation is not MineShaft { mineLevel: not 77377 } mineShaft) return;

        Shafts.Clear();
        Stairs.Clear();
        Statue.Clear();
        ModEntry.ShaftPrompter.RefreshSleepTime();
        mineShaft.UpdateHoles();
        mineShaft.UpdatePrismaticSlime();

        if (Slime is not null)
            ModEntry.ModHelper.Events.Player.InventoryChanged += OnInventoryChanged;
    }

    /// <summary>
    /// After killed the prismatic slime, remove the indicator.
    /// 击杀五彩史莱姆后，移除指示器。
    /// </summary>
    private static void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
    {
        if (e.Added.Any(i => i.QualifiedItemId == "(O)876"))
        {
            Slime = null;
            ModEntry.ModHelper.Events.Player.InventoryChanged -= OnInventoryChanged;
        }
    }

    /// <summary>
    /// Traverse each tile to find all coordinates of shafts and stairs.
    /// 遍历当前地图，获取全部的竖井和梯子坐标。
    /// </summary>
    private static void UpdateHoles(this GameLocation location)
    {
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
                    Shafts.Add(new Point(x, y));
                    break;
                case 173:
                    Stairs.Add(new Point(x, y));
                    break;
                case 284:
                    Statue.Add(new Point(x, y));
                    break;
            }
        }
    }

    /// <summary>
    /// Try to find the prismatic slime on the current map.
    /// 寻找当前地图内的五彩史莱姆。
    /// </summary>
    private static void UpdatePrismaticSlime(this GameLocation location)
    {
        if (!Game1.player.team.SpecialOrderActive("Wizard2") || Game1.player.Items.ContainsId("(O)876"))
            return;

        var slime = location.characters
            .OfType<GreenSlime>()
            .FirstOrDefault(s => s.prismatic.Value);
        Slime = slime;
    }
}