using System;
using Common;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using WhyNotJumpInThatMineShaft.Framework;
using xTile.Dimensions;

namespace WhyNotJumpInThatMineShaft;

internal class ShaftPrompter : IModule
{
    public bool IsActive => ModEntry.Config.ShaftPrompter;

    public void Activate()
    {
        lock (ModEntry.Config)
        {
            var harmony = ModEntry.Harmony;
            RegisterHarmonyPatches(harmony);
        }
    }

    public void Deactivate() { }

    private static void RegisterHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalM1 = AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkAction));
            var prefixM1 = AccessTools.Method(typeof(ShaftPrompter), nameof(Patch_checkAction));
            harmony.Patch(original: originalM1, prefix: new HarmonyMethod(prefixM1));
            ModEntry.Log("Patched MineShaft.checkAction successfully.");
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed to patch method: {ex.Message}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Patches <see cref="StardewValley.Locations.MineShaft.checkAction"/> method.
    /// 玩家与地形交互时，检测是否是一个梯子，如果是梯子，检测附近有没有洞。
    /// When the player interacts with the map, check if it is a stair. If true,
    /// check if there are any shafts nearby.
    /// </summary>
    /// <returns>是否需要使用原方法进一步处理，若为 true 则使用原方法继续处理。
    /// Whether it needs to be proceeded with the original method.</returns>
    private bool Patch_checkAction(Location tileLocation, Farmer who, ref bool __result)
    {
        if (!IsActive ||!who.IsLocalPlayer) return true;

        // Check whether the tile is a stair, and there are any shafts here
        // 检查这个地块是否是一个梯子，以及此处是否有竖井
        var location = Game1.currentLocation;
        var index = location.getTileIndexAt(tileLocation, "Buildings", "mine");
        if (index != 173 || !location.HasAHoleHere()) return true;

        // Generate an option menu
        // 生成选项菜单
        var options2 = new []
        {
            new Response("Go", I18n.Choice_Yes()).SetHotKey(Keys.Y),
            new Response("Do", I18n.Choice_No()).SetHotKey(Keys.Escape)
        };
        location.createQuestionDialogue(I18n.String_Prompt(), options2, "Dungeon");

        __result = true;
        return false;
    }
}