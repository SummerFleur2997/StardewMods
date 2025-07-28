using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.ItemService;
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using StardewValley;
using UI.UserInterface;
using Background = UI.UserInterface.Background;

namespace ConvenientChests.CategorizeChests.Framework;

internal class CategoryMenu : ModMenu
{
    private int NumItems => ActiveCategory.CategoryDisplayName == ""
        ? 0
        : CategoryDataManager.Categories[ActiveCategory].Count;
    private ChestData ChestData { get; }
    private LabeledCheckbox SelectAllButton { get; set; }
    private SpriteButton PrevButton { get; set; }
    private SpriteButton NextButton { get; set; }
    private int Index { get; set; }
    private List<ItemCategoryName> Categories { get; set; }
    private ItemCategoryName ActiveCategory => Categories[Index];
    public event Action OnClose;

    public CategoryMenu(ChestData chestData, TooltipManager tooltipManager, int width)
    {
        TooltipManager = tooltipManager;
        ChestData = chestData;
        Width = width;

        Categories = CategoryDataManager.ItemCategories;

        BuildWidgets();
        SortCategories();
    }

    private void SortCategories()
    {
        // 根据配置文件决定列表排序方式
        // Determine list sorting method based on configuration settings
        if (ModEntry.Config.EnableSort)
        {
            // 按字母顺序排序
            // Sort in alphabetical order
            Categories = Categories
                .OrderBy(c => c.CategoryDisplayName)
                .ToList();
        }
        else
        {
            // 定义自定义排序顺序的基准名称列表
            // Define custom sorting order using base names
            var customOrder = new List<string>
            {
                "Vegetable", "Fruit", "Flower", "Animal Product", "Artisan Goods", "Seed", "Fertilizer",
                "Fish", "Bait", "Fishing Tackle", "Crafting", "Machine", "BigCrafts", "Book", "Skill Book", 
                "Trash", "Tool", "Weapons", "Ring", "Hats", "Shirts", "Pants", "Footwear", "Mannequin", 
                "Decor", "Wallpaper", "Flooring", "Consumable", "Cooking", "Miscellaneous", "Trinket", 
                "Monster Loot", "Artifact", "Mineral", "Resource", "Forage"
            };

            // 创建基准名称到排序索引的字典
            // Create lookup dictionary: base name -> predefined sorting index
            var orderDictionary = customOrder
                .Select((name, index) => new { name, index })
                .ToDictionary(item => item.name, item => item.index);

            // 根据自定义顺序排序
            // Sort in custom rules
            Categories = Categories
                .OrderBy(c => orderDictionary.GetValueOrDefault(c.CategoryBaseName, int.MaxValue))
                .ToList();
        }
        SetCategory(Index);
    }

    private void BuildWidgets()
    {
        Background = AddChild(new Background(Sprites.MenuBackground));
        Body = AddChild(new Widget());
        TopRow = Body.AddChild(new Widget());
        ToggleBag = Body.AddChild(new WrapBag(MaxItemColumns * Game1.tileSize));

        NextButton = TopRow.AddChild(new SpriteButton(Sprites.RightArrow));
        PrevButton = TopRow.AddChild(new SpriteButton(Sprites.LeftArrow));
        NextButton.OnPress += () => CycleCategory(1);
        PrevButton.OnPress += () => CycleCategory(-1);

        SelectAllButton = TopRow.AddChild(new LabeledCheckbox(I18n.Categorize_All()));
        SelectAllButton.OnChange += OnToggleSelectAll;

        CloseButton = AddChild(new SpriteButton(Sprites.ExitButton));
        CloseButton.OnPress += () => OnClose?.Invoke();

        TitleLabel = TopRow.AddChild(new Label("", Color.Black, HeaderFont));

        ScrollBar = AddChild(new ScrollBar());
        ScrollBar.OnScroll += (_, args) => UpdateScrollPosition(args.Position);
    }

    private void UpdateScrollPosition(int position)
    {
        Row = Math.Max(0, position / MaxItemColumns);
        RecreateItemToggles();
    }

    private void PositionElements()
    {
        Body.Position = new Point(Background.Graphic.LeftBorderThickness, Background.Graphic.TopBorderThickness);

        // Figure out width
        Body.Width = ToggleBag.Width;
        TopRow.Width = Body.Width;

        // Build the top row
        var categoriesName = ItemCategoryName.GetCategoriesDisplayNames(Categories);
        var longestCat = categoriesName.OrderByDescending(s => s.Length).First();
        var headerWidth = (int)HeaderFont.MeasureString(longestCat).X;
        NextButton.X = TopRow.Width / 2 + headerWidth / 2;
        PrevButton.X = TopRow.Width / 2 - PrevButton.Width - headerWidth / 2;

        SelectAllButton.X = Padding;

        TitleLabel.Text = ActiveCategory.CategoryDisplayName;
        TitleLabel.CenterHorizontally();

        TopRow.Height = TopRow.Children.Max(c => c.Height);

        foreach (var child in TopRow.Children)
            child.Y = TopRow.Height / 2 - child.Height / 2;

        // Figure out height and vertical positioning
        ToggleBag.Y = TopRow.Y + TopRow.Height + Padding;
        Body.Height = ToggleBag.Y + ToggleBag.Height;
        Height = TopRow.Height + Game1.tileSize * MaxItemRows + Padding * (MaxItemRows - 1) +
                 Background.Graphic.TopBorderThickness + Background.Graphic.BottomBorderThickness + Padding * 2;

        Background.Width = Width;
        Background.Height = Height;

        CloseButton.Position = new Point(Width - CloseButton.Width, 0);

        ScrollBar.Position = new Point(Width - 64 - 3 * 4, CloseButton.Height);
        ScrollBar.Height = Height - CloseButton.Height - 16;
        ScrollBar.Visible = NumItems > MaxItemsPage;

        ScrollBar.ScrollPosition = 0;
        ScrollBar.ScrollMax = NumItems - MaxItemColumns * (MaxItemRows - 2);
        ScrollBar.Step = MaxItemColumns * 2;
    }

    private void OnToggleSelectAll(bool on)
    {
        if (on)
            SelectAll();
        else
            SelectNone();
    }

    private void SelectAll()
    {
        var allItems = CategoryDataManager.Categories[ActiveCategory];
        foreach (var itemKey in allItems)
            if (!ChestData.Accepts(itemKey))
                ChestData.Toggle(itemKey);

        RecreateItemToggles();
    }

    private void SelectNone()
    {
        var allItems = CategoryDataManager.Categories[ActiveCategory];
        foreach (var itemKey in allItems)
            if (ChestData.Accepts(itemKey))
                ChestData.Toggle(itemKey);

        RecreateItemToggles();
    }

    private void CycleCategory(int offset)
    {
        SetCategory(Mod(Index + offset, Categories.Count));
        return;

        int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }

    private void SetCategory(int index)
    {
        Index = index;
        Row = 0;

        RecreateItemToggles();

        SelectAllButton.Checked = AreAllSelected();

        PositionElements();
    }

    private void RecreateItemToggles()
    {
        ToggleBag.RemoveChildren();

        var entries = CategoryDataManager.Categories[ActiveCategory]
            .Select(itemKey => new ItemEntry(itemKey))
            .OrderBy(itemEntry => itemEntry)
            .Skip(Row * MaxItemColumns)
            .Take(MaxItemsPage)
            .ToList();

        foreach (var entry in entries)
        {
            var toggle =
                ToggleBag.AddChild(new ItemToggle(TooltipManager, entry.Item, ChestData.Accepts(entry.ItemKey)));
            toggle.OnToggle += () => ToggleItem(entry.ItemKey);
        }
    }

    private void ToggleItem(ItemKey itemKey)
    {
        ChestData.Toggle(itemKey);
        SelectAllButton.Checked = AreAllSelected();
    }

    private bool AreAllSelected()
    {
        return ToggleBag.Children.OfType<ItemToggle>().Count(t => !t.Active) == 0;
    }

    public override bool ReceiveLeftClick(Point point)
    {
        PropagateLeftClick(point);
        return true;
    }

    public override bool ReceiveScrollWheelAction(int amount)
    {
        var direction = amount > 1 ? -1 : 1;

        if (ScrollBar.Visible)
            switch (direction)
            {
                case -1 when ScrollBar.ScrollPosition > 0:
                case +1 when ScrollBar.ScrollPosition < ScrollBar.ScrollMax:
                    ScrollBar.Scroll(direction);
                    return true;
            }

        CycleCategory(direction);
        return true;
    }
}