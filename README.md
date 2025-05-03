# Convenient Chests

English | [简体中文](README_zh.md)

This is an unofficial fork from [aEnigmatic's Convenient Chests](https://github.com/aEnigmatic/ConvenientChests), mainly fixed some bugs and added a few features.

## Main features
Combines features of [StackToNearbyChests](https://www.nexusmods.com/stardewvalley/mods/1787) and [CategorizeChests](https://www.nexusmods.com/stardewvalley/mods/1300), and allows crafting from nearby chests.

## Update logs

### 1.8.0 

**New Features**
- Added item locking functionality: Lock items in your inventory to prevent them from being stashed into nearby chests.
- Tools can now be prevented from being stashed into chests (configurable).

**Optimizations & Improvements**
- Mod configuration changes now take effect immediately (previously required exiting and re-entering the save file).
- Updated item sorting in the category menu to use the game’s native sorting method.
- Improved category menu ordering: Frequently used categories now appear more prominently (For English users, you can enable the "Sort categories" option in mod config to sort alphabetically).

### 1.7.0

**Bug Fixes**
- Fixed inaccurate item quantity when stashing over 999 items via "Stash to Nearby Chests" key. _(Resolves [original issue #30](https://github.com/aEnigmatic/ConvenientChests/issues/30))_
- Fixed "Select All" button only selecting visible items in scrollable views 

**New Features**
- Added internationalization (i18n) support
- Added configuration option to toggle alphabetical sorting for tab headers in "Categorize Chest" feature, mainly designed for CJK users. .

## Translating the mods
The mods can be translated into any language supported by the game, and SMAPI will automatically
use the right translations.

Contributions are welcome! See [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations)
on the wiki for help contributing translations.

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

| Language      |               Status               | Language       |           Status           |
|:--------------|:----------------------------------:|:---------------|:--------------------------:|
| English       | [↻](ConvenientChests/i18n/en.json) | 한국어            | [❑](ConvenientChests/i18n) |
| Français      |     [❑](ConvenientChests/i18n)     | Português      | [❑](ConvenientChests/i18n) |
| Deutsch       |     [❑](ConvenientChests/i18n)     | Русский        | [❑](ConvenientChests/i18n) |
| Magyar        |     [❑](ConvenientChests/i18n)     | Español        | [❑](ConvenientChests/i18n) |
| Italiano      |     [❑](ConvenientChests/i18n)     | Türkçe         | [❑](ConvenientChests/i18n) |
| 日本語           |     [❑](ConvenientChests/i18n)     | 中文             |            O.o             |

## Requirements
Stardew Valley 1.6  
[SMAPI 4.0](https://smapi.io)

## Thanks to...
* [Pathoschild](https://github.com/Pathoschild) for [SMAPI](https://github.com/Pathoschild/SMAPI)
* [Ilyaki](https://github.com/Ilyaki) for [StackToNearbyChests](https://github.com/Ilyaki/StackToNearbyChests)
* [doncollins](https://github.com/doncollins) for [CategorizeChests](https://github.com/doncollins/StardewValleyMods)
* [aEnigmatic](https://github.com/aEnigmatic) for the old version of [Convenient Chests](https://github.com/aEnigmatic/ConvenientChests)
