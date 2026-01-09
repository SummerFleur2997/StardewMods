using StardewModdingAPI.Utilities;

namespace Common.ConfigurationServices;

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

    /// <summary>Add a section title at the current position in the form.</summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="text">The title text shown in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the title, or <c>null</c> to disable the tooltip.</param>
    void AddSectionTitle(IManifest manifest, Func<string> text, Func<string> tooltip = null);

    /// <summary>Add a paragraph of text at the current position in the form.</summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="text">The paragraph text to display.</param>
    void AddParagraph(IManifest manifest, Func<string> text);

    /// <summary>Add a boolean option at the current position in the form.</summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="fieldId">The unique field ID for use with OnFieldChanged, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddBoolOption(IManifest manifest, Func<bool> getValue, Action<bool> setValue, Func<string> name,
        Func<string> tooltip = null, string fieldId = null);

    /// <summary>Add an integer option at the current position in the form.</summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
    /// <param name="fieldId">The unique field ID for use with OnFieldChanged, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddNumberOption(IManifest manifest, Func<int> getValue, Action<int> setValue, Func<string> name,
        Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null,
        Func<int, string> formatValue = null, string fieldId = null);

    /// <summary>Add a float option at the current position in the form.</summary>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="min">The minimum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="max">The maximum allowed value, or <c>null</c> to allow any.</param>
    /// <param name="interval">The interval of values that can be selected.</param>
    /// <param name="formatValue">Get the display text to show for a value, or <c>null</c> to show the number as-is.</param>
    /// <param name="fieldId">The unique field ID for use with <c>OnFieldChanged</c>, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name,
        Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null,
        Func<float, string> formatValue = null, string fieldId = null);


    /// <summary>Add a keybind at the current position in the form.</summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="fieldId">The unique field ID for use with OnFieldChanged, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddKeybind(IManifest manifest, Func<SButton> getValue, Action<SButton> setValue, Func<string> name,
        Func<string> tooltip = null, string fieldId = null);

    /// <summary>Add a keybind list at the current position in the form.</summary>
    /// <param name="manifest">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="fieldId">The unique field ID for use with OnFieldChanged, or <c>null</c> to auto-generate a randomized ID.</param>
    void AddKeybindList(IManifest manifest, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name,
        Func<string> tooltip = null, string fieldId = null);
}