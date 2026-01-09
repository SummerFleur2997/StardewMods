using Common.ConfigurationServices;

namespace WhyNotJumpInThatMineShaft.Framework;

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

        // 【标题】图形提示
        // [Title] Graphic Prompter
        api.AddSectionTitle(manifest, I18n.Config_GraphicPrompter);

        // 【选项】启用竖井提示
        // [Checkbox] Enable shaft prompter
        api.AddBoolOption(
            manifest,
            name: I18n.Config_ShaftPrompter_Title,
            tooltip: I18n.Config_ShaftPrompter_Desc,
            getValue: () => ModEntry.Config.ShaftPrompter,
            setValue: value => ModEntry.Config.ShaftPrompter = value
        );

        // 【选项】显示竖井指示器
        // [Checkbox] Show shaft indicator
        api.AddBoolOption(
            manifest,
            name: I18n.Config_ShaftIndicator_Title,
            tooltip: I18n.Config_ShaftIndicator_Desc,
            getValue: () => ModEntry.Config.ShaftIndicator,
            setValue: value => ModEntry.Config.ShaftIndicator = value
        );

        // 【选项】显示梯子指示器
        // [Checkbox] Show stair indicator
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StairIndicator_Title,
            tooltip: I18n.Config_StairIndicator_Desc,
            getValue: () => ModEntry.Config.StairIndicator,
            setValue: value => ModEntry.Config.StairIndicator = value
        );

        // 【选项】显示三花猫雕像指示器
        // [Checkbox] Show calico statue indicator
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StatueIndicator_Title,
            tooltip: I18n.Config_StatueIndicator_Desc,
            getValue: () => ModEntry.Config.StatueIndicator,
            setValue: value => ModEntry.Config.StatueIndicator = value
        );

        // 【数值选择条】指示器最小距离
        // [Number bar] Indicator minium distance
        api.AddNumberOption(
            manifest,
            name: I18n.Config_IndicatorMinDistance_Title,
            tooltip: I18n.Config_IndicatorMinDistance_Desc,
            getValue: () => ModEntry.Config.HideDistance,
            setValue: value => ModEntry.Config.HideDistance = value,
            min: 1,
            max: 10,
            interval: 1
        );

        // 【数值选择条】指示器缩放比例
        // [Number bar] Text scale
        api.AddNumberOption(
            manifest,
            name: I18n.Config_IndicatorScale_Title,
            tooltip: I18n.Config_IndicatorScale_Desc,
            getValue: () => ModEntry.Config.IndicatorScale,
            setValue: value => ModEntry.Config.IndicatorScale = value,
            min: 0.1f,
            max: 3.0f,
            interval: 0.1f
        );

        // 【标题】文字提示
        // [Title] Text Prompter
        api.AddSectionTitle(manifest, I18n.Config_TextPrompter);

        // 【选项】启用文字提示
        // [Checkbox] Enable text prompter
        api.AddBoolOption(
            manifest,
            name: I18n.Config_TextPrompter_Title,
            tooltip: I18n.Config_TextPrompter_Desc,
            getValue: () => ModEntry.Config.TextPrompter,
            setValue: value => ModEntry.Config.TextPrompter = value
        );

        // 【数值输入框】文字 X 坐标
        // [Number inputbox] Text X position
        api.AddNumberOption(
            manifest,
            name: I18n.Config_TextPositionX_Title,
            tooltip: I18n.Config_TextPositionX_Desc,
            getValue: () => ModEntry.Config.TextPositionX,
            setValue: value => ModEntry.Config.TextPositionX = value
        );

        // 【数值输入框】文字 Y 坐标
        // [Number inputbox] Text Y position
        api.AddNumberOption(
            manifest,
            name: I18n.Config_TextPositionY_Title,
            tooltip: I18n.Config_TextPositionY_Desc,
            getValue: () => ModEntry.Config.TextPositionY,
            setValue: value => ModEntry.Config.TextPositionY = value
        );

        // 【数值选择条】文字缩放比例
        // [Number bar] Text scale
        api.AddNumberOption(
            manifest,
            name: I18n.Config_TextScale_Title,
            tooltip: I18n.Config_TextScale_Desc,
            getValue: () => ModEntry.Config.TextScale,
            setValue: value => ModEntry.Config.TextScale = value,
            min: 0.5f,
            max: 3.0f,
            interval: 0.1f
        );

        // 【选项】显示方向
        // [Checkbox] Show direction
        api.AddBoolOption(
            manifest,
            name: I18n.Config_ShowDirection_Title,
            tooltip: I18n.Config_ShowDirection_Desc,
            getValue: () => ModEntry.Config.ShowDirection,
            setValue: value => ModEntry.Config.ShowDirection = value
        );

        // 【选项】显示距离
        // [Checkbox] Show distance
        api.AddBoolOption(
            manifest,
            name: I18n.Config_ShowDistance_Title,
            tooltip: I18n.Config_ShowDistance_Desc,
            getValue: () => ModEntry.Config.ShowDistance,
            setValue: value => ModEntry.Config.ShowDistance = value
        );

        // 【选项】显示可生成竖井提示
        // [Checkbox] Show shaft generatable indicator
        api.AddBoolOption(
            manifest,
            name: I18n.Config_ShaftGeneratableIndicator_Title,
            tooltip: I18n.Config_ShaftGeneratableIndicator_Desc,
            getValue: () => ModEntry.Config.ShaftGeneratableIndicator,
            setValue: value => ModEntry.Config.ShaftGeneratableIndicator = value
        );
    }
}