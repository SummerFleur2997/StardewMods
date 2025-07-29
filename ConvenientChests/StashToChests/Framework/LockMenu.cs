using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.ItemService;
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using StardewValley;
using UI.UserInterface;
using Background = UI.UserInterface.Background;

namespace ConvenientChests.StashToChests.Framework;

internal class LockMenu : ModMenu
{
    private static int NumItems => Game1.player.Items.Count;
    private InventoryData InventoryData { get; }
    private ItemKey[] BackpackItems { get; }
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

        TitleLabel = TopRow.AddChild(new Label("", Color.Black, HeaderFont));

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

        // Build the top row
        TitleLabel.Text = I18n.LockItems_Title();
        TitleLabel.CenterHorizontally();

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
        ScrollBar.ScrollMax = NumItems - MaxItemColumns * (MaxItemRows - 2);
        ScrollBar.Step = MaxItemColumns * 2;
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