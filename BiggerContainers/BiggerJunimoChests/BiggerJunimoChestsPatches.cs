using BiggerContainers.Framework;
using StardewValley.Objects;

namespace BiggerContainers.BiggerJunimoChests;

internal static class BiggerJunimoChestsPatches
{
    /// <summary>
    /// Patches <see cref="StardewValley.Objects.Chest.GetActualCapacity"/> method.
    /// 如果箱子是一个有效的祝尼魔箱，并且相应的配置被启用，则修改其容量。If the chest is a valid junimo chest 
    /// and the corresponding configuration is enabled, modifies its capacity.
    /// </summary>
    /// <param name="__instance">要检查和调整容量的箱子。 The chest to check and potentially modify.</param>
    /// <param name="__result">原函数的返回值。The return value of the original method.</param>
    /// <returns>是否需要使用原方法进一步处理，若为 true 则使用原方法继续处理。
    /// Whether it needs to be proceeded with the original method.</returns>
    public static bool Patch_GetActualCapacity(Chest __instance, ref int __result)
    {
        if (__instance.MyChestType() is not ChestTypes.JunimoChest) return true;
        if (!ModEntry.JunimoChestsModule.IsActive) return true;

        __result = ModEntry.Config.BiggerJunimoChest switch
        {
            1 => 36,
            2 => 70,
            _ => 9
        };
        return false;
    }
}