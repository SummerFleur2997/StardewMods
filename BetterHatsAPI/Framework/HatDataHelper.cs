using JetBrains.Annotations;
using StardewValley.Extensions;
using StardewValley.Objects;
using StardewValley.Triggers;
using static BetterHatsAPI.Framework.Utilities;

namespace BetterHatsAPI.Framework;

public static class HatDataHelper
{
    /// <summary>
    /// A dictionary of all hat data. The format is
    /// <see cref="StardewValley.Item.QualifiedItemId"/> -> List[<see cref="HatData"/>].
    /// </summary>
    internal static Dictionary<string, List<HatData>> AllHatData { get; private set; }

    /// <summary>
    /// The default order of hats for each content pack.
    /// The format is ContentPackID -> List[HatID].
    /// </summary>
    internal static Dictionary<string, List<string>> Order { get; private set; }

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
    public static void LoadContentPacks()
    {
        Log("Loading content packs and local data files.");

        var allHatData = new Dictionary<string, List<HatData>>();
        var allOrder = new Dictionary<string, List<string>>();

        var defaultOrder = DataLoader.Hats(Game1.content).Keys
            .Select(h => ItemRegistry.ManuallyQualifyItemId(h, ItemRegistry.type_hat))
            .ToList();
        allOrder.Add(HatData.CombinedDataSign, defaultOrder);

        /*****
         * Some logic below is copied from Esca-MMC's FTM mod, purported under the MIT license.
         ****/
        foreach (var pack in ModEntry.ContentPacks)
        {
            Log($"Loading content pack - {pack.Manifest.UniqueID}.");
            Dictionary<string, HatData> data;
            try
            {
                // load the content pack's farm config (null if it doesn't exist)
                data = pack.ReadJsonFile<Dictionary<string, HatData>>("content.json");
            }
            catch (Exception ex)
            {
                Error($"Error: This content pack could not be parsed correctly: {pack.Manifest.Name}");
                Warn("If you are the author, please check your content file.");
                Warn("If you are its user, please reinstall the content pack or report the issue to the author.");
                Warn("The auto-generated error message is displayed below:");
                Warn($"{ex.Message}");
                Warn("--------------------");
                continue; // skip to the next content pack
            }

            // no config file found for this farm
            if (data == null)
            {
                Warn($"Warning: The content.json file for this content pack could not be found: {pack.Manifest.Name}");
                Warn("If you are the author, please ensure the file named content.json in the pack's main folder.");
                Warn("If you are its user, please reinstall the content pack or report the issue to the author.");
                continue; // skip to the next content pack
            }

            var order = new List<string>();
            foreach (var (key, hatInfo) in data)
            {
                // set some required fields if they are not set
                hatInfo.Pack = pack;
                hatInfo.ID = pack.Manifest.UniqueID;
                if (string.IsNullOrWhiteSpace(hatInfo.UniqueBuffID))
                    hatInfo.UniqueBuffID = pack.Manifest.UniqueID;
                if (string.IsNullOrWhiteSpace(hatInfo.Description))
                    hatInfo.Description = null;

                // add the data to the dictionary
                allHatData.TryAdd(key, hatInfo);
                order.Add(key);
            }

            allOrder.Add(pack.Manifest.UniqueID, order);
            Log($"Successfully loaded content pack - {pack.Manifest.UniqueID}.");
        }

        AllHatData = allHatData;
        Order = allOrder;
    }

    /// <summary>
    /// Initialize the hat data from content packs when the mod is loaded.
    /// </summary>
    public static void ReloadContentPacks(string id)
    {
        Info($"Reloading for content packs {id} ...");

        foreach (var pack in ModEntry.ContentPacks)
        {
            if (pack.Manifest.UniqueID != id)
                continue;

            Log($"Found content pack {id}, preparing to reload ...");
            foreach (var (_, oldData) in AllHatData)
                oldData.RemoveAll(d => d.ID == id); // delete old data

            Dictionary<string, HatData> data;
            try
            {
                // load the content pack's farm config (null if it doesn't exist)
                data = pack.ReadJsonFile<Dictionary<string, HatData>>("content.json");
                if (data == null)
                    throw new Exception("Null data returned.");
            }
            catch (Exception ex)
            {
                Error($"Error: Failed to reload content pack {pack.Manifest.Name}!");
                Warn("The auto-generated error message is displayed below:");
                Warn($"{ex.Message}");
                Warn("--------------------");
                return;
            }

            var order = new List<string>();
            foreach (var (key, newData) in data)
            {
                // set some required fields if they are not set
                newData.Pack = pack;
                newData.ID = pack.Manifest.UniqueID;
                if (string.IsNullOrWhiteSpace(newData.UniqueBuffID))
                    newData.UniqueBuffID = pack.Manifest.UniqueID;
                if (string.IsNullOrWhiteSpace(newData.Description))
                    newData.Description = null;

                // add the data to the dictionary
                AllHatData.TryAdd(key, newData);
                order.Add(key);
            }

            Order[id] = order;
            goto Success;
        }

        Error($"Error: Content pack {id} not found!");
        return;

        Success:
        foreach (var key in AllHatData.Keys)
            if (!AllHatData[key].Any())
                AllHatData.Remove(key);

        Info($"Successfully reloaded content pack {id}.");
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
    public float GetValueByIndex(int index) => index switch
    {
        0 => FarmingLevel,
        1 => FishingLevel,
        2 => ForagingLevel,
        3 => MiningLevel,
        4 => CombatLevel,
        5 => LuckLevel,
        6 => Speed,
        7 => Defense,
        8 => Immunity,
        9 => MaxStamina,
        10 => MagneticRadius,
        11 => Attack,
        12 => AttackMultiplier,
        13 => CriticalChanceMultiplier,
        14 => CriticalPowerMultiplier,
        15 => WeaponSpeedMultiplier,
        16 => WeaponPrecisionMultiplier,
        17 => KnockbackMultiplier,
        _ => throw new IndexOutOfRangeException()
    };

    public string GetTranslationByIndex(int index)
    {
        var value = GetValueByIndex(index);
        var sign = value >= 0 ? "+" : "";
        var text = sign + (index >= 12 ? value.FormatAndTrim("P0") : value.FormatAndTrim());

        return index switch
        {
            0 => I18n.String_FarmingLevel(text),
            1 => I18n.String_FishingLevel(text),
            2 => I18n.String_ForagingLevel(text),
            3 => I18n.String_MiningLevel(text),
            4 => I18n.String_CombatLevel(text),
            5 => I18n.String_LuckLevel(text),
            6 => I18n.String_Speed(text),
            7 => I18n.String_Defense(text),
            8 => I18n.String_Immunity(text),
            9 => I18n.String_MaxStamina(text),
            10 => I18n.String_MagneticRadius(text),
            11 => I18n.String_Attack(text),
            12 => I18n.String_AttackMultiplier(text),
            13 => I18n.String_CriticalChanceMultiplier(text),
            14 => I18n.String_CriticalPowerMultiplier(text),
            15 => I18n.String_WeaponSpeedMultiplier(text),
            16 => I18n.String_WeaponPrecisionMultiplier(text),
            17 => I18n.String_KnockbackMultiplier(text),
            _ => throw new IndexOutOfRangeException()
        };
    }

    public static HatData Combine(IEnumerable<HatData> dataList)
    {
        var data = new HatData { ID = CombinedDataSign };
        foreach (var d in dataList)
        {
            data.FarmingLevel += d.FarmingLevel;
            data.FishingLevel += d.FishingLevel;
            data.ForagingLevel += d.ForagingLevel;
            data.MiningLevel += d.MiningLevel;
            data.CombatLevel += d.CombatLevel;
            data.LuckLevel += d.LuckLevel;
            data.Speed += d.Speed;
            data.Defense += d.Defense;
            data.Immunity += d.Immunity;
            data.MaxStamina += d.MaxStamina;
            data.MagneticRadius += d.MagneticRadius;
            data.Attack += d.Attack;
            data.AttackMultiplier += d.AttackMultiplier;
            data.CriticalChanceMultiplier += d.CriticalChanceMultiplier;
            data.CriticalPowerMultiplier += d.CriticalPowerMultiplier;
            data.WeaponSpeedMultiplier += d.WeaponSpeedMultiplier;
            data.WeaponPrecisionMultiplier += d.WeaponPrecisionMultiplier;
            data.KnockbackMultiplier += d.KnockbackMultiplier;
        }

        return data;
    }

    /// <summary>
    /// Convert this hat data to a buff.
    /// </summary>
    public Buff ConvertToBuff()
    {
        var buff = new Buff(UniqueBuffID);
        if (ID == CombinedDataSign)
        {
            buff.millisecondsDuration = 0;
            return buff;
        }

        var hatName = Game1.player.hat?.Value?.DisplayName ?? string.Empty;
        buff.displaySource = $"{hatName} ({Pack.Manifest.Name})";

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
        TryApplyModifier(buff);

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
    public bool TryCheckCondition()
    {
        // Check if the custom checker is null, if true, log a warning.
        if (UseCustomCondition)
        {
            // Check if custom cc is null, if true, log a warning and set the Trigger to None.
            if (CustomConditionChecker == null)
            {
                Warn($"{Pack.Manifest.Name} uses a custom condition checker, but it is not available.");
                UseCustomCondition = false;
                Trigger = Trigger.None;
                return true;
            }

            // not null, run it.
            try
            {
                return CustomConditionChecker.Invoke();
            }
            catch (Exception e)
            {
                Error($"An error occured while using custom condition checker for content pack {ID}!");
                Warn("See detailed information below:");
                Warn(e.Message);
                Warn(e.StackTrace);
                return false;
            }
        }

        if (Condition.EqualsIgnoreCase(CustomConditionSign))
        {
            Warn($"Content pack {ID} marks the condition as a custom one, but it haven't been set.");
            return false;
        }

        return GameStateQuery.CheckConditions(Condition);
    }

    /// <summary>
    /// Try to perform the action. If something goes wrong, log the error.
    /// </summary>
    public void TryPerformAction()
    {
        // Check if the custom action is null, if true, log a warning.
        if (UseCustomAction)
        {
            if (CustomAction == null)
            {
                Warn($"{Pack.Manifest.Name} uses a custom action, but it seems not available. ");
                UseCustomAction = false;
                return;
            }

            // not null, run it.
            try
            {
                CustomAction.Invoke();
            }
            catch (Exception e)
            {
                Error($"An error occured while performing custom action for content pack {ID}!");
                Warn("See detailed information below:");
                Warn(e.Message);
                Warn(e.StackTrace);
            }
        }

        if (Condition.EqualsIgnoreCase(CustomActionSign))
        {
            Warn($"Content pack {ID} marks the action as a custom one, but it haven't been set.");
            return;
        }

        TriggerActionManager.TryRunAction(Action, out var error, out var ex);
        if (ex == null) return;

        Error($"An error occured while performing action '{Action}' for content pack {ID}!");
        Warn("See detailed information below:");
        Warn(error);
        Warn(ex.Message);
        Warn(ex.StackTrace);
    }

    /// <summary>
    /// Try applying the modifier. If something goes wrong, log the error.
    /// </summary>
    public void TryApplyModifier(Buff buff)
    {
        if (CustomModifier is null) return;
        try
        {
            CustomModifier.Invoke(buff);
        }
        catch (Exception e)
        {
            Error($"An error occured while performing custom modifier for content pack {ID}!");
            Warn("See detailed information below:");
            Warn(e.Message);
            Warn(e.StackTrace);
        }
    }

    public string GetNoDescriptionWarning()
    {
        var hints = new List<string>();

        if (!string.IsNullOrWhiteSpace(Condition) && string.IsNullOrWhiteSpace(_conditionDescription))
            hints.Add(I18n.String_Condition());

        if (!string.IsNullOrWhiteSpace(Action) && string.IsNullOrWhiteSpace(_actionDescription))
            hints.Add(I18n.String_Action());

        if (CustomModifier != null && string.IsNullOrWhiteSpace(_modifierDescription))
            hints.Add(I18n.String_Modifier());

        return hints.Count > 0 ? string.Join(I18n.Punctuation_Comma(), hints).ToLower() : "";
    }
}