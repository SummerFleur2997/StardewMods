using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.ItemService;
using ConvenientChests.Framework.UserInterfacService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Background = ConvenientChests.Framework.UserInterfacService.Background;

namespace ConvenientChests.StashToChests.Framework;

internal class LockMenu : Widget
{
    // Styling settings
    private const int MaxItemRows = 7;
    private const int MaxItemColumns = 12;
    private const int MaxItemsPage = MaxItemColumns * MaxItemRows;

    private static int Padding => 2 * Game1.pixelZoom;
    private static SpriteFont HeaderFont => Game1.dialogueFont;

    // pagination
    private int Row { get; set; }
    private static int NumItems => Game1.player.Items.Count;

    // Elements
    private Widget Body { get; set; }
    private Widget TopRow { get; set; }
    private SpriteButton CloseButton { get; set; }
    private ScrollBar ScrollBar { get; set; }
    private Background Background { get; set; }
    private Label CategoryLabel { get; set; }
    private WrapBag ToggleBag { get; set; }
    private ItemKey[] BackpackItems { get; }

    private TooltipManager TooltipManager { get; }
    private InventoryData InventoryData { get; }

    public event Action OnClose;

    public LockMenu(InventoryData inventoryData, TooltipManager tooltipManager, int width)
    {
        TooltipManager = tooltipManager;
        InventoryData = inventoryData;
        Width = width;
        BackpackItems = GetBackpackItems();

        BuildWidgets();
        RecreateItemToggles();
        PositionElements();
    }

    private static ItemKey[] GetBackpackItems()
    {
        var itemKeyList = new List<ItemKey>();
        foreach (var item in Game1.player.Items)
        {
            if (item is Tool or null) continue;
            itemKeyList.Add(new ItemKey(item));
        }

        return itemKeyList.Distinct().ToArray();
    }

    private void BuildWidgets()
    {
        Background = AddChild(new Background(Sprites.MenuBackground));
        Body = AddChild(new Widget());
        TopRow = Body.AddChild(new Widget());
        ToggleBag = Body.AddChild(new WrapBag(MaxItemColumns * Game1.tileSize));

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

    /// <summary>
    /// 绘制各个辅助元素。
    /// Draw all the supplementary elements in position.
    /// </summary>
    private void PositionElements()
    {
        Body.Position = new Point(Background.Graphic.LeftBorderThickness, Background.Graphic.TopBorderThickness);

        // Figure out width
        Body.Width = ToggleBag.Width;
        TopRow.Width = Body.Width;
        // Width        = Body.Width + Background.Graphic.LeftBorderThickness + Background.Graphic.RightBorderThickness + Padding * 2;

        // Build the top row
        CategoryLabel.Text = I18n.LockItems_Title();
        CategoryLabel.CenterHorizontally();

        TopRow.Height = TopRow.Children.Max(c => c.Height);


        // Figure out height and vertical positioning
        ToggleBag.Y = TopRow.Y + TopRow.Height + Padding;
        Body.Height = ToggleBag.Y + ToggleBag.Height;
        Height = TopRow.Height +
                 Game1.tileSize * MaxItemRows + Padding * (MaxItemRows - 1) +
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

    /// <summary>
    /// 绘制物品列表。
    /// Draw the item lists in position.
    /// </summary>
    private void RecreateItemToggles()
    {
        var entries = BackpackItems
            .Select(key => new { Key = key, Item = key.GetOne() })
            .Skip(Row * MaxItemColumns)
            .Take(MaxItemsPage)
            .ToList();

        foreach (var entry in entries)
        {
            var toggle =
                ToggleBag.AddChild(new ItemToggle(TooltipManager, entry.Item, InventoryData.Locks(entry.Key)));
            toggle.OnToggle += () => ToggleItem(entry.Key);
        }
    }

    private void ToggleItem(ItemKey itemKey)
    {
        InventoryData.Toggle(itemKey);
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

        return true;
    }
}