# Convenient Chests 2

English | [简体中文](README_zh.md)

This is an unofficial reforge from [aEnigmatic's Convenient Chests](https://github.com/aEnigmatic/ConvenientChests),
mainly fixed some bugs and added a lot of new features.

## Main features

### 1. Chest Alias: Add alias to your chests

Facing a pile of chests, do you often forget what's inside? In the vanilla game, the common
solution is placing a sign behind the chest, which takes up extra space and can be easily edited
by accident while holding an item.

In version 2.0.0, this mod introduces the Chest Notes feature. You can add a note to each chest.
Simply hover your mouse over the chest to see the note, helping you quickly find the items you
need among a cluster of chests.

### 2. Chest Categories: Categorize chests by item type

This is a core feature carried over from the original mod. You can set item categories for chests
to specify what can be stored inside. By pressing the quick storage key or clicking the button
next to the chest, items matching the category will be be stored into chests.

### 3. Quick Storage: Store items into chests with one click

Stand near a chest and press the quick storage key to stack items defined by the "Category"
feature. You can also stack items into chests anywhere regardless of distance or map restrictions
(disabled by default), provided the chest has a set category or already contains similar items.

This mod also provide an auto-storage feature, this feature currently applies only when you are in
the Mines, Skull Cavern, and Volcano Dungeon. Items are automatically stored remotely every 30
minutes to prevent inventory overflow.

### 4. Item Locking: Prevent important items from being accidentally stored

Have you ever taken items out for a quest, only to accidentally hit the quick storage key and have
them silently return to the chest? Or perhaps you were deep in the Skull Caverns and accidentally
stored your vital supplies back home?

These situations can be solved with the Item Locking feature. Simply select and lock specific items
to prevent them from being automatically stored in chests.

### 5. Chest Snapshots: Save category configurations

Tired of re-setting your item categories every time you start a new save file?

In version 2.0.0, this mod introduces the Chest Snapshot feature. You can save your current
category settings as a snapshot and import it into a new save. This allows you to share the same
item categorization across different save files.

### 6. Quick Crafting: Craft items using materials from nearby chests

Stand near your chests and open the crafting or cooking menu to use materials directly from nearby
chests.

Note: If you also have the ‘Better Crafting’ mod installed, this feature may not work. However,
since ‘Better Crafting’ includes its own version of this functionality, it is recommended to use
that instead if both mods are present.

### 7. Compatibility Optimization: Improved compatibility with Convenient Inventory

In older versions, design inconsistencies in overlapping features between the two mods caused
issues when both were installed. I have submitted compatibility patches to the Convenient
Inventory mod. It is expected that its next version (estimated 1.6.1, depending on the author)
will be fully compatible with this mod, ensuring mutual compatibility.

## Update logs

### 2.0.0

**New Features**

- Added Chest Alias feature: View alias by hovering the mouse over a chest;
- Added Chest Category Snapshot feature: Save and share item categories across different 
  save files.

**Optimizations**

- Redesigned all UI components to be more align with the vanilla game;
- Redesigned Item Locking menu, now the interface has been improved for better usability;
- When open a category menu, the menu will be automatically jump to the most relative category;
- Optimized the vanilla "Add to existing stacks" button, now it can correctly recognize locked
  items and prevent them from being stored, and it can also stack your categorized items as 
  well (configurable).

**Compatibility Improve**

- Achieved mutual compatibility with the next version of _Convenient Inventory_ mod
  (As of this writing on 2026.3.25, this version is not yet released; the actual date depends on 
  the mod author). The experience for overlapping features has been unified.

**Breaking Changes for Android**

- Due to extensive changes and increased complexity in the new version, Android compatibility is
  currently not implemented. Support will be gradually reintroduced in future versions.

> For earlier update logs (1.x.x), see [Version History](VersionHistories/VersionHistory.md).

## Translating the mods

The mods can be translated into any language supported by the game, and SMAPI will automatically
use the right translations.

Contributions are welcome! See [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations)
on the wiki for help contributing translations.

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

| Language   |         Status         | Language   |      Status       |
|:-----------|:----------------------:|:-----------|:-----------------:|
| English    | [↻](i18n/default.json) | 한국어        |     [❑](i18n)     |
| Français   |       [❑](i18n)        | Português  | [↻](i18n/pt.json) |
| Deutsch    |       [❑](i18n)        | Русский    |     [❑](i18n)     |
| Magyar     |       [❑](i18n)        | Español    |     [❑](i18n)     |
| Italiano   |       [❑](i18n)        | Türkçe     |     [❑](i18n)     |
| 日本語        |       [❑](i18n)        | 中文         |        O.o        |

#### Translation Authors

* Thanks to [Kiyara99](https://next.nexusmods.com/profile/Kiyara99) for Português translation

## Requirements

Stardew Valley 1.6  
[SMAPI 4.0](https://smapi.io)

## Thanks to...

* [Pathoschild](https://github.com/Pathoschild) for [SMAPI](https://github.com/Pathoschild/SMAPI)
* [Ilyaki](https://github.com/Ilyaki) for [StackToNearbyChests](https://github.com/Ilyaki/StackToNearbyChests)
* [doncollins](https://github.com/doncollins) for [CategorizeChests](https://github.com/doncollins/StardewValleyMods)
* [aEnigmatic](https://github.com/aEnigmatic) for the old version of [Convenient Chests](https://github.com/aEnigmatic/ConvenientChests)
