using System.Text;
using JetBrains.Annotations;
using Netcode;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using StardewValley.Quests;

namespace RoleRecovery;

[UsedImplicitly]
internal class ModEntry : Mod
{
    /****
     ** 属性
     ** Properties
     ****/

    #region Properties

    internal static IModHelper ModHelper { get; private set; }
    private static IMonitor ModMonitor { get; set; }
    public static void Log(string s, LogLevel l = LogLevel.Info) => ModMonitor.Log(s, l);

    private static List<Farmer> AllFarmers() => Game1.getAllFarmers().ToList();

    #endregion

    public override void Entry(IModHelper helper)
    {
        ModMonitor = Monitor;
        ModHelper = Helper;

        helper.ConsoleCommands.Add("get_all_player_name", "获取所有玩家列表", GetAllFarmers);
        helper.ConsoleCommands.Add("recover_role", "恢复指定玩家角色", RecoverRole);
        helper.ConsoleCommands.Add("get_items_from_player", "获取指定玩家的物品", GetItemFromOtherPlayer);
    }

    private static void GetAllFarmers(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log("未进入存档，无法获取玩家列表。", LogLevel.Warn);
            return;
        }

        var count = 0;
        var sb = new StringBuilder();
        var farmers = AllFarmers();
        foreach (var farmer in farmers)
        {
            sb.AppendLine($"{count}. {farmer.Name}");
            count++;
        }

        Log($"所有玩家列表如下：\n{sb}");
    }

    private static void RecoverRole(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log("未进入存档，无法进行恢复操作。", LogLevel.Warn);
            return;
        }

        if (Context.IsMultiplayer)
        {
            Log("无法在多人模式下恢复角色！", LogLevel.Warn);
            return;
        }

        try
        {
            var index = int.Parse(args[0]);
            var farmers = AllFarmers();
            if (farmers[index].UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                Log("自己恢复自己，你搁这儿卡 Bug 呢？", LogLevel.Warn);
                return;
            }

            RecoverRole(farmers[index]);
            Log($"成功恢复角色 {farmers[index].Name}。");
        }
        catch (Exception ex)
        {
            Log($"尝试恢复玩家时出现错误：{ex}", LogLevel.Error);
            Log(ex.StackTrace, LogLevel.Error);
        }
    }

    private static void GetItemFromOtherPlayer(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log("未进入存档，无法进行物品读取。", LogLevel.Warn);
            return;
        }

        try
        {
            var index = int.Parse(args[0]);
            var farmers = AllFarmers();
            if (farmers[index].UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                Log("自己拿自己的东西，你搁这儿卡 Bug 呢？", LogLevel.Warn);
                return;
            }

            if (Context.IsMultiplayer)
            {
                Log("你想干什么？！", LogLevel.Warn);
                Game1.Multiplayer.sendChatMessage(
                    LocalizedContentManager.CurrentLanguageCode,
                    $"{Game1.player.Name} 在试图使用存档恢复 Mod 获取 {farmers[index].Name} 的物品时被作者逮了个正着，这种行为是非常不道德的！",
                    Multiplayer.AllPlayers);
                return;
            }

            var inventory = farmers[index].Items;
            Game1.activeClickableMenu = new ItemGrabMenu(inventory);
            Log($"成功获取玩家 {farmers[index].Name} 的物品。");
        }
        catch (Exception ex)
        {
            Log($"尝试获取其他玩家物品时出现错误：{ex}", LogLevel.Error);
        }
    }

    private static void RecoverRole(Farmer oldRole)
    {
        var local = Game1.player;
        var hasRustyKey = local.hasRustyKey;
        var hasSkullKey = local.hasSkullKey;
        var canUnderstandDwarves = local.canUnderstandDwarves;

        local.SwapFarmHouse(oldRole);
        local.RecoverProperty<long>(oldRole, nameof(Farmer.UniqueMultiplayerID), true);
        local.RecoverProperty<string>(oldRole, nameof(Farmer.Name));
        local.RecoverField<NetObjectList<Quest>>(oldRole, nameof(Farmer.questLog));
        local.RecoverField<NetIntHashSet>(oldRole, nameof(Farmer.professions));
        local.RecoverField<NetArray<int, NetInt>>(oldRole, nameof(Farmer.experiencePoints));
        local.RecoverField<NetRef<Inventory>>(oldRole, nameof(Farmer.netItems), true);
        local.RecoverField<NetStringHashSet>(oldRole, nameof(Farmer.dialogueQuestionsAnswered));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.cookingRecipes));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.craftingRecipes));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.activeDialogueEvents));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.previousActiveDialogueEvents));
        local.RecoverField<NetStringHashSet>(oldRole, nameof(Farmer.triggerActionsRun));
        local.RecoverField<NetIntHashSet>(oldRole, nameof(Farmer.secretNotesSeen));
        local.RecoverField<HashSet<string>>(oldRole, nameof(Farmer.songsHeard));
        local.RecoverField<NetIntHashSet>(oldRole, nameof(Farmer.achievements));
        local.RecoverField<NetStringList>(oldRole, nameof(Farmer.specialItems));
        local.RecoverField<NetStringHashSet>(oldRole, nameof(Farmer.mailReceived));
        local.RecoverField<NetStringHashSet>(oldRole, nameof(Farmer.mailForTomorrow));
        local.RecoverField<NetStringList>(oldRole, nameof(Farmer.mailbox));
        local.RecoverField<NetStringHashSet>(oldRole, nameof(Farmer.locationsVisited));
        local.RecoverField<Stats>(oldRole, nameof(Farmer.stats));
        local.RecoverField<NetObjectList<Item>>(oldRole, nameof(Farmer.itemsLostLastDeath));
        local.RecoverField<NetString>(oldRole, nameof(Farmer.favoriteThing));
        local.RecoverProperty<bool>(oldRole, nameof(Farmer.hasClubCard));
        local.RecoverProperty<bool>(oldRole, nameof(Farmer.hasDarkTalisman));
        local.RecoverProperty<bool>(oldRole, nameof(Farmer.hasMagicInk));
        local.RecoverProperty<bool>(oldRole, nameof(Farmer.hasMagnifyingGlass));
        local.RecoverProperty<bool>(oldRole, nameof(Farmer.hasSpecialCharm));
        local.RecoverProperty<bool>(oldRole, nameof(Farmer.HasTownKey));
        local.RecoverField<NetString>(oldRole, nameof(Farmer.shirt));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.hair));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.skin));
        local.RecoverField<NetString>(oldRole, nameof(Farmer.shoes));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.accessory));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.facialHair));
        local.RecoverField<NetString>(oldRole, nameof(Farmer.pants));
        local.RecoverField<NetRef<WorldDate>>(oldRole, nameof(Farmer.lastGotPrizeFromGil));
        local.RecoverField<NetRef<WorldDate>>(oldRole, nameof(Farmer.lastDesertFestivalFishingQuest));
        local.RecoverField<NetColor>(oldRole, nameof(Farmer.hairstyleColor));
        local.RecoverField<NetColor>(oldRole, nameof(Farmer.pantsColor));
        local.RecoverField<NetColor>(oldRole, nameof(Farmer.newEyeColor));
        local.RecoverField<NetRef<Hat>>(oldRole, nameof(Farmer.hat));
        local.RecoverField<NetRef<Boots>>(oldRole, nameof(Farmer.boots));
        local.RecoverField<NetRef<Clothing>>(oldRole, nameof(Farmer.shirtItem));
        local.RecoverField<NetRef<Clothing>>(oldRole, nameof(Farmer.pantsItem));
        local.RecoverField<NetList<Trinket, NetRef<Trinket>>>(oldRole, nameof(Farmer.trinketItems));
        local.RecoverField<bool>(oldRole, nameof(Farmer.usingRandomizedBobber));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.farmingLevel));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.miningLevel));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.combatLevel));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.foragingLevel));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.fishingLevel));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.maxStamina));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.maxItems));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.lastSeenMovieWeek));
        local.RecoverField<int>(oldRole, nameof(Farmer.clubCoins));
        local.RecoverField<int>(oldRole, nameof(Farmer.trashCanLevel));
        local.RecoverField<NetRef<Tool>>(oldRole, nameof(Farmer.toolBeingUpgraded));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.daysLeftForToolUpgrade));
        local.RecoverField<NetInt>(oldRole, nameof(Farmer.daysUntilHouseUpgrade));
        local.RecoverField<int>(oldRole, nameof(Farmer.maxHealth));
        local.RecoverField<float>(oldRole, nameof(Farmer.difficultyModifier));
        local.RecoverProperty<Gender>(oldRole, nameof(Farmer.Gender));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.basicShipped));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.mineralsFound));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.recipesCooked));
        local.RecoverField<NetStringIntArrayDictionary>(oldRole, nameof(Farmer.fishCaught));
        local.RecoverField<NetStringIntArrayDictionary>(oldRole, nameof(Farmer.archaeologyFound));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.callsReceived));
        local.RecoverField<SerializableDictionary<string, SerializableDictionary<string, int>>>(oldRole,
            nameof(Farmer.giftedItems));
        local.RecoverField<NetStringDictionary<int, NetInt>>(oldRole, nameof(Farmer.tailoredItems));
        local.RecoverField<NetStringDictionary<Friendship, NetRef<Friendship>>>(oldRole, nameof(Farmer.friendshipData));
        local.RecoverProperty<int>(oldRole, nameof(Farmer.QiGems));
        local.RecoverField<NetIntDictionary<bool, NetBool>>(oldRole, nameof(Farmer.chestConsumedMineLevels));
        local.RecoverField<int>(oldRole, nameof(Farmer.saveTime));
        local.RecoverField<NetBool>(oldRole, nameof(Farmer.isCustomized));
        local.RecoverField<float>(oldRole, nameof(Farmer.movementMultiplier));
        local.RecoverProperty<int>(oldRole, nameof(Farmer.deepestMineLevel));
        local.RecoverField<int>(oldRole, nameof(Farmer.health));
        local.RecoverProperty<float>(oldRole, nameof(Farmer.Stamina));
        local.RecoverProperty<uint>(oldRole, nameof(Farmer.totalMoneyEarned));
        local.RecoverProperty<ulong>(oldRole, nameof(Farmer.millisecondsPlayed));
        local.RecoverProperty<int>(oldRole, nameof(Farmer.timesReachedMineBottom));
        local.RecoverProperty<string>(oldRole, nameof(Farmer.spouse), true);
        local.CombineEventsSeen(oldRole);

        local.hasRustyKey = hasRustyKey;
        local.hasSkullKey = hasSkullKey;
        local.canUnderstandDwarves = canUnderstandDwarves;
    }
}

public static class Utils
{
    public static void RecoverField<T>(this Farmer local, Farmer target, string which, bool swap = false)
    {
        try
        {
            var lrf = ModEntry.ModHelper.Reflection.GetField<T>(local, which);
            var trf = ModEntry.ModHelper.Reflection.GetField<T>(target, which);

            if (swap)
            {
                var cache = lrf.GetValue();
                lrf.SetValue(trf.GetValue());
                trf.SetValue(cache);
            }
            else
            {
                lrf.SetValue(trf.GetValue());
            }
        }
        catch (Exception ex)
        {
            ModEntry.Log($"交换数据时出现错误，未能交换数据 {which}: {ex}", LogLevel.Error);
        }
    }

    public static void RecoverProperty<T>(this Farmer local, Farmer target, string which, bool swap = false)
    {
        try
        {
            var lrp = ModEntry.ModHelper.Reflection.GetProperty<T>(local, which);
            var trp = ModEntry.ModHelper.Reflection.GetProperty<T>(target, which);

            if (swap)
            {
                var cache = lrp.GetValue();
                lrp.SetValue(trp.GetValue());
                trp.SetValue(cache);
            }
            else
            {
                lrp.SetValue(trp.GetValue());
            }
        }
        catch (Exception ex)
        {
            ModEntry.Log($"交换数据时出现错误，未能交换数据 {which}: {ex}", LogLevel.Error);
        }
    }

    public static void SwapFarmHouse(this Farmer local, Farmer target)
    {
        var farmHouse = Utility.getHomeOfFarmer(local);
        var currentUpgradeLevel = local.HouseUpgradeLevel;
        var targetUpgradeLevel = target.HouseUpgradeLevel;
        for (; currentUpgradeLevel < targetUpgradeLevel; currentUpgradeLevel++)
        {
            farmHouse.moveObjectsForHouseUpgrade(local.HouseUpgradeLevel + 1);
            local.HouseUpgradeLevel++;
            farmHouse.setMapForUpgradeLevel(local.HouseUpgradeLevel);
        }

        local.RecoverField<NetString>(target, nameof(Farmer.homeLocation));
        local.RecoverField<NetString>(target, nameof(Farmer.lastSleepLocation));
        local.RecoverField<NetPoint>(target, nameof(Farmer.lastSleepPoint));
    }

    public static void CombineEventsSeen(this Farmer local, Farmer target)
    {
        var cache = local.eventsSeen.ToList();

        local.RecoverField<NetStringHashSet>(target, nameof(Farmer.eventsSeen));
        foreach (var e in cache) local.eventsSeen.Add(e);
    }
}