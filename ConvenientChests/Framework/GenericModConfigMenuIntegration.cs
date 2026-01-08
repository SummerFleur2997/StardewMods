using System;
using Common.ConfigurationServices;
using StardewModdingAPI;

namespace ConvenientChests.Framework;

internal static class GenericModConfigMenuIntegration
{
    /// <summary>
    /// 向 Generic Mod Config Menu 的配置菜单中添加配置项。
    /// Adds configuration options to the Generic Mod Config Menu.
    /// </summary>
    /// <param name="manifest">本模组的注册名单 The mod's manifest</param>
    /// <param name="modRegistry">本模组的元数据 Metadata about loaded mods</param>
    /// <param name="reset">初始化配置操作 Actions to reset the mod's config to its default</param>
    /// <param name="save">保存配置操作 Actions when save the mod's new config</param>
    public static void Register(IManifest manifest, IModRegistry modRegistry, Action reset, Action save)
    {
        // 获取 GenericModConfigMenu 模组 API
        // get GenericModConfigMenu API
        var api = IntegrationHelper.GetGenericModConfigMenu(modRegistry, ModEntry.Log);
        if (api == null)
            return;

        // 注册可视化界面
        // register config UI
        api.Register(manifest, reset, save);

        // 【标题】存储归类 - 对箱子的内容物进行归类
        // [Title] Categorize chests - Categorize your chests by items
        api.AddSectionTitle(manifest, I18n.Config_Categorize_Title, I18n.Config_Categorize_Desc);

        // 【选项】启用
        // [Checkbox] active
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Active,
            getValue: () => ModEntry.Config.CategorizeChests,
            setValue: value => ModEntry.Config.CategorizeChests = value
        );

        // 【选项】物品种类排序 - 按字母顺序对种类进行排序，不建议中文用户启用
        // [Checkbox] Sort categories - Sort categories alphabetically
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Categorize_Sort,
            tooltip: I18n.Config_Categorize_Sort_Desc,
            getValue: () => ModEntry.Config.EnableSort,
            setValue: value => ModEntry.Config.EnableSort = value
        );

        // 【标题】从附近的箱子里打造 - 使用附近箱子里的物品进行打造
        // [Title] Craft from chests - Use items from nearby chests to craft
        api.AddSectionTitle(manifest, I18n.Config_CraftFromChest_Title, I18n.Config_CraftFromChest_Desc);

        // 【选项】启用
        // [Checkbox] active
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Active,
            getValue: () => ModEntry.Config.CraftFromChests,
            setValue: value => ModEntry.Config.CraftFromChests = value
        );

        // 【数值输入框】搜寻半径
        // [Number inputbox] Craft from chests' radius
        api.AddNumberOption(
            manifest,
            name: I18n.Config_CraftFromChest_Radius,
            getValue: () => ModEntry.Config.CraftRadius,
            setValue: value => ModEntry.Config.CraftRadius = value
        );

        // 【标题】存储至箱子 - 使用快捷键将物品存储至箱子中
        // [Title] Stash to chests - Stash items to chests using a hot key
        api.AddSectionTitle(manifest, I18n.Config_StashToChests_Title, I18n.Config_StashToChests_Desc);

        // 【选项】存储至附近的箱子 - 将物品快速存储至附近的箱子里
        // [Checkbox] Stash to nearby - Allows for items to be stashed to chests in the player's vicinity
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashToNearby_Title,
            tooltip: I18n.Config_StashToNearby_Desc,
            getValue: () => ModEntry.Config.StashToNearby,
            setValue: value => ModEntry.Config.StashToNearby = value
        );

        // 【选项】存储至任意位置的箱子 - 将物品快速存储至世界任意位置的箱子里
        // [Checkbox] Stash from anywhere - Allows for items to be stashed to any chest accessible to the player
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashAnywhere_Title,
            tooltip: I18n.Config_StashAnywhere_Desc,
            getValue: () => ModEntry.Config.StashAnywhere,
            setValue: value => ModEntry.Config.StashAnywhere = value
        );

        // 【数值输入框】搜寻半径
        // [Number inputbox] Stash to nearby chests radius
        api.AddNumberOption(
            manifest,
            name: I18n.Config_StashToNearby_Radius,
            getValue: () => ModEntry.Config.StashRadius,
            setValue: value => ModEntry.Config.StashRadius = value
        );

        // 【选项】存储至已存在的组
        // [Checkbox] Stash to existing stacks?
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashLogic_Exist,
            getValue: () => ModEntry.Config.StashToExistingStacks,
            setValue: value => ModEntry.Config.StashToExistingStacks = value
        );

        // 【选项】优先存储至冰箱中
        // [Checkbox] Stash to fridge first?
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashLogic_Fridge,
            getValue: () => ModEntry.Config.StashAnywhereToFridge,
            setValue: value => ModEntry.Config.StashAnywhereToFridge = value
        );

        // 【选项】禁用工具存储 - 禁止将任何工具存储至箱子中，在多人游戏中较为实用
        // [Checkbox] Never stash tools - Prevent tools from stash into chests. May useful in multiplayer
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashRule_LockItems,
            tooltip: I18n.Config_StashRule_LockItems_Desc,
            getValue: () => ModEntry.Config.NeverStashTools,
            setValue: value => ModEntry.Config.NeverStashTools = value
        );

        api.AddSectionTitle(manifest, I18n.Config_AutoStash_Title, I18n.Config_AutoStash_Desc);

        // 【选项】禁用工具存储 - 禁止将任何工具存储至箱子中，在多人游戏中较为实用
        // [Checkbox] The Mine - Active auto stash while in the Mines
        api.AddBoolOption(
            manifest,
            name: I18n.Config_AutoStash_Mine,
            tooltip: I18n.Config_AutoStash_Mine_Desc,
            getValue: () => ModEntry.Config.AutoStashInTheMine,
            setValue: value => ModEntry.Config.AutoStashInTheMine = value
        );

        // 【选项】禁用工具存储 - 禁止将任何工具存储至箱子中，在多人游戏中较为实用
        // [Checkbox] Skull Cavern - Active auto stash while in the Skull Cavern
        api.AddBoolOption(
            manifest,
            name: I18n.Config_AutoStash_Skull,
            tooltip: I18n.Config_AutoStash_Skull_Desc,
            getValue: () => ModEntry.Config.AutoStashInSkullCavern,
            setValue: value => ModEntry.Config.AutoStashInSkullCavern = value
        );

        // 【选项】禁用工具存储 - 禁止将任何工具存储至箱子中，在多人游戏中较为实用
        // [Checkbox] Volcano Dungeon - Active auto stash while in the Volcano Dungeon
        api.AddBoolOption(
            manifest,
            name: I18n.Config_AutoStash_Dungeon,
            tooltip: I18n.Config_AutoStash_Dungeon_Desc,
            getValue: () => ModEntry.Config.AutoStashInVolcanoDungeon,
            setValue: value => ModEntry.Config.AutoStashInVolcanoDungeon = value
        );

        // 【标题】按键绑定
        // [Title] Key bind
        api.AddSectionTitle(manifest, I18n.Config_KeyBind_Title);

        // 【按键绑定列表】存储至箱子
        // [Key-bind list] Stash to chests key-bind
        api.AddKeybindList(
            manifest,
            name: I18n.Config_StashToChests_Key,
            getValue: () => ModEntry.Config.StashToNearbyKey,
            setValue: value => ModEntry.Config.StashToNearbyKey = value
        );

        // 【按键绑定列表】存储至任意位置的箱子
        // [Key-bind list] Stash from anywhere key-bind
        api.AddKeybindList(
            manifest,
            name: I18n.Config_StashAnywhere_Key,
            getValue: () => ModEntry.Config.StashAnywhereKey,
            setValue: value => ModEntry.Config.StashAnywhereKey = value
        );

        if (ModEntry.IsAndroid)
        {
            // 【标题】移动端适配
            // [Title] Mobile Adaptation
            api.AddSectionTitle(manifest, I18n.Config_Mobile_Title);
            api.AddNumberOption(
                manifest,
                name: I18n.Config_Mobile_Offset,
                tooltip: I18n.Config_Mobile_Offset_Desc,
                getValue: () => ModEntry.Config.MobileOffset,
                setValue: value => ModEntry.Config.MobileOffset = value,
                min: 0,
                max: 200
            );
        }

        ModEntry.Log("Successfully added configurations to GMCM.");
    }
}