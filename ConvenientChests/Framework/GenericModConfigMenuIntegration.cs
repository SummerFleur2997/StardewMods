using System;
using System.Linq;
using StardewModdingAPI;

namespace ConvenientChests.Framework;

internal class GenericModConfigMenuIntegration
{
    public static void Register(IManifest manifest, IModRegistry modRegistry, IMonitor monitor,
        Func<ModConfig> getConfig, Action reset, Action save)
    {
        // get API
        IGenericModConfigMenuApi api = IntegrationHelper.GetGenericModConfigMenu(modRegistry, monitor);
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
            name: I18n.Config_Radius,
            getValue: () => getConfig().CraftRadius,
            setValue: value => getConfig().CraftRadius = value
        );
        
        api.AddSectionTitle(manifest, I18n.Config_StashToNearby_Title, I18n.Config_StashToNearby_Desc);
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Active,
            getValue: () => getConfig().StashToNearbyChests,
            setValue: value => getConfig().StashToNearbyChests = value
        );
        api.AddNumberOption(
            manifest,
            name: I18n.Config_Radius,
            getValue: () => getConfig().StashRadius,
            setValue: value => getConfig().StashRadius = value
        );
        api.AddKeybind(
            manifest,
            name: I18n.Config_StashToNearby_Key,
            getValue: () => getConfig().StashKey,
            setValue: value => getConfig().StashKey = value
        );
        
        api.AddSectionTitle(manifest, I18n.Config_StashAnywhere_Title, I18n.Config_StashAnywhere_Desc);
        api.AddBoolOption(
            manifest,
            name: I18n.Config_Active,
            getValue: () => getConfig().StashAnywhere,
            setValue: value => getConfig().StashAnywhere = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashAnywhere_Fridge,
            getValue: () => getConfig().StashAnywhereToFridge,
            setValue: value => getConfig().StashAnywhereToFridge = value
        );
        api.AddBoolOption(
            manifest,
            name: I18n.Config_StashAnywhere_Exist,
            getValue: () => getConfig().StashToExistingStacks,
            setValue: value => getConfig().StashToExistingStacks = value
        );
        api.AddKeybind(
            manifest,
            name: I18n.Config_StashAnywhere_Key,
            getValue: () => getConfig().StashAnywhereKey,
            setValue: value => getConfig().StashAnywhereKey = value
        );
    }
}