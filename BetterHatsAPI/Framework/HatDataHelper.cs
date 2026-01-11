using JetBrains.Annotations;
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
        Log("Loading content packs and local data files.");

        var allHatData = new Dictionary<string, List<HatData>>();

        /*****
         * Some logic below is copied from Esca-MMC's FTM mod, purported under the MIT license.
         ****/
        foreach (var pack in helper.ContentPacks.GetOwned())
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

            Log($"Successfully loaded content pack - {pack.Manifest.UniqueID}.");
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
        buff.displaySource = $"{hatName} ({Pack.Manifest.Name})";

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
                var id = Pack.Manifest.UniqueID;
                Error($"An error occured while using custom condition checker for content pack {id}!");
                Warn("See detailed information below:");
                Warn(e.Message);
                Warn(e.StackTrace);
                return false;
            }
        }

        return GameStateQuery.CheckConditions(Condition);
    }

    /// <summary>
    /// Try to perform the action. If something goes wrong, log the error.
    /// </summary>
    public void TryPerformAction()
    {
        if (string.IsNullOrWhiteSpace(Action)) return;

        var id = Pack.Manifest.UniqueID;
        // Check if custom action is null, if true, log a warning and.
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
                Error($"An error occured while performing custom action for content pack {id}!");
                Warn("See detailed information below:");
                Warn(e.Message);
                Warn(e.StackTrace);
            }
        }

        TriggerActionManager.TryRunAction(Action, out var error, out var ex);
        if (ex == null) return;

        Error($"An error occured while performing action '{Action}' for content pack {id}!");
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
            var id = Pack.Manifest.UniqueID;
            Error($"An error occured while performing custom multiplier for content pack {id}!");
            Warn("See detailed information below:");
            Warn(e.Message);
            Warn(e.StackTrace);
        }
    }
}