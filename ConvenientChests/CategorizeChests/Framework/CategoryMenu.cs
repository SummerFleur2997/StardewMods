using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.ChestService;
using ConvenientChests.Framework.ItemService;
using ConvenientChests.Framework.UserInterfacService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Background = ConvenientChests.Framework.UserInterfacService.Background;

namespace ConvenientChests.CategorizeChests.Framework;

internal class CategoryMenu : Widget
{
    // Styling settings
    private const int MaxItemRows = 7;
    private const int MaxItemColumns = 12;
    private const int MaxItemsPage = MaxItemColumns * MaxItemRows;

    private static int Padding => 2 * Game1.pixelZoom;
    private static SpriteFont HeaderFont => Game1.dialogueFont;

    // pagination
    private int Row { get; set; }

    private int NumItems => ActiveCategory.CategoryDisplayName == ""
        ? 0
        : CategoryDataManager.Categories[ActiveCategory].Count;

    // Elements
    private Widget Body { get; set; }
    private Widget TopRow { get; set; }
    private LabeledCheckbox SelectAllButton { get; set; }
    private SpriteButton CloseButton { get; set; }
    private ScrollBar ScrollBar { get; set; }
    private Background Background { get; set; }
    private Label CategoryLabel { get; set; }
    private SpriteButton PrevButton { get; set; }
    private SpriteButton NextButton { get; set; }
    private WrapBag ToggleBag { get; set; }
    private IEnumerable<ItemToggle> ItemToggles => ToggleBag.Children.OfType<ItemToggle>();

    private CategoryDataManager CategoryDataManager { get; }
    private TooltipManager TooltipManager { get; }
    private ChestData ChestData { get; }
    private int Index { get; set; }
    public List<ItemCategoryName> Categories { get; set; }

    private ItemCategoryName ActiveCategory => Categories[Index];

    public event Action OnClose;

    public void RefreshMenu()
    {
        SetCategory(0);
    }

    public CategoryMenu(ChestData chestData, CategoryDataManager categoryDataManager, TooltipManager tooltipManager,
        int width)
    {
        CategoryDataManager = categoryDataManager;
        TooltipManager = tooltipManager;
        ChestData = chestData;
        Width = width;

        Categories = categoryDataManager.ItemCategories;
        BuildWidgets();

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

        CategoryLabel = TopRow.AddChild(new Label("", Color.Black, HeaderFont));

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
        // Width        = Body.Width + Background.Graphic.LeftBorderThickness + Background.Graphic.RightBorderThickness + Padding * 2;

        // Build the top row
        var categoriesName = ItemCategoryName.GetCategoriesDisplayNames(Categories);
        var longestCat = categoriesName.OrderByDescending(s => s.Length).First();
        var headerWidth = (int)HeaderFont.MeasureString(longestCat).X;
        NextButton.X = TopRow.Width / 2 + headerWidth / 2;
        PrevButton.X = TopRow.Width / 2 - PrevButton.Width - headerWidth / 2;

        SelectAllButton.X = Padding;

        CategoryLabel.Text = ActiveCategory.CategoryDisplayName;
        CategoryLabel.CenterHorizontally();

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
        ScrollBar.ScrollMax = NumItems;
        ScrollBar.Step = MaxItemsPage;
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
        return ItemToggles.Count(t => !t.Active) == 0;
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