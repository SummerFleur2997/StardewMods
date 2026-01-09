using Common.ConfigurationServices;

namespace BetterRetainingSoils.Framework;

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

        // 【标题】水分保持时间 
        // [Title] Water maintaining days
        api.AddSectionTitle(manifest, I18n.Config_WaterMaintain);

        // 【数值输入框】初级保湿土壤
        // [Number inputbox] Basic Retaining Soil
        api.AddNumberOption(
            manifest,
            name: I18n.Config_BasicSoilRemainDays_Title,
            tooltip: I18n.Config_BasicSoilRemainDays_Desc,
            getValue: () => getConfig().BasicSoilRemainDays,
            setValue: value => getConfig().BasicSoilRemainDays = value
        );

        // 【数值输入框】高级保湿土壤
        // [Number inputbox] Quality Retaining Soil
        api.AddNumberOption(
            manifest,
            name: I18n.Config_QualitySoilRemainDays_Title,
            tooltip: I18n.Config_QualitySoilRemainDays_Desc,
            getValue: () => getConfig().QualitySoilRemainDays,
            setValue: value => getConfig().QualitySoilRemainDays = value
        );
    }
}