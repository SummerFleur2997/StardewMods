# Convenient Chests

[English](README.md) | 简体中文

此 mod 为 [aEnigmatic](https://github.com/aEnigmatic) 开发的 [Convenient Chests](https://github.com/aEnigmatic/ConvenientChests) 的非官方重制版，目的在于解决作者长期未更新的问题。主要修复了一些 bug，同时添加了一些新的功能。

## 主要功能
合并了模组 [StackToNearbyChests](https://www.nexusmods.com/stardewvalley/mods/1787) 与 [CategorizeChests](https://www.nexusmods.com/stardewvalley/mods/1300) 的全部功能，并且允许玩家使用附近箱子内的物品进行打造，具体使用方法详见使用文档。
（使用文档在哪？？正在写，咕咕咕...）

## 版本更新日志

### 1.8.0

**新功能**
- 新增锁定物品功能，可锁定物品栏内的物品，防止其被堆叠至附近的箱子中；
- 禁止将工具堆叠至箱子中（可配置）。

**现有功能优化**
- 修改模组配置文件后，配置会立即生效（在之前的版本中，需要退出并重新进入存档后，修改了的配置才会生效）；
- 修改分类菜单内物品的排序，使用游戏内的排序方法；
- 修改分类菜单的排序，常用的分类标签将更加靠前。

### 1.7.0

**Bug 修复**
- 修复了使用 mod “堆叠” 功能，一次性将多于 999 个物品放入箱子中时，会导致物品数量错误的 bug（详见原项目 _[issue #30](https://github.com/aEnigmatic/ConvenientChests/issues/30)_ ）；
- 修复了在带有滚动条的物品分类标签页点击左上角的“全选”按钮时，只会选中当前所显示的物品的 bug。

**新功能**
- 新增 i18n 支持；
- 新增了一个配置选项，用于禁止按照字母顺序排序物品分类标签页标题，建议中文用户不要启用此选项。

## 运行需求
Stardew Valley 1.6  
[SMAPI 4.0](https://smapi.io)

## 鸣谢
* [Pathoschild](https://github.com/Pathoschild) 开发了 [SMAPI](https://github.com/Pathoschild/SMAPI)
* [Ilyaki](https://github.com/Ilyaki) 开发了 [StackToNearbyChests](https://github.com/Ilyaki/StackToNearbyChests)
* [doncollins](https://github.com/doncollins) 开发了 [CategorizeChests](https://github.com/doncollins/StardewValleyMods)
* [aEnigmatic](https://github.com/aEnigmatic) 开发了旧版本的 [Convenient Chests](https://github.com/aEnigmatic/ConvenientChests)