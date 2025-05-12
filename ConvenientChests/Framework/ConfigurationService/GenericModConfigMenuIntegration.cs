using System;
using StardewModdingAPI;

namespace ConvenientChests.Framework.ConfigurationService;

internal static class GenericModConfigMenuIntegration
{
    public static void Register(IManifest manifest, IModRegistry modRegistry,
        Func<ModConfig> getConfig, Action reset, Action save)
    {
        // get API
        var api = IntegrationHelper.GetGenericModConfigMenu(modRegistry);
        if (api == null)
            return;

        // register config UI
        api.Register(manifest, reset, save);

        api.AddSectionTitle(manifest, I18n.Config_Categorize_Title, I18n.Config_Categorize_Desc);
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Active,
            getValue: () => getConfig().CategorizeChests,
            setValue: value => getConfig().CategorizeChests = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Categorize_Sort,
            tooltip: I18n.Config_Categorize_Sort_Desc,
            getValue: () => getConfig().EnableSort,
            setValue: value => getConfig().EnableSort = value
        );

        api.AddSectionTitle(manifest, I18n.Config_CraftFromChest_Title, I18n.Config_CraftFromChest_Desc);
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Active,
            getValue: () => getConfig().CraftFromChests,
            setValue: value => getConfig().CraftFromChests = value
        );
        api.AddNumberOption(
            manifest,
            name: I18n.Config_CraftFromChest_Radius,
            getValue: () => getConfig().CraftRadius,
            setValue: value => getConfig().CraftRadius = value
        );

        api.AddSectionTitle(manifest, I18n.Config_StashToChests_Title, I18n.Config_StashToChests_Desc);

        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashToNearby_Title,
            tooltip: I18n.Config_StashToNearby_Desc,
            getValue: () => getConfig().StashToNearby,
            setValue: value => getConfig().StashToNearby = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashAnywhere_Title,
            tooltip: I18n.Config_StashAnywhere_Desc,
            getValue: () => getConfig().StashAnywhere,
            setValue: value => getConfig().StashAnywhere = value
        );
        api.AddNumberOption(
            manifest,
            name: I18n.Config_StashToNearby_Radius,
            getValue: () => getConfig().StashRadius,
            setValue: value => getConfig().StashRadius = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashLogic_Exist,
            getValue: () => getConfig().StashToExistingStacks,
            setValue: value => getConfig().StashToExistingStacks = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashLogic_Fridge,
            getValue: () => getConfig().StashAnywhereToFridge,
            setValue: value => getConfig().StashAnywhereToFridge = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashRule_LockItems,
            tooltip: I18n.Config_StashRule_LockItems_Desc,
            getValue: () => getConfig().NeverStashTools,
            setValue: value => getConfig().NeverStashTools = value
        );

        api.AddSectionTitle(manifest, I18n.Config_KeyBind_Title);
        api.AddKeybindList(
            manifest,
            name: I18n.Config_StashToChests_Key,
            getValue: () => getConfig().StashToNearbyKey,
            setValue: value => getConfig().StashToNearbyKey = value
        );
        api.AddKeybindList(
            manifest,
            name: I18n.Config_StashAnywhere_Key,
            getValue: () => getConfig().StashAnywhereKey,
            setValue: value => getConfig().StashAnywhereKey = value
        );
    }
}