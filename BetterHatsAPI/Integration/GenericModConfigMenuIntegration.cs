using Common.ConfigurationServices;

namespace BetterHatsAPI.Integration;

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

        // 【选项】禁用帧循环条件检查
        // [Checkbox] Disable Per-Frame Condition Check
        api.AddBoolOption(
            manifest,
            name: I18n.Config_DisableTickUpdate_Name,
            tooltip: I18n.Config_DisableTickUpdate_Desc,
            getValue: () => ModEntry.Config.DisableTickUpdateChecker,
            setValue: value => ModEntry.Config.DisableTickUpdateChecker = value
        );

        // 【按键绑定列表】图鉴菜单快捷键
        // [Key-bind list] Hat Guide Book Menu Shortcut
        api.AddKeybindList(
            manifest,
            name: I18n.Config_GuideBook_Name,
            tooltip: I18n.Config_GuideBook_Desc,
            getValue: () => ModEntry.Config.OpenGuideBookKey,
            setValue: value => ModEntry.Config.OpenGuideBookKey = value
        );

        ModEntry.Log("Successfully added configurations to GMCM.");
    }
}