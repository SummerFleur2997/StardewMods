using ConvenientChests.AliasForChests;
using ConvenientChests.CategorizeChests;
using ConvenientChests.CategorizeChests.UI;
using ConvenientChests.Framework.DataService;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using StardewValley.Objects;
using UI.Component;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class ChestOverlay : IOverlay<ItemGrabMenu>
{
    private readonly Chest _chest;
    public ItemGrabMenu RootMenu { get; }
    public Tooltip? Tooltip { get; private set; }

    public AliasSetMenu? AliasMenu { get; set; }
    private SpriteButton AliasButton { get; }
    private SpriteButton CategorizeButton { get; }

    private readonly LockItemMenu _lockItemMenu;
    private readonly InventoryMenu.highlightThisItem _defaultChestHighlighter;
    private readonly InventoryMenu.highlightThisItem _defaultInventoryHighlighter;

    /// <summary>
    /// 构造函数，初始化 ChestOverlay 类。
    /// Constructor to initialize the ChestOverlay class.
    /// </summary>
    public ChestOverlay(ItemGrabMenu menu, Chest chest)
    {
        _chest = chest;
        RootMenu = menu;

        _defaultChestHighlighter = menu.inventory.highlightMethod;
        _defaultInventoryHighlighter = menu.ItemsToGrabMenu.highlightMethod;

        // 添加分类和存储按钮，然后确定它们的位置，使它们在箱子界面右侧与原版 UI 对齐。
        // Add categorize and stash buttons. Then determine their position to align
        // them on the right side of the chest interface, matching the original UI.
        // LockButton = UIHelper.SideButton(0, 0, SideButtonVariant.Lock);
        AliasButton = UIHelper.SideButton(0, 0, SideButtonVariant.Alias);
        AliasButton.OnPress += OpenAliasMenu;

        CategorizeButton = UIHelper.SideButton(0, 0, SideButtonVariant.Categorize);
        CategorizeButton.OnPress += OpenCategoryMenu;

        /* Old logic for android compatibility
        var delta = ModEntry.IsAndroid
            // For android, use a fixed offset.
            ? 100 + ModEntry.Config.MobileOffset

        // Calculate the offset based on the chest size, use a dynamic offset based on the chest size.
        var delta = _chest.GetActualCapacity() switch
        {
            // Big chests, >=70 to compatible with unlimited storage
            >= 70 => 128,
            // Junimo chests / Shipping bin
            9 => 34,
            // Common chests
            _ => 112
        };*/

        var x = RootMenu.fillStacksButton.bounds.X + 80;
        var y = RootMenu.fillStacksButton.bounds.Y - 80;
        _lockItemMenu = new LockItemMenu(x, y);

        y += 80;
        AliasButton.SetPosition(x, y);

        y += 80;
        CategorizeButton.SetPosition(x, y);
    }

    /// <inheritdoc />
    public void DrawUi(SpriteBatch b)
    {
        if (_chest.SpecialChestType == Chest.SpecialChestTypes.Enricher)
            return;

        var drawTooltip = true;
        if (!ModEntry.Config.HideSideTab)
        {
            if (AliasForChestsModule.Instance.IsActive)
                AliasButton.Draw(b);

            if (CategorizeChestsModule.Instance.IsActive)
                CategorizeButton.Draw(b);
        }
        else drawTooltip = false;

        _lockItemMenu.Draw(b, RootMenu.inventory, RootMenu.ItemsToGrabMenu);

        if (AliasMenu is not null)
            AliasMenu.Draw(b);

        else if (DrawNativeTooltip())
            IClickableMenu.drawHoverText(b, RootMenu.hoverText, Game1.smallFont);

        if (drawTooltip)
            Tooltip?.Draw(b);

        RootMenu.drawMouse(b);
    }

    private bool DrawNativeTooltip() =>
        !string.IsNullOrWhiteSpace(RootMenu.hoverText) &&
        (RootMenu.hoveredItem == null || RootMenu.ItemsToGrabMenu == null) &&
        RootMenu.hoverAmount <= 0;

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

        if (_lockItemMenu.ReceiveLeftClick(x, y, RootMenu.inventory, RootMenu.ItemsToGrabMenu))
            return true;

        if (AliasMenu is not null)
        {
            AliasMenu.ReceiveLeftClick(x, y);
            return true;
        }

        if (AliasForChestsModule.Instance.IsActive && AliasButton.Contains(x, y))
            return AliasButton.ReceiveLeftClick(x, y);

        if (CategorizeChestsModule.Instance.IsActive && CategorizeButton.Contains(x, y))
            return CategorizeButton.ReceiveLeftClick(x, y);

        return false;
    }

    /// <inheritdoc />
    public bool ReceiveCursorHover(int x, int y)
    {
        Tooltip = null;
        if (_chest.SpecialChestType == Chest.SpecialChestTypes.Enricher)
            return false;

        if (_lockItemMenu.ReceiveCursorHover(x, y))
            return true;

        if (AliasForChestsModule.Instance.IsActive && AliasButton.Contains(x, y))
        {
            Tooltip = AliasButton.Tooltip;
            return true;
        }

        if (CategorizeChestsModule.Instance.IsActive && CategorizeButton.Contains(x, y))
        {
            Tooltip = CategorizeButton.Tooltip;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public bool ReceiveKeyPress(Keys key)
    {
        if (_lockItemMenu.ReceiveKeyPress(key))
            return true;

        return AliasMenu?.HandleOrSuppressThisKeyPress(key) ?? false;
    }

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