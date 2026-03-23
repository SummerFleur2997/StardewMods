using ConvenientChests.AliasForChests;
using ConvenientChests.CategorizeChests.UI;
using ConvenientChests.Framework.DataService;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using StardewValley.Objects;
using UI.Component;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class ChestSideTab : IOverlay<ItemGrabMenu>
{
    private readonly Chest _chest;
    public ItemGrabMenu RootMenu { get; }
    public Tooltip? Tooltip { get; private set; }

    public AliasSetMenu? AliasMenu { get; set; }
    private SpriteButton? AliasButton { get; }
    private SpriteButton? CategorizeButton { get; }

    private readonly InventoryMenu.highlightThisItem _defaultChestHighlighter;
    private readonly InventoryMenu.highlightThisItem _defaultInventoryHighlighter;

    /// <summary>
    /// 构造函数，初始化 ChestOverlay 类。
    /// Constructor to initialize the ChestOverlay class.
    /// </summary>
    public ChestSideTab(ItemGrabMenu menu, Chest chest)
    {
        _chest = chest;
        RootMenu = menu;

        _defaultChestHighlighter = menu.inventory.highlightMethod;
        _defaultInventoryHighlighter = menu.ItemsToGrabMenu.highlightMethod;

        if (_chest.SpecialChestType == Chest.SpecialChestTypes.Enricher)
            return;

        // 添加分类和存储按钮，然后确定它们的位置，使它们在箱子界面左侧对齐。
        // Add categorize and stash buttons. Then determine their position to
        // align them on the left side of the chest interface.
        AliasButton = UIHelper.SideButton(0, 0, SideButtonVariant.Alias);
        AliasButton.OnPress += OpenAliasMenu;

        CategorizeButton = UIHelper.SideButton(0, 0, SideButtonVariant.Categorize);
        CategorizeButton.OnPress += OpenCategoryMenu;

        // var delta = ModEntry.IsAndroid
        //     // For android, use a fixed offset.
        //     ? 100 + ModEntry.Config.MobileOffset

        // // Calculate the offset based on the chest size, use a dynamic offset based on the chest size.
        // var delta = _chest.GetActualCapacity() switch
        // {
        //     // Big chests, >=70 to compatible with unlimited storage
        //     >= 70 => 128,
        //     // Junimo chests / Shipping bin 
        //     9 => 34,
        //     // Common chests
        //     _ => 112
        // };

        var x = RootMenu.fillStacksButton.bounds.X + 80;
        var y = RootMenu.fillStacksButton.bounds.Y;
        AliasButton.SetPosition(x, y);

        y += 80;
        CategorizeButton.SetPosition(x, y);
    }

    /// <inheritdoc />
    public void DrawUi(SpriteBatch b)
    {
        var drawTooltip = true;
        if (!ModEntry.Config.HideSideTab)
        {
            if (ModEntry.AliasModule.IsActive)
                AliasButton?.Draw(b);

            if (ModEntry.CategorizeModule.IsActive)
                CategorizeButton?.Draw(b);
        }
        else drawTooltip = false;

        if (AliasMenu is not null)
            AliasMenu.Draw(b);

        else if (!string.IsNullOrWhiteSpace(RootMenu.hoverText))
            IClickableMenu.drawHoverText(b, RootMenu.hoverText, Game1.smallFont);

        if (drawTooltip)
            Tooltip?.Draw(b);

        RootMenu.drawMouse(b);
    }

    /// <summary>
    /// 打开分类菜单，同时按照配置文件决定分类列表排序方式。
    /// Open the category menu and sort the list of categories based on configuration settings.
    /// </summary>
    private void OpenCategoryMenu()
    {
        var data = _chest.GetChestData();
        // var delta = ModEntry.IsAndroid ? 70 : 0; // y + delta, height - delta
        var menu = new CategoryChestMenu(RootMenu.xPositionOnScreen, RootMenu.yPositionOnScreen,
            RootMenu.width, RootMenu.height, data, RootMenu, IClickableMenu.borderWidth);
        Game1.activeClickableMenu = menu;
    }

    /// <summary>
    /// 打开备注修改菜单，修改箱子备注。
    /// Open the alias menu to edit the alias of this chest.
    /// </summary>
    private void OpenAliasMenu()
    {
        var data = _chest.GetChestData();
        AliasMenu = new AliasSetMenu(data, this);
        SetItemsClickable(false);
    }

    /// <inheritdoc />
    public bool ReceiveLeftClick(int x, int y)
    {
        if (_chest.SpecialChestType == Chest.SpecialChestTypes.Enricher)
            return false;

        if (AliasMenu is not null)
        {
            AliasMenu.ReceiveLeftClick(x, y);
            return true;
        }

        if (ModEntry.AliasModule.IsActive && (AliasButton?.Contains(x, y) ?? false))
            return AliasButton?.ReceiveLeftClick(x, y) ?? false;

        if (ModEntry.CategorizeModule.IsActive && (CategorizeButton?.Contains(x, y) ?? false))
            return CategorizeButton.ReceiveLeftClick(x, y);

        return false;
    }

    /// <inheritdoc />
    public bool ReceiveCursorHover(int x, int y)
    {
        Tooltip = null;
        if (_chest.SpecialChestType == Chest.SpecialChestTypes.Enricher)
            return false;

        if (ModEntry.AliasModule.IsActive && (AliasButton?.Contains(x, y) ?? false))
        {
            Tooltip = AliasButton.Tooltip;
            return true;
        }

        if (ModEntry.CategorizeModule.IsActive && (CategorizeButton?.Contains(x, y) ?? false))
        {
            Tooltip = CategorizeButton.Tooltip;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public bool ReceiveKeyPress(Keys key) => AliasMenu?.HandleOrSuppressThisKeyPress(key) ?? false;

    public void SetItemsClickable(bool clickable)
    {
        if (clickable)
        {
            RootMenu.inventory.highlightMethod = _defaultChestHighlighter;
            RootMenu.ItemsToGrabMenu.highlightMethod = _defaultInventoryHighlighter;
        }
        else
        {
            RootMenu.inventory.highlightMethod = _ => false;
            RootMenu.ItemsToGrabMenu.highlightMethod = _ => false;
        }
    }
}