# Better Hats API

简体中文 | [English](README.md)

**Better Hats API**（BHA）是一个为《星露谷物语》模组开发者设计的前置工具，它允许创建具有自定义 buff 效果、条件逻辑和独特行为的帽子。BHA 通过自定义 JSON 格式，让开发者能够通过 JSON 配置或深度集成 C# 代码来定义属性。

使用 BHA，您可以轻松应用标准属性加成（如耕种、采集），或根据游戏状态（季节、地点、天气）设置条件、触发自定义操作，还可以结合 C# 动态调整数值、实现更强大的功能。其设计强调灵活性：简单的效果仅需 JSON 即可实现，而复杂的行为则可以通过 C# API 来完成。

关于更多信息，请参见 [开发者文档](./docs/author-guide.md)。

## 版本更新日志

### 1.0.0

初次上传

## 计划添加的内容

- [ ] 针对 [Minute Time Helper](https://www.nexusmods.com/stardewvalley/mods/29950) 设计以分钟为单位的触发器
- [ ] 为图鉴菜单中条件与事件的描述添加“防剧透”功能，仅当满足某些条件后才可显示其效果
- [ ] 新增自定义 Buff 的支持
- [ ] （待定）修改 `content.json` 的数据结构，从 `Dictionary<string, HatData>` 修改为 `Dictionary<string, List<HatData>>` 以支持一个帽子有多个效果。

## 运行需求
Stardew Valley 1.6  
[SMAPI 4.0](https://smapi.io)

## 鸣谢
* [Pathoschild](https://github.com/Pathoschild) 开发了 [SMAPI](https://github.com/Pathoschild/SMAPI)
