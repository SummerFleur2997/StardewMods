using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ConvenientChests.Framework.ConfigurationService;

public interface IGenericModConfigMenuApi
{
    /// <summary>
    /// 注册本 Mod，使之配置文件能被 Generic Mod Config Menu 以可视化形式编辑。
    /// Register a mod whose config can be edited through the UI.
    /// </summary>
    /// <param name="manifest">本模组的注册名单 The mod's manifest</param>
    /// <param name="reset">初始化配置操作 Actions to reset the mod's config to its default</param>
    /// <param name="save">保存配置操作 Actions when save the mod's current config</param>
    /// <param name="titleScreenOnly">
    /// 配置是否只能在游戏标题界面更改？
    /// Whether the options can only be edited from the title screen.
    /// </param>
    void Register(IManifest manifest, Action reset, Action save, bool titleScreenOnly = false);

    /// <summary>
    /// 章节标题控件。 Section title element.
    /// </summary>
    /// <param name="manifest">本模组的注册名单 The mod's manifest</param>
    /// <param name="text">标题文字 The label text of the section title</param>
    /// <param name="tooltip">悬停提示信息 Tips shown when the cursor hovers</param>
    void AddSectionTitle(IManifest manifest, Func<string> text, Func<string> tooltip = null);

    /// <summary>
    /// 复选框控件。 Checkbox element.
    /// </summary>
    /// <param name="manifest">本模组的注册名单 The mod's manifest</param>
    /// <param name="getValue">读取配置值操作 Actions to read the current config value</param>
    /// <param name="setValue">设置配置值操作 Actions to save the new config value</param>
    /// <param name="name">复选框控件的名称 The label text of the checkbox</param>
    /// <param name="tooltip">悬停提示信息 Tips shown when the cursor hovers</param>
    /// <param name="fieldId">控件唯一标识符 Unique field ID of the element</param>
    void AddBoolOption(IManifest manifest, Func<bool> getValue, Action<bool> setValue, Func<string> name,
        Func<string> tooltip = null, string fieldId = null);

    /// <summary>
    /// 数字填框控件。 Number input element.
    /// </summary>
    /// <param name="manifest">本模组的注册名单 The mod's manifest</param>
    /// <param name="getValue">读取配置值操作 Actions to read the current config value</param>
    /// <param name="setValue">设置配置值操作 Actions to save the new config value</param>
    /// <param name="name">复选框控件的名称 The label text of the number inputbox</param>
    /// <param name="tooltip">悬停提示信息 Tips shown when the cursor hovers</param>
    /// <param name="min">最小值 Minium value</param>
    /// <param name="max">最大值 Maxium value</param>
    /// <param name="interval">可选择的数值间隔 The interval of values that can be selected.</param>
    /// <param name="formatValue">数字格式化操作方法 Function to format the number value</param>
    /// <param name="fieldId">控件唯一标识符 Unique field ID of the element</param>
    void AddNumberOption(IManifest manifest, Func<int> getValue, Action<int> setValue, Func<string> name,
        Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null,
        Func<int, string> formatValue = null, string fieldId = null);

    /// <summary>
    /// 按键绑定列表控件。 Key-bind list element.
    /// </summary>
    /// <param name="manifest">本模组的注册名单 The mod's manifest</param>
    /// <param name="getValue">读取配置值操作 Actions to read the current config value</param>
    /// <param name="setValue">设置配置值操作 Actions to save the new config value</param>
    /// <param name="name">按键绑定控件的名称 The label text of the key-bind list</param>
    /// <param name="tooltip">悬停提示信息 Tips shown when the cursor hovers</param>
    /// <param name="fieldId">控件唯一标识符 Unique field ID of the element</param>
    void AddKeybindList(IManifest manifest, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name,
        Func<string> tooltip = null, string fieldId = null);
}