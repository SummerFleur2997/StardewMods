using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Triggers;

namespace BetterHatsAPI.Framework;

public static class HatDataHelper
{
    /// <summary>
    /// A dictionary of all hat data. The format is
    /// <see cref="StardewValley.Item.QualifiedItemId"/> -> List[<see cref="HatData"/>].
    /// </summary>
    internal static Dictionary<string, List<HatData>> AllHatData { get; set; }

    /// <summary>
    /// Gets the <see cref="HatData"/> of the specified hat.
    /// 获取指定帽子的 <see cref="HatData"/> 数据。
    /// </summary>
    [NotNull]
    public static List<HatData> GetHatData(this Hat hat) =>
        AllHatData.TryGetValue(hat.QualifiedItemId, out var data) ? data : new List<HatData>();

    /// <summary>
    /// Initialize the hat data from content packs when the mod is loaded.
    /// </summary>
    public static void LoadContentPacks(IModHelper helper)
    {
        var log = ModEntry.Log;
        log("Loading content packs and local data files.");

        var allHatData = new Dictionary<string, List<HatData>>();

        /*****
         * Some logic below is copied from Esca-MMC's FTM mod, purported under the MIT license.
         ****/
        foreach (var pack in helper.ContentPacks.GetOwned())
        {
            log($"Loading content pack: {pack.Manifest.UniqueID}");
            Dictionary<string, HatData> data;
            try
            {
                // load the content pack's farm config (null if it doesn't exist)
                data = pack.ReadJsonFile<Dictionary<string, HatData>>("content.json");
            }
            catch (Exception ex)
            {
                log($"Warning: This content pack could not be parsed correctly: {pack.Manifest.Name}", LogLevel.Warn);
                log("Please edit the content.json file or reinstall the content pack. ", LogLevel.Warn);
                log("The auto-generated error message is displayed below:", LogLevel.Warn);
                log("----------", LogLevel.Warn);
                log($"{ex.Message}", LogLevel.Warn);
                continue; // skip to the next content pack
            }

            // no config file found for this farm
            if (data == null)
            {
                log($"Warning: The content.json file for this content pack could not be found: {pack.Manifest.Name}",
                    LogLevel.Warn);
                log("Please reinstall the content pack. If you are its author, " +
                    "please create a config file named content.json in the pack's main folder.", LogLevel.Warn);
                continue; // skip to the next content pack
            }

            foreach (var (key, hatInfo) in data)
            {
                // set some required fields if they are not set
                hatInfo.Pack = pack;
                if (string.IsNullOrWhiteSpace(hatInfo.UniqueBuffID))
                    hatInfo.UniqueBuffID = pack.Manifest.UniqueID;
                if (string.IsNullOrWhiteSpace(hatInfo.BuffDescription))
                    hatInfo.BuffDescription = null;

                // add the data to the dictionary
                allHatData.TryAdd(key, hatInfo);
            }
        }

        AllHatData = allHatData;
    }

    /// <summary>
    /// Try to add a HatData object to the dictionary. 
    /// </summary>
    /// <param name="dict"> The target dictionary, where the key is
    /// the item ID and the value is a list of HatData. </param>
    /// <param name="key">The unique item ID of the hat. </param>
    /// <param name="value">The HatData object to be added. </param>
    private static void TryAdd(this Dictionary<string, List<HatData>> dict, string key, HatData value)
    {
        // Check if the key already exists in the dictionary
        if (!dict.ContainsKey(key))
            dict[key] = new List<HatData>();

        // Add the value to the list corresponding to the key
        dict[key].Add(value);
    }
}

/*****
 * This is a partial class of <see cref="HatData"/>. It contains the
 * logic for converting the hat to a buff, condition checker and
 * action trigger.
 */
public partial class HatData
{
    /// <summary>
    /// Convert this hat data to a buff.
    /// </summary>
    public Buff ConvertToBuff()
    {
        var buff = new Buff(UniqueBuffID);
        var hatName = Game1.player.hat?.Value?.DisplayName ?? string.Empty;
        buff.source = $"{hatName} ({Pack.Manifest.Name})";

        if (!string.IsNullOrWhiteSpace(BuffDescription))
            buff.description = BuffDescription;

        buff.effects.CombatLevel.Value = CombatLevel;
        buff.effects.FarmingLevel.Value = FarmingLevel;
        buff.effects.FishingLevel.Value = FishingLevel;
        buff.effects.MiningLevel.Value = MiningLevel;
        buff.effects.LuckLevel.Value = LuckLevel;
        buff.effects.ForagingLevel.Value = ForagingLevel;
        buff.effects.MaxStamina.Value = MaxStamina;
        buff.effects.MagneticRadius.Value = MagneticRadius;
        buff.effects.Speed.Value = Speed;
        buff.effects.Attack.Value = Attack;
        buff.effects.Defense.Value = Defense;
        buff.effects.Immunity.Value = Immunity;
        buff.effects.AttackMultiplier.Value = AttackMultiplier;
        buff.effects.CriticalChanceMultiplier.Value = CriticalChanceMultiplier;
        buff.effects.CriticalPowerMultiplier.Value = CriticalPowerMultiplier;
        buff.effects.WeaponSpeedMultiplier.Value = WeaponSpeedMultiplier;
        buff.effects.WeaponPrecisionMultiplier.Value = WeaponPrecisionMultiplier;
        buff.effects.KnockbackMultiplier.Value = KnockbackMultiplier;

        buff.millisecondsDuration = Buff.ENDLESS;
        return buff;
    }

    /// <summary>
    /// Convert this hat data to an empty buff to overwrite the existing one.
    /// </summary>
    public Buff ConvertToEmptyBuff() => new(UniqueBuffID) { millisecondsDuration = 100 };

    /// <summary>
    /// Checks if the condition is met.
    /// </summary>
    /// <returns>
    /// True, if the trigger is None or the condition is met;
    /// False otherwise.
    /// </returns>
    public bool CheckCondition() => GameStateQuery.CheckConditions(Condition);

    /// <summary>
    /// Try to perform the action. If something goes wrong, log the error.
    /// </summary>
    public void TryPerformAction()
    {
        if (string.IsNullOrWhiteSpace(Action)) return;
        TriggerActionManager.TryRunAction(Action, out var error, out var ex);
        if (ex != null)
            ModEntry.Log($"Error while performing action '{Action}' for content pack {Pack.Manifest.UniqueID}: \n" +
                         $"{error}", LogLevel.Warn);
    }
}