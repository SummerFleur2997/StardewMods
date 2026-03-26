## Version History

English | [简体中文](VersionHistory_zh.md)

### 1.8.4

**New Features**

- Introduce a drop-down menu for the category menu.

**Optimizations**

- Provide an api for other mods.
- Now, when using a new chest to swap the old chest, the category data will keep.
- More clever ui design to fit with other mods or Android device.

**Bug Fixes**

- Fix a bug causing double ingredient use when crafting.
  - Merged from [Pull #8](https://github.com/SummerFleur2997/StardewMods/pull/8)
    (thanks to [DrakeCaesar](https://github.com/DrakeCaesar)!)

### 1.8.3

**New Features**

- Introduce a new feature to auto stash every 30 minutes.

**Optimizations**

- Add more categorizable items.
- Compatible with QuickSave.
- Polish Chinese wording.

**Bug Fixes**

- Fixed the issue that the Category menu does not fit the screen in Android.

### 1.8.2

**New Features**

- Added Portuguese translation. (Thanks to [Kiyara99](https://next.nexusmods.com/profile/Kiyara99))

**Optimizations**

- Add a config to set the offset of side buttons for Android players.

**Bug Fixes**

- Fixed the issue with incorrect alphabetical sorting.

### 1.8.1

**New Features**

- Added multiplayer services, now the mod's data will be synced to everyone. (In previous versions, categorize data
  can't be synced across players, farmhands' modification to chest categorize can't be saved either)

**Optimizations & Improvements**

- Buttons are now compatible with mod [AutoHealAndEnergize](https://www.nexusmods.com/stardewvalley/mods/29035)'s food
  bag layout.
- Added key-bind list support.

**Bug Fixes**

- Fix the bug which caused the unavailable of Category button.
- Fix some potential bugs.

### 1.8.0

**New Features**

- Added item locking functionality: Lock items in your inventory to prevent them from being stashed into nearby chests.
- Tools can now be prevented from being stashed into chests (configurable).

**Optimizations & Improvements**

- Mod configuration changes now take effect immediately (previously required exiting and re-entering the save file).
- Updated item sorting in the category menu to use the game’s native sorting method.
- Improved category menu ordering: Frequently used categories now appear more prominently (For English users, you can
  enable the "Sort categories" option in mod config to sort alphabetically).

### 1.7.0

**Bug Fixes**

- Fixed inaccurate item quantity when stashing over 999 items via "Stash to Nearby Chests" key.
  _(Resolves [original issue #30](https://github.com/aEnigmatic/ConvenientChests/issues/30))_
- Fixed "Select All" button only selecting visible items in scrollable views

**New Features**

- Added internationalization (i18n) support
- Added configuration option to toggle alphabetical sorting for tab headers in "Categorize Chest" feature, mainly
  designed for CJK users.