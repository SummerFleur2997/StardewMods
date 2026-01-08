using System.Collections.Generic;
using System.Linq;
using ConvenientChests.Framework.InventoryService;
using ConvenientChests.Framework.ItemService;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.StashToChests.Framework;

internal class LockMenu : BaseMenu
{
    /// <summary>
    /// Used to compatible with LookupAnything.
    /// </summary>
    [UsedImplicitly] public Item HoveredItem;

    private readonly InventoryData _inventoryData;
    private readonly List<ItemKey> _backpackItems;
    private TextLabel _label;
    private GridMenu _gridMenu;
    private ItemToggle _hoveredToggle;

    public LockMenu(int x, int y, int width, int height, InventoryData inventoryData, int padding = 0)
        : base(x, y, width, height)
    {
        _inventoryData = inventoryData;
        _backpackItems = Game1.player.GetBackpackItems();

        BuildWidgets(padding);
        CreateItemToggles();
    }

    private void BuildWidgets(int padding)
    {
        _label = new TextLabel(I18n.LockItems_Title(), Color.Black, Game1.smallFont);
        _label.SetPosition(
            xPositionOnScreen + (width - _label.Width) / 2,
            yPositionOnScreen + padding);

        var maxColumns = ModEntry.IsAndroid ? 99 : 12;
        _gridMenu = new GridMenu(
            xPositionOnScreen + padding,
            _label.Y + _label.Height + Game1.pixelZoom * 4,
            width - padding * 2,
            height - _label.Height - padding * 2 - Game1.pixelZoom * 4,
            66, maxColumns);

        AddChild(_gridMenu);
        AddChild(_label);
    }

    /// <summary>
    /// 绘制物品列表。
    /// Draw the item lists in position.
    /// </summary>
    private void CreateItemToggles()
    {
        var entries = _backpackItems
            .Select(itemKey => new ItemEntry(itemKey))
            .OrderBy(itemEntry => itemEntry)
            .ToList();

        var toggles = new List<ItemToggle>();
        foreach (var entry in entries)
        {
            var toggle = new ItemToggle(entry.Item, _inventoryData.Locks(entry.ItemKey));
            toggle.OnToggle += () => ToggleItem(entry.ItemKey);
            toggle.OnHover += () =>
            {
                _hoveredToggle = toggle;
                HoveredItem = toggle.Item;
            };
            toggles.Add(toggle);
        }

        _gridMenu.AddComponents(toggles);

    }

    private void ToggleItem(ItemKey itemKey) => _inventoryData.Toggle(itemKey);

    public override void Draw(SpriteBatch b) => _hoveredToggle?.Tooltip.Draw(b);

    public override bool ReceiveCursorHover(int x, int y)
    {
        _hoveredToggle = null;
        HoveredItem = null;
        return base.ReceiveCursorHover(x, y);
    }
}