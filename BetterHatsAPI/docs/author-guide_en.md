# Author Guide

**🌐 In other languages: [简体中文](./author-guide.md)**

**Better Hats API** (BHA) is a tool for Stardew Valley mod developers that allows you to add custom
buff effects, conditional logic, and special behaviors to hats. When a player equips a hat, BHA can
apply buffs, check conditions, and trigger actions based on game state, time, location, or any
custom logic you implement. 

This mod's features are basically from my need to create hats with special effects in my own mods.
Instead of hardcoding each hat's behavior, I think it's a good idea to develop a flexible API that
other modders can use to create their own hats with unique effects. So I made Better Hats API to
fill this gap.

Please feel free to use it in your own mods, and advise me if you have any suggestions or issues,
I will try my best to improve it.

With BHA, you can create hats that:

- Apply standard stat bonuses (farming level, attack bonus, etc.)
- Only activate under specific conditions (season, location, weather, time)
- Trigger custom actions when conditions are met
- Scale buff values dynamically based on context
- Integrate deeply with game mechanics through custom C# code

The key design principle is flexibility: **simple effects work through JSON alone**, while 
**complex behaviors are achievable through API integration and C# codes**.

This API does not include any Harmony patches by itself, so there is no risk of conflicts with 
other mods theoretically. However, you can combine BHA with your own Harmony patches to create 
even more powerful hats.

---

**Content**
- [Using JSON for Basic Buff Configuration](#using-json-for-basic-buff-configuration)
  - [Basic Structure for `content.json`](#basic-structure-for-contentjson)
  - [Available Buff Properties](#available-buff-properties)
  - [Using Conditions with GameStateQuery](#condition-system)
  - [Using Actions with TriggerActionManager](#action-system)
  - [Combining Conditions and Actions](#combining-conditions-and-actions)
  - [The Dynamic Flag](#the-dynamic-flag)
  - [Custom Condition and Action Placeholders](#custom-condition-and-action-placeholders)
- [Using C# API for Complex Functionality](#using-c-api-for-complex-functionality)
  - [Getting the API](#getting-the-api)
  - [Setting Custom Condition Checkers](#setting-custom-condition-checkers)
  - [Setting Custom Action Triggers](#setting-custom-action-triggers)
  - [Setting Custom Buff Modifiers](#setting-custom-buff-modifiers)
  - [Using Events: OnHatEquipped and OnHatUnequipped](#using-events-onhatequipped-and-onhatunequipped)
  - [Complete Integration Example](#complete-integration-example)
- [Advanced: Harmony Patching with Hat Detection](#advanced-harmony-patching-with-hat-detection)
- [More Example References](#more-example-references)

---

## Using JSON for Basic Buff Configuration

To create a content pack for BHA, you should:

1. Create a new folder in the `Stardew Valley\Mods` folder. Its name should follow this format:
  `[BHA] Your Pack Name`
2. Create a text file in the new folder called `manifest.json`. (See the
  [Manifest](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) wiki article for
  more info.) Copy the format below and fill in the settings appropriately:
    ```json
    {
      "Name": "Your Pack Name",
      "Author": "Your Name",
      "Version": "1.0.0",
      "Description": "Your description here. Briefly explain what the content pack does.",
      "UniqueID": "YourName.YourPackName",
      "MinimumApiVersion": "4.0.0",
      "UpdateKeys": [],
      "ContentPackFor": {
        "UniqueID": "SummerFleur.BetterHatsAPI",
        "MinimumVersion": "1.0.0"
      }
    }
    ```
3. Create a text file in the new folder called `content.json` in your content pack folder.

### Basic Structure for `content.json`

Your JSON file maps hat IDs to configuration objects. Each hat's qualified ID serves as the key:

```json
{
  "(H)0": {
    "Speed": 1.5,
    "Description": "Move faster with this cowboy hat"
  },
  "(H)70": {
    "Immunity": 2,
    "Description": "Extra immunity from the witch's magic"
  }
}
```

**Key points:**
- You must use the hat's Qualified Item ID as the key (start with `(H)`)
- All properties are optional and default to 0 or empty string
- `UniqueBuffID` defaults to your content pack's ID if not specified (see below)
- Supports localization through `i18n:your.key` format

### Available Buff Properties

BHA supports all standard Stardew Valley buff properties, these properties are directly applied
to the given hat.

| Property                    | Description                       | Type   |
|-----------------------------|-----------------------------------|--------|
| `CombatLevel`               | Combat skill bonus                | number |
| `FarmingLevel`              | Farming skill bonus               | number |
| `FishingLevel`              | Fishing skill bonus               | number |
| `MiningLevel`               | Mining skill bonus                | number |
| `LuckLevel`                 | Luck skill bonus                  | number |
| `ForagingLevel`             | Foraging skill bonus              | number |
| `Speed`                     | Movement speed bonus              | number |
| `Defense`                   | Defense bonus                     | number |
| `Immunity`                  | Immunity bonus                    | number |
| `MaxStamina`                | Max stamina bonus                 | number |
| `MagneticRadius`            | Magnetic radius bonus             | number |
| `Attack`                    | Attack bonus                      | number |
| `AttackMultiplier`          | Attack multiplier bonus           | number |
| `CriticalChanceMultiplier`  | Critical chance multiplier bonus  | number |
| `CriticalPowerMultiplier`   | Critical power multiplier bonus   | number |
| `WeaponSpeedMultiplier`     | Weapon speed multiplier bonus     | number |
| `WeaponPrecisionMultiplier` | Weapon precision multiplier bonus | number |
| `KnockbackMultiplier`       | Knockback multiplier bonus        | number |

**Example**： A powerful combat hat
```json
{
  "(H)64": {
    "Description": "Legendary turban with immense power",
    "Attack": 2,
    "Defense": 2,
    "Immunity": 2,
    "Speed": 1,
    "MaxStamina": 50,
    "MagneticRadius": 64
  }
}
```

### Choose appropriate triggers for conditions and actions

If you want to introduce advanced conditions and actions, you might need to set a trigger to 
determine when to check the condition or trigger the action. BHA provides six different frequency
condition checkers, which can be set using the `Trigger` field.

| Trigger           | When Checked                        |
|-------------------|-------------------------------------|
| `None`            | Only when hat is equipped           |
| `DayStarted`      | Each morning at 6 AM                |
| `LocationChanged` | When player warps to a new location |
| `TimeChanged`     | When in-game clock advances         |
| `SecondUpdated`   | Once per second                     |
| `TickUpdated`     | Every game tick (~60 times/second)  |。

`TickUpdated` is a high-cost event. Before using `TickUpdated`, consider if `SecondUpdated`
suffices for your needs. BHA provides a toggle in the GMCM for players to disable it, in which case
it converts to `SecondUpdated`. It is recommended to choose a trigger that fits your condition best
while minimizing performance impact.

**Tips**: If you really need to use `TickUpdated`, consider using C# code with the API to set
custom conditions or actions, as opposed to setting strings in JSON and having the game parse them,
this method avoids unnecessary overhead.

### Condition System

The `Condition` field uses Stardew Valley's **GameStateQuery** system to determine when buffs
apply, and whether to trigger an event.

Below are some common GameStateQuery examples:

**Season-based conditions:** An example of a hat that only apply buffs in winter.
```json
{
  "(H)11": {
    "Speed": 0.5,
    "LuckLevel": 1,
    "Condition": "LOCATION_SEASON Here Winter",
    "Trigger": "LocationChanged",
    "ConditionDescription": "Only works in winter"
  }
}
```

**Location-based conditions:** An example of a hat that only apply buffs on specific location.
```json
{
  "(H)56": {
    "FishingLevel": 2,
    "Condition": "LOCATION_NAME Here Submarine",
    "Trigger": "LocationChanged"
  }
}
```

**Date-based conditions:** An example of a hat that only apply buffs during specific days in spring.
```json
{
  "(H)14": {
    "ForagingLevel": 1,
    "Condition": "LOCATION_SEASON Here Spring, SEASON_DAY Spring 15 Spring 16 Spring 17 Spring 18",
    "Trigger": "DayStarted",
    "Description": "Extra foraging during salmonberry season",
    "ConditionDescription": "Spring 15-18 only"
  }
}
```

**Weather-based conditions:** An example of a hat that only apply buffs during rainy weather.
```json
{
  "(H)28": {
    "FishingLevel": 1,
    "Condition": "LOCATION_IS_OUTDOORS Here, WEATHER Here Rain Storm GreenRain",
    "Trigger": "LocationChanged",
    "ConditionDescription": "During stormy weather"
  }
}
```

See [`content.json`](../../SummerFleursBetterHats/content%20for%20BHA/content.json) in the example
project for more GameStateQuery patterns.

### Action System

The `Action` field uses Stardew Valley's **TriggerActionManager** to execute commands when
conditions are met.

**Example**: Give daily money.
```json
{
  "(H)18": {
    "Action": "AddMoney 20",
    "Trigger": "DayStarted"
  }
}
```

**Common TriggerAction Commands:**
- `AddMoney <amount>` - Add money to player
- `AddItem <item_id> <amount>` - Add item to player inventory
- `AddConversationTopic <topic_id>` - Add conversation topic

### Combining Conditions and Actions

You can combine conditions with actions for sophisticated effects:

```json
{
  "(H)25": {
    "Condition": "SEASON_DAY Winter 25",
    "Action": "AddItem (O)MysteryBox",
    "Trigger": "DayStarted",
    "Description": "Get a mystery box on Winter Star",
    "ConditionDescription": "Winter 25 only",
    "ActionDescription": "Gifts a mystery box"
  }
}
```

### The Dynamic Flag

The `Dynamic` flag is crucial for hats with values that change based on conditions, especially when
using API-set custom modifiers.

```json
{
  "(H)45": {
    "FishingLevel": 1,
    "LuckLevel": 1,
    "Condition": "SFBH_MINE_LEVEL Here 20 60 100",
    "Trigger": "LocationChanged",
    "Dynamic": true,
    "ConditionDescription": "In specific mine levels",
    "ModifierDescription": "Scales with mine level"
  }
}
```

By default, BHA applies the hat's buff only once when the condition is first met. If the buff is
already active and the trigger fires again while the condition remains satisfied, BHA will never
re-apply the buff to the player. As a result, buff effects with dynamic values may not update in
a timely manner. When `Dynamic: true`, BHA will re-apply the buff whenever the trigger fires, even
if the buff is already active. This ensures that custom modifiers (set via API) update the buff
values dynamically.

### Custom Condition and Action Placeholders

For advanced integration with C# mods, use the special placeholders:

```json
{
  "(H)2": {
    "Condition": "$CustomCondition",
    "Action": "$CustomAction",
    "Trigger": "DayStarted",
    "ConditionDescription": "Special wedding effect",
    "ActionDescription": "Adds friendship bonus"
  }
}
```

These markers tell BHA that a C# mod will provide the actual logic through the API. It is actually
optional to use these placeholders; you can still register custom logic for hats without use them.
However, using them improves clarity and may help avoid mistakes because BHA will checks these
placeholders and throw warnings if you forget to register your custom logic.

### Using Localization (i18n)

BHA supports localization for all text fields in your content pack. This allows your hat
descriptions, condition descriptions, action descriptions, and modifier descriptions to be
displayed in different languages based on the player's game language setting. The following
text fields support i18n:

| Field                  | Description                                 |
|------------------------|---------------------------------------------|
| `Description`          | Main description of the hat effect          |
| `ConditionDescription` | Description of when the condition is met    |
| `ActionDescription`    | Description of what the action does         |
| `ModifierDescription`  | Description of the custom modifier's effect |

To enable i18n, you should create an i18n folder in your content pack directory:

```
[BHA] Your Pack Name/
├── manifest.json
├── content.json
└── i18n/
    ├── default.json    (English - default)
    └── zh.json         (Chinese for example)
```

The default.json file serves as the fallback language (typically English). If a translation
is not found for the player's language, the content from default.json will be used. Each 
language file is a JSON object mapping translation keys to localized strings.

```json
{
  "description.YourHat": "Description of your hat in this language.",
  "condition.YourHat": "Description of when the condition activates.",
  "action.YourHat": "Description of what the action does.",
  "modifier.YourHat": "Description of how the modifier works."
}
```

Then you can reference translations using the `i18n:` prefix. You can also mix localized and
non-localized text. Any text not prefixed with `i18n:` will be displayed as-is.

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

Unfortunately, BHA does not support i18n for tokens. So if you need to use tokens in your
descriptions, you will need to manually translate them.

---

## Using C# API for Complex Functionality

When JSON isn't enough, BHA's API allows deep integration through C#. The main interface is
[`ISummerFleurBetterHatsAPI`](../../BetterHatsAPI/API/ISummerFleurBetterHatsAPI.cs).

### Getting the API

To access the API, you should first copy `ISummerFleurBetterHatsAPI.cs` to your project, modify
the namespace or remove unnecessary methods as needed, and then access the API in your mod:

```csharp
var api = ModHelper.ModRegistry.GetApi<ISummerFleurBetterHatsAPI>("BetterHatsAPI");
if (api == null)
{
    Monitor.Log("Better Hats API not found!", LogLevel.Error);
    return;
}
```

### Setting Custom Condition Checkers

Replace the simple GameStateQuery with custom C# logic:

```csharp
// Register a custom condition checker
api.SetCustomConditionChecker(
    qualifiedHatID: "(H)SpaceHelmet", // tell the API which hat this applies to
    packID: "YourMod.ContentPackID",  // your content pack's unique ID
    customConditionChecker: MyCondition_SpaceHelmet_InDifficultyMine
);

private bool MyCondition_SpaceHelmet_InDifficultyMine()
{
    if (Game1.currentLocation is not MineShaft mineShaft)
        return false;

    return mineShaft.GetAdditionalDifficulty() > 0;
}
```

**Real Example** from [`SpaceHelmet.cs`](../../SummerFleursBetterHats/HatWithEffects/SpaceHelmet.cs)

### Setting Custom Action Triggers

Execute complex code when conditions are met:

```csharp
api.SetCustomActionTrigger(
    qualifiedHatID: "(H)40",  // Living Hat
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

**Real Example** from [`LivingHat.cs`](../../SummerFleursBetterHats/HatWithEffects/LivingHat.cs).

### Setting Custom Buff Modifiers

Dynamically modify buff values based on context:

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

**Real example** from [`SpaceHelmet.cs`](../../SummerFleursBetterHats/HatWithEffects/SpaceHelmet.cs).

The custom modifier function receives the active `Buff` instance, bringing you the full ability to
adjust any of its properties before it's applied to the player. This is where the `Dynamic: true`
flag becomes essential - it forces BHA to re-apply (and thus re-modify the value of effect) the
buff each time the trigger fires. So what you did to the buff inside this function (especially when
you buff the value on specific attributes) will take effect immediately.

### Using Events: OnHatEquipped and OnHatUnequipped

Subscribe to hat change events for special initialization/cleanup:

```csharp
api.OnHatEquipped += (sender, e) => {
    var hatId = e.NewHat.QualifiedItemId;
    Monitor.Log($"Player equipped: {hatId}");
    
    // Register game events specific to this hat
    if (hatId == "(H)BlueRibbon")
        ModHelper.Events.Player.Warped += OnBlueRibbonWarped;
    
    if (!e.InvokedWhenSaveLoaded)
        // One-time setup when player manually equips
        DoInitialSetup();
};

api.OnHatUnequipped += (sender, e) => {
    var hatId = e.OldHat.QualifiedItemId;
    Monitor.Log($"Player unequipped: {hatId}");
    
    // Clean up events
    ModHelper.Events.Player.Warped -= OnBlueRibbonWarped;
};
```

**Real example** from [`EventRegister.cs`](../../SummerFleursBetterHats/HatRelyOnEvents/%23EventRegister.cs).

### Complete Integration Example

Here's a complete example showing how to integrate a custom mod with BHA content pack:

**Step 1: Create JSON content for your hat**
```json
// content.json in your content pack
{
  "(H)YourCustomHat": {
    "Condition": "$CustomCondition",
    "Action": "$CustomAction",
    "Trigger": "SecondUpdated",
    "Dynamic": true,
    "ConditionDescription": "In danger zone",
    "ActionDescription": "Slowly regenerates health",
    "ModifierDescription": "Scales with danger level"
  }
}
```

**Step 2: Register API logic in your C# mod**
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
        
        // Register custom condition
        _bhaApi.SetCustomConditionChecker(hatId, packId, IsPlayerInDangerZone);
        
        // Register custom action
        _bhaApi.SetCustomActionTrigger(hatId, packId, RegenerateHealth);
        
        // Register custom modifier
        _bhaApi.SetCustomBuffModifier(hatId, packId, ScaleBuffWithDangerLevel);
        
        // Subscribe to events if needed
        _bhaApi.OnHatEquipped += OnHatEquipped;
        _bhaApi.OnHatUnequipped += OnHatUnequipped;
    }
    
    private bool IsPlayerInDangerZone()
    {
        // Your custom logic
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

## Advanced: Harmony Patching with Hat Detection

For even deeper integration, you can combine Harmony patching. Check if player is wearing a
specific hat inside patched methods:

```csharp
// From HatsForCrops.cs
public static void AddCropCount(Crop crop, int xTile, int yTile, ref int numToHarvest)
{
    var r = Utility.CreateRandom(xTile * 7.0, yTile * 5.0, 
        Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame);
    if (r.NextDouble() > 0.1) return;
    
    // Check if player is wearing Junimo Hat
    if (PlayerHatIs("(H)JunimoHat"))
        numToHarvest += 1;
}
```

Detailed contents of Harmony are beyond the scope of this document, so they are not covered here. 
For more complete transpiler examples, please refer to the
[HatWithPatches](../../SummerFleursBetterHats/HatWithPatches) 
folder in the SummerFleursBetterHats project.

BHA can't automatically detect any Harmony patches, but you can use the `Description` fields in
your JSON to inform players about special interactions. All that texts will appear in the in-game
guide book, bring an easy way for players to learn about your mod's features.

---

## More Example References

The [`SummerFleursBetterHats`](../../SummerFleursBetterHats) project contains extensive examples:

- **Simple JSON configs**: [`content for BHA/content.json`](../../SummerFleursBetterHats/content%20for%20BHA/content.json)
- **Custom conditions/actions**: [`HatWithEffects`](../../SummerFleursBetterHats/HatWithEffects) folder
- **Event integration**: [`HatRelyOnEvents`](../../SummerFleursBetterHats/HatRelyOnEvents) folder
- **Harmony patches**: [`HatWithPatches`](../../SummerFleursBetterHats/HatWithPatches) folder

You can see these examples to understand the full power of Better Hats API integration.

---

For API details, refer to [ISummerFleurBetterHatsAPI.cs](../API/ISummerFleurBetterHatsAPI.cs) and [HatData.cs](../Framework/HatData.cs).