using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using xTile.Tiles;

namespace WhyNotJumpInThatMineShaft.Framework;

internal static class DebugModule
{
    private static void OnButtonChanged(object sender, ButtonPressedEventArgs e)
    {
        var keybind = KeybindList.Parse("B");
        if (keybind.JustPressed()) DrawShaftHere();
    }

    private static void DrawShaftHere()
    {
        var location = Game1.currentLocation;
        if (location is not MineShaft { mineLevel: > 120 }) return;

        var x = (Game1.getMouseX() + Game1.viewport.X) / 64;
        var y = (Game1.getMouseY() + Game1.viewport.Y) / 64;
        var map = location.map;
        var layer = map.RequireLayer("Buildings");
        var tileSheet = map.RequireTileSheet(0, "mine");
        layer.Tiles[x, y] = new StaticTile(layer, tileSheet, BlendMode.Alpha, 174);
        MapScanner.Shafts.Add(new Point(x, y));
    }

    private static void ToggleDebug(string command, string[] args)
    {
        if (args.Length != 2 && args[0] != "debug") return;
        switch (args[1])
        {
            case "on":
                ModEntry.ModHelper.Events.Input.ButtonPressed += OnButtonChanged;
                ModEntry.Log("已启用调试功能！", LogLevel.Info);
                break;
            case "off":
                ModEntry.ModHelper.Events.Input.ButtonPressed -= OnButtonChanged;
                ModEntry.Log("已禁用调试功能！", LogLevel.Info);
                break;
        }
    }

    public static void Initialize()
    {
        ModEntry.ModHelper.ConsoleCommands.Add("wnj",
            "启用调试模式。",
            ToggleDebug);
    }
}