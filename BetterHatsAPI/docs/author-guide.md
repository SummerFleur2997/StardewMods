# 开发者文档

**🌐 In other languages: [English](./author-guide_en.md)**

**Better Hats API**（BHA）是一个为 Stardew Valley 模组开发者提供的工具，使用本模组可以向帽子添加自定义 buff 效果、条件逻辑和特殊行为。当玩 家装备帽子时，BHA 可以根据游戏状态、时间、地点或高级自定义逻辑来应用 buff、检查条件并触发操作。

这个模组的功能几乎全部源于我在开发自己模组的过程中自己的需求——我希望开发一个 Mod，给每一顶帽子加入一个特殊的效果。但在我浏览了社区内现有的功能类似的 Mod 后，发现他们多使用硬编码来实现目的。我认为与其将每个帽子的行为硬编码，不如开发一个更加灵活的 API，让其他模组制作者可以用 JSON 来创建自己具有独特效果的帽子。于是便有了 Better Hats API 这个 Mod。

欢迎使用它来开发您自己的模组，如果您有任何建议或问题，欢迎提出，我会尽力改进。

通过 BHA，您可以创建能够实现以下功能的帽子：

- 应用基础的属性加成（如耕种等级、攻击加成等）
- 仅在特定条件下生效的加成（如特定的季节、地点、天气、时间）
- 当条件满足时触发自定义操作
- 根据当前游戏上下文动态调整 buff 数值
- 通过自定义 C# 代码与游戏机制进行深度集成

核心设计原则是灵活性：**简单效果通过 JSON 即可实现**，而**复杂行为可以通过 API 集成 C# 代码实现**。

这个 API 本身不包含任何 Harmony 补丁，因此在理论上不存在与其他模组冲突的风险。不过，您可以将 BHA 与您自己的 Harmony 补丁结合使用，以实现更强大效果。

---

**目录**
- [使用 JSON 配置基础 Buff](#使用-json-配置基础-buff)
    - [`content.json` 的基本结构](#contentjson-的基本结构)
    - [可用的 Buff 属性](#可用的-buff-属性)
    - [为条件和事件选择合适的触发器](#为条件和事件选择合适的触发器)
    - [条件系统](#条件系统)
    - [事件系统](#事件系统)
    - [综合条件和事件的进阶效果](#综合条件和事件的进阶效果)
    - [动态效果标志](#动态效果标志)
    - [自定义条件事件及其占位符](#自定义条件事件及其占位符)
- [使用 C# API 实现复杂功能](#使用-c-api-实现复杂功能)
    - [获取 API](#获取-api)
    - [设置自定义条件检查器](#设置自定义条件检查器)
    - [设置自定义操作触发器](#设置自定义操作触发器)
    - [设置自定义 Buff 修饰器](#设置自定义-buff-修饰器)
    - [OnHatEquipped 和 OnHatUnequipped 事件](#onhatequipped-和-onhatunequipped-事件)
    - [完整集成示例](#完整集成示例)
- [高级：使用 Harmony 补丁和帽子检测](#高级使用-harmony-补丁和帽子检测)
- [更多示例参考](#更多示例参考)

---

## 使用 JSON 配置基础 Buff

要为 BHA 创建数据包（content pack），您需要：

1. 在 `Stardew Valley\Mods` 文件夹中创建一个新文件夹，文件夹名称应遵循以下格式：`[BHA] 您的数据包名称`
2. 在新文件夹中创建一个名为 `manifest.json` 的文本文件。（更多信息请参见 [Wiki 中关于 Manifest 的介绍](https://wiki.biligame.com/stardewvalley/%E6%A8%A1%E7%BB%84:%E5%88%B6%E4%BD%9C%E6%8C%87%E5%8D%97/APIs/Manifest)）复制以下格式并按照自己的需求修改相关项：
    ```json
    {
      "Name": "您的数据包名称",
      "Author": "您的名字",
      "Version": "1.0.0",
      "Description": "在这里填写描述。简要说明数据包的功能。",
      "UniqueID": "YourName.YourPackName", // 必须是英文
      "MinimumApiVersion": "4.0.0",
      "UpdateKeys": [],
      "ContentPackFor": {
        "UniqueID": "SummerFleur.BetterHatsAPI",
        "MinimumVersion": "1.0.0"
      }
    }
    ```
3. 在您的数据包文件夹中创建一个名为 `content.json` 的文本文件。

### `content.json` 的基本结构

`content.json` 文件的基本映射是 `帽子 ID -> 具体属性`。BHA 使用每个帽子带类型标识的完整 ID（Qualified Item ID）作为键：

```json
{
  "(H)0": {
    "Speed": 1.5,
    "Description": "戴上这顶牛仔帽能跑得更快。"
  },
  "(H)70": {
    "Immunity": 2,
    "Description": "这顶帽子注入了来自女巫的魔法，能够提供额外的免疫。"
  }
}
```

**注意：**
- 必须使用帽子的 Qualified Item ID 作为键（以 `(H)` 开头）
- 所有属性都是可选的，对于数字默认为 0，对于文本默认为空字符串
- 如果未指定，`UniqueBuffID` 默认为您数据包的 ID（参见下文）
- 通过 `i18n:your.key` 格式支持本地化

### 可用的 Buff 属性

BHA 支持所有标准的 Stardew Valley buff 属性，这些属性直接用于给定的帽子。

| 属性                          | 描述        | 类型     |
|-----------------------------|-----------|--------|
| `CombatLevel`               | 战斗技能加成    | number |
| `FarmingLevel`              | 耕种技能加成    | number |
| `FishingLevel`              | 钓鱼技能加成    | number |
| `MiningLevel`               | 采矿技能加成    | number |
| `LuckLevel`                 | 运气技能加成    | number |
| `ForagingLevel`             | 采集技能加成    | number |
| `Speed`                     | 移动速度加成    | number |
| `Defense`                   | 防御加成      | number |
| `Immunity`                  | 免疫加成      | number |
| `MaxStamina`                | 最大体力加成    | number |
| `MagneticRadius`            | 磁力半径加成    | number |
| `Attack`                    | 攻击加成      | number |
| `AttackMultiplier`          | 攻击倍率加成    | number |
| `CriticalChanceMultiplier`  | 暴击率倍率加成   | number |
| `CriticalPowerMultiplier`   | 暴击力量倍率加成  | number |
| `WeaponSpeedMultiplier`     | 武器速度倍率加成  | number |
| `WeaponPrecisionMultiplier` | 武器精确度倍率加成 | number |
| `KnockbackMultiplier`       | 击退倍率加成    | number |

**示例**：带来强大战斗属性的帽子
```json
{
  "(H)64": {
    "Description": "传说中的神秘头巾，可以带来强大的力量。",
    "Attack": 2,
    "Defense": 2,
    "Immunity": 2,
    "Speed": 1,
    "MaxStamina": 50,
    "MagneticRadius": 64
  }
}
```

### 为条件和事件选择合适的触发器

若您希望引入高级的条件和事件，您或许需要设定一个触发器来确定何时检查条件或触发事件，BHA 提供了六种不同频率的条件检查器，可以使用 `Trigger` 字段来进行设置。

| Trigger           | 检查时机            |
|-------------------|-----------------|
| `None`            | 仅在装备帽子时         |
| `DayStarted`      | 每天早上 6:00       |
| `LocationChanged` | 当玩家传送到新地点时      |
| `TimeChanged`     | 当游戏内时钟前进时       |
| `SecondUpdated`   | 每秒一次            |
| `TickUpdated`     | 每个游戏刻（约每秒 60 次） |

`TickUpdated` 是一个高开销事件。在使用 `TickUpdated` 之前，请务必三思，同时考虑使用 `SecondUpdated` 是否能满足您的需求。BHA 在 GMCM 中为玩家提供了一个选项以禁用它，此时它会被转换为 `SecondUpdated`。建议选择最适合您条件的触发器，同时尽量减少性能影响。

**提示**：若必须使用 `TickUpdated`，请尽量考虑使用 C# 代码配合 API 来设置自定义条件或事件，相比于在 JSON 中设置字符串并让游戏进行解析，C# 代码能够避免这种不必要的开销。

### 条件系统

`Condition`（条件）字段使用 Stardew Valley 的 **GameStateQuery** 系统来确定何时应用 buff，以及是否应当触发事件。

以下是一些常见的 GameStateQuery 条件：

**基于季节的条件**：一顶仅在冬季提供 buff 的帽子。

```json
{
  "(H)11": {
    "Speed": 0.5,
    "LuckLevel": 1,
    "Condition": "LOCATION_SEASON Here Winter",
    "Trigger": "LocationChanged",
    "ConditionDescription": "仅在冬季生效"
  }
}
```

**基于地点的条件**：一顶仅在特定地点提供 buff 的帽子。

```json
{
  "(H)56": {
    "FishingLevel": 2,
    "Condition": "LOCATION_NAME Here Submarine",
    "Trigger": "LocationChanged"
  }
}
```

**基于日期的条件**：一顶仅在春季特定日期提供 buff 的帽子。

```json
{
  "(H)14": {
    "ForagingLevel": 1,
    "Condition": "LOCATION_SEASON Here Spring, SEASON_DAY Spring 15 Spring 16 Spring 17 Spring 18",
    "Trigger": "DayStarted",
    "Description": "在美洲大树莓成熟季获得额外采集增益。",
    "ConditionDescription": "仅春季 15-18 日"
  }
}
```

**基于天气的条件**：一顶仅在雨天应用 buff 的帽子。

```json
{
  "(H)28": {
    "FishingLevel": 1,
    "Condition": "LOCATION_IS_OUTDOORS Here, WEATHER Here Rain Storm GreenRain",
    "Trigger": "LocationChanged",
    "ConditionDescription": "当前天气为雨天"
  }
}
```

更多 GameStateQuery 模式请参阅 SummerFleur's Better Hats 项目中的 [`content.json`](../../SummerFleursBetterHats/content%20for%20BHA/content.json)。

### 事件系统

`Action` 字段使用 Stardew Valley 的 **TriggerActionManager**，在条件满足时执行某些操作。

**示例**：一顶在每天早上醒来时赠送 20 金币的帽子。

```json
{
  "(H)18": {
    "Action": "AddMoney 20",
    "Trigger": "DayStarted"
  }
}
```

**常见 TriggerAction 命令：**
- `AddMoney <amount>` - 给予玩家金币
- `AddItem <item_id> <amount>` - 添加物品
- `AddConversationTopic <topic_id>` - 添加与 NPC 的对话话题

### 综合条件和事件的进阶效果

您可以将条件与事件结合起来，实现复杂的效果：

```json
{
  "(H)25": {
    "Condition": "SEASON_DAY Winter 25",
    "Action": "AddItem (O)MysteryBox",
    "Trigger": "DayStarted",
    "Description": "在冬日星盛宴时获得圣诞老人的礼物",
    "ConditionDescription": "仅冬季 25 日",
    "ActionDescription": "获得一个谜之盒"
  }
}
```

### 动态效果标志

`Dynamic` 标志可用于配合带有动态效果的帽子，尤其是当使用 API 设置了自定义修饰器时。

```json
{
  "(H)45": {
    "FishingLevel": 1,
    "LuckLevel": 1,
    "Condition": "SFBH_MINE_LEVEL Here 20 60 100",
    "Trigger": "LocationChanged",
    "Dynamic": true,
    "ConditionDescription": "位于矿井的 20、60 或 100 层",
    "ModifierDescription": "位于 60 层获得 2 倍的增益效果，100 层获得 3 倍的增益效果。"
  }
}
```

默认情况下，BHA 仅在条件首次满足时应用一次帽子的 buff。 当 buff 已经激活，且触发器再次触发时，即使条件仍然满足，BHA 也不会重新应用 buff。这导致具有动态数值的 buff 效果可能无法及时更新。当 `Dynamic: true` 时，BHA 会在触发器每次触发时重新应用 buff，即使 buff 已经激活。这确保了通过 API 设置的自定义修饰器能够动态地更新 buff 数值。

### 自定义条件、事件及其占位符

要与 C# 模组进行高级集成，请使用特殊占位符：

```json
{
  "(H)2": {
    "Condition": "$CustomCondition",
    "Action": "$CustomAction",
    "Trigger": "DayStarted",
    "ConditionDescription": "特殊婚礼效果",
    "ActionDescription": "增加友谊加成"
  }
}
```

这些诸如 `$CustomCondition` 的标记告诉 BHA，外部的 C# 模组将通过 API 提供实际方法。这些占位符并不是必须的，您仍然可以在不使用它们的情况下使用 API 为帽子注册自定义逻辑。但是，使用它们可以提高可读性，并可能有助于避免不必要的错误，因为 BHA 会检查这些占位符，并在您忘记设置自己的方法时抛出警告。

### 使用本地化（i18n）

BHA 支持对您数据包中的所有文本字段进行本地化。这使得您的帽子描述、条件描述、操作描述和修饰器描述都可以根据玩家的游戏语言设置以不同语言显示。以下文本字段支持 i18n：

| 字段                     | 描述          |
|------------------------|-------------|
| `Description`          | 帽子效果的主要描述   |
| `ConditionDescription` | 描述条件何时满足    |
| `ActionDescription`    | 描述操作的作用     |
| `ModifierDescription`  | 描述自定义修饰器的效果 |

要启用 i18n，您应该在数据包目录中创建一个 i18n 文件夹：

```
[BHA] 您的数据包名称/
├── manifest.json
├── content.json
└── i18n/
    ├── default.json    (英语 - 默认)
    └── zh.json         (中文，示例)
```

default.json 文件作为回退语言（通常是英语）。如果找不到玩家语言的翻译，将使用 default.json 中的内容。每个语言文件是一个将翻译键映射到本地化字符串的 JSON 对象。

```json
{
  "description.YourHat": "在此语言中描述您的帽子。",
  "condition.YourHat": "描述条件何时激活。",
  "action.YourHat": "描述操作的作用。",
  "modifier.YourHat": "描述修饰器如何工作。"
}
```

然后您即可使用 `i18n:` 前缀来引用翻译。您也可以混合使用本地化和非本地化文本。任何不以 `i18n:` 为前缀的文本都将原样显示。

```json
{
  "(H)YourCustomHat": {
    "Description": "i18n:description.YourHat",
    "ConditionDescription": "i18n:condition.YourHat",
    "ActionDescription": "i18n:action.YourHat",
    "ModifierDescription": "i18n:modifier.YourHat"
  }
}
```

遗憾的是，BHA 暂时不支持在 i18n 中使用令牌（token）。因此，如果您需要在描述中使用令牌，您需要手动翻译它们。

---

## 使用 C# API 实现复杂功能

当 JSON 无法实现某些更高级的功能时，您可以使用 BHA 提供的 API 与 C# 进行深度集成。相关的接口为 [`ISummerFleurBetterHatsAPI`](../../BetterHatsAPI/API/ISummerFleurBetterHatsAPI.cs)。

### 获取 API

要获取 API，您首先要将 `ISummerFleurBetterHatsAPI.cs` 文件复制到您的项目中，按照您的需求修改命名空间或删除不需要的方法，然后在您模组中访问 API：

```csharp
var api = ModHelper.ModRegistry.GetApi<ISummerFleurBetterHatsAPI>("BetterHatsAPI");
if (api == null)
{
    Monitor.Log("Better Hats API 未找到！", LogLevel.Error);
    return;
}
```

### 设置自定义条件检查器

使用自定义的 C# 逻辑替换基础的 GameStateQuery：

```csharp
// 注册自定义条件检查器
api.SetCustomConditionChecker(
    qualifiedHatID: "(H)SpaceHelmet", // 告诉 API 这适用于哪个帽子
    packID: "YourMod.ContentPackID",  // 您的数据包的唯一 ID
    customConditionChecker: MyCondition_SpaceHelmet_InDifficultyMine
);

private bool MyCondition_SpaceHelmet_InDifficultyMine()
{
    if (Game1.currentLocation is not MineShaft mineShaft)
        return false;

    return mineShaft.GetAdditionalDifficulty() > 0;
}
```

取自**实际示例** —— [`SpaceHelmet.cs`](../../SummerFleursBetterHats/HatWithEffects/SpaceHelmet.cs)

### 设置自定义操作触发器

在条件满足时执行复杂代码：

```csharp
api.SetCustomActionTrigger(
    qualifiedHatID: "(H)40",  // 活的帽子
    packID: "YourMod.ContentPackID",
    customActionTrigger: MyAction_LivingHat_RegenerateHealth
);

private void MyAction_LivingHat_RegenerateHealth()
{
    if (!Game1.shouldTimePass())
        return;

    var player = Game1.player;
    player.health = Math.Min(player.maxHealth, player.health + 1);
    player.Stamina += 2;
}
```

取自**实际示例** —— [`LivingHat.cs`](../../SummerFleursBetterHats/HatWithEffects/LivingHat.cs)。

### 设置自定义 Buff 修饰器

根据游戏上下文动态修改 buff 数值：

```csharp
api.SetCustomBuffModifier(
    qualifiedHatID: "(H)SpaceHelmet",
    packID: "YourMod.ContentPackID",
    customModifier: MyBuffModifier_SpaceHelmet_ScaleWithMineDifficulty);

private void MyBuffModifier_SpaceHelmet_ScaleWithMineDifficulty(Buff buff)
{
    if (Game1.currentLocation is not MineShaft mineShaft)
        return;
    
    var multiplier = mineShaft.GetAdditionalDifficulty();
    buff.effects.AttackMultiplier.Value *= multiplier;
    buff.effects.Defense.Value *= multiplier;
    buff.effects.Speed.Value *= multiplier;
}
```

取自**实际示例** —— [`SpaceHelmet.cs`](../../SummerFleursBetterHats/HatWithEffects/SpaceHelmet.cs)。

自定义修饰器能够接收 `Buff` 实例，这使您完整拥有了在 buff 应用于玩家之前调整其任何属性的控制权。这就是 `Dynamic: true` 标志存在的意义。它强制 BHA 在触发器每次触发时重新应用（并因此重新修饰效果的值）buff。因此，您在自己的方法中对 buff 所做的更改（尤其是当您增强特定属性的值时）将立即生效。

### OnHatEquipped 和 OnHatUnequipped 事件

订阅帽子变化事件以进行特殊的初始化或清理：

```csharp
api.OnHatEquipped += (sender, e) => {
    var hatId = e.NewHat.QualifiedItemId;
    Monitor.Log($"玩家装备了：{hatId}");
    
    // 注册特定于这个帽子的游戏事件
    if (hatId == "(H)BlueRibbon")
        ModHelper.Events.Player.Warped += OnBlueRibbonWarped;
    
    if (!e.InvokedWhenSaveLoaded)
        // 玩家手动装备时的一次性设置
        DoInitialSetup();
};

api.OnHatUnequipped += (sender, e) => {
    var hatId = e.OldHat.QualifiedItemId;
    Monitor.Log($"玩家卸下了：{hatId}");
    
    // 清理事件
    ModHelper.Events.Player.Warped -= OnBlueRibbonWarped;
};
```

取自**实际示例** —— [`EventRegister.cs`](../../SummerFleursBetterHats/HatRelyOnEvents/%23EventRegister.cs)。

### 完整集成示例

这是一个完整的示例，展示如何将您的 C# 模组与 BHA 的 JSON 数据包深度集成：

**步骤 1：为您的帽子创建 JSON 数据包**
```json
// 您的数据包中的 content.json
{
  "(H)YourCustomHat": {
    "Condition": "$CustomCondition",
    "Action": "$CustomAction",
    "Trigger": "SecondUpdated",
    "Dynamic": true,
    "ConditionDescription": "在危险区域",
    "ActionDescription": "缓慢恢复生命值",
    "ModifierDescription": "随危险等级提升"
  }
}
```

**步骤 2：在您的 C# 模组中注册 API 逻辑**
```csharp
public class ModEntry : Mod
{
    private ISummerFleurBetterHatsAPI? _bhaApi;
    private const string packId = "YourMod.YourContentPackID";
    
    public override void Entry(IModHelper helper)
    {
        _bhaApi = Helper.ModRegistry.GetApi<ISummerFleurBetterHatsAPI>("BetterHatsAPI");
        if (_bhaApi == null) return;

        var hatId = "(H)YourCustomHat";
        
        // 注册自定义条件
        _bhaApi.SetCustomConditionChecker(hatId, packId, IsPlayerInDangerZone);
        
        // 注册自定义操作
        _bhaApi.SetCustomActionTrigger(hatId, packId, RegenerateHealth);
        
        // 注册自定义修饰器
        _bhaApi.SetCustomBuffModifier(hatId, packId, ScaleBuffWithDangerLevel);
        
        // 如需要则订阅事件
        _bhaApi.OnHatEquipped += OnHatEquipped;
        _bhaApi.OnHatUnequipped += OnHatUnequipped;
    }
    
    private bool IsPlayerInDangerZone()
    {
        // 您的自定义逻辑
        return Game1.currentLocation is MineShaft mine && mine.getMineArea() >= 80;
    }
    
    private void RegenerateHealth()
    {
        var player = Game1.player;
        if (player.health < player.maxHealth)
            player.health++;
    }
    
    private void ScaleBuffWithDangerLevel(Buff buff)
    {
        if (Game1.currentLocation is MineShaft mine)
        {
            var danger = mine.getMineArea() / 100.0;
            buff.effects.Defense.Value *= (1 + danger);
        }
    }
}
```

---

## 高级：使用 Harmony 补丁和帽子检测

要进行更深入的集成，您可以结合 Harmony 补丁。在补丁方法中检查玩家是否佩戴特定帽子：

```csharp
// 来自 HatsForCrops.cs
public static void AddCropCount(Crop crop, int xTile, int yTile, ref int numToHarvest)
{
    var r = Utility.CreateRandom(xTile * 7.0, yTile * 5.0, 
        Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame);
    if (r.NextDouble() > 0.1) return;
    
    // 检查玩家是否佩戴祝尼魔帽
    if (PlayerHatIs("(H)JunimoHat"))
        numToHarvest += 1;
}
```

Harmony 的具体内容已经超过了本文档的范畴，因此在此处不再赘述。若希望参考更多完整的 transpiler 示例请参阅 SummerFleursBetterHats 项目中的 [HatWithPatches](../../SummerFleursBetterHats/HatWithPatches) 目录。

BHA 并不能检测您的模组是否有 Harmony 补丁，但您可以使用 JSON 中的 `Description` 字段告知玩家您做了什么。这些提示文本将显示在游戏内的帽子图鉴菜单中，这使得玩家能够通过一种简单的方式来了解您的模组有什么功能。

---

## 更多示例参考

[`SummerFleursBetterHats`](../../SummerFleursBetterHats) 项目包含大量示例：

- **简单的 JSON 配置**：[`content for BHA/content.json`](../../SummerFleursBetterHats/content%20for%20BHA/content.json)
- **自定义条件/操作**：[`HatWithEffects`](../../SummerFleursBetterHats/HatWithEffects) 文件夹
- **事件集成**：[`HatRelyOnEvents`](../../SummerFleursBetterHats/HatRelyOnEvents) 文件夹
- **Harmony 补丁**：[`HatWithPatches`](../../SummerFleursBetterHats/HatWithPatches) 文件夹

您可以查看这些示例来了解 Better Hats API 集成的全部功能。

---

有关 API 详细信息，请参阅 [ISummerFleurBetterHatsAPI.cs](../API/ISummerFleurBetterHatsAPI.cs) 和 [HatData.cs](../Framework/HatData.cs)。