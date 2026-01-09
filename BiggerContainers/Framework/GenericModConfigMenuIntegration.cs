using Common.ConfigurationServices;

namespace BiggerContainers.Framework;

internal static class GenericModConfigMenuIntegration
{
    /// <summary>
    /// 向 Generic Mod Config Menu 的配置菜单中添加配置项。
    /// Adds configuration options to the Generic Mod Config Menu.
    /// </summary>
    /// <param name="manifest">本模组的注册名单 The mod's manifest</param>
    /// <param name="modRegistry">本模组的元数据 Metadata about loaded mods</param>
    /// <param name="getConfig">读取配置操作 Actions to read the mod's current config</param>
    /// <param name="reset">初始化配置操作 Actions to reset the mod's config to its default</param>
    /// <param name="save">保存配置操作 Actions when save the mod's new config</param>
    /// <param name="log">输出日志的操作 Action to output a log on screen</param>
    public static void Register(IManifest manifest, IModRegistry modRegistry,
        Func<ModConfig> getConfig, Action reset, Action save, Action<string, LogLevel> log)
    {
        // 获取 GenericModConfigMenu 模组 API
        // get GenericModConfigMenu API
        var api = IntegrationHelper.GetGenericModConfigMenu(modRegistry, log);
        if (api == null)
            return;

        // 注册可视化界面
        // register config UI
        api.Register(manifest, reset, save);

        // 【标题】更大的冰箱 - 让冰箱的容量变得更大
        // [Title] Bigger Fridges - Make fridges bigger
        api.AddSectionTitle(manifest, I18n.Config_FridgeModule);

        // 【选项】更大的冰箱
        // [Checkbox] Bigger main fridge
        api.AddBoolOption(
            manifest,
            name: I18n.Config_BiggerFridge_Title,
            tooltip: I18n.Config_BiggerFridge_Desc,
            getValue: () => getConfig().BiggerFridge,
            setValue: value => getConfig().BiggerFridge = value
        );

        // 【选项】更大的迷你冰箱
        // [Checkbox] Bigger mini fridge
        api.AddBoolOption(
            manifest,
            name: I18n.Config_BiggerMiniFridge_Title,
            tooltip: I18n.Config_BiggerMiniFridge_Desc,
            getValue: () => getConfig().BiggerMiniFridge,
            setValue: value => getConfig().BiggerMiniFridge = value
        );

        // 【标题】更大的祝尼魔箱 - 让祝尼魔箱的容量变得更大
        // [Title] Bigger Junimo Chests - Make junimo chests bigger
        api.AddSectionTitle(manifest, I18n.Config_JunimoChestModule);

        // 【选项】更大的祝尼魔箱
        // [Checkbox] Bigger junimo chests
        api.AddNumberOption(
            manifest,
            name: I18n.Config_BiggerJunimoChest_Title,
            tooltip: I18n.Config_BiggerJunimoChest_Desc,
            getValue: () => getConfig().BiggerJunimoChest,
            setValue: value => getConfig().BiggerJunimoChest = value,
            min: 0,
            max: 2,
            interval: 1,
            formatValue: i => i switch
            {
                0 => I18n.Config_BiggerJunimoChest_0(),
                1 => I18n.Config_BiggerJunimoChest_1(),
                2 => I18n.Config_BiggerJunimoChest_2(),
                _ => I18n.Config_BiggerJunimoChest_E()
            }
        );
    }
}