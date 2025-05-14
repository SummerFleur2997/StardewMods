## Version History

English | [简体中文](VersionHistory_zh.md)

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
- Added configuration option to toggle alphabetical sorting for tab headers in "Categorize Chest" feature, mainly designed for CJK users.