using ConvenientChests.Framework.InventoryService;
using ConvenientChests.StashToChests.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using UI.UserInterface;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class InventoryOverlay : Widget
{
    private readonly Farmer _player;
    private readonly GameMenu _gameMenu;
    private TextButton LockButton { get; set; }
    private LockMenu LockMenu { get; set; }
    private TooltipManager TooltipManager { get; }

    public InventoryOverlay(GameMenu menu)
    {
        _player = Game1.player;
        _gameMenu = menu;
        TooltipManager = new TooltipManager();
        AddButtons();
    }

    protected override void OnParent(Widget parent)
    {
        base.OnParent(parent);

        if (parent == null) return;
        Width = parent.Width;
        Height = parent.Height;
    }

    public override void Draw(SpriteBatch batch)
    {
        base.Draw(batch);
        TooltipManager.Draw(batch);
    }

    private void AddButtons()
    {
        LockButton = new TextButton(I18n.LockItems_Title(), Sprites.LeftProtrudingTab);
        LockButton.OnPress += ToggleMenu;
        if (ModEntry.StashModule.IsActive || ModEntry.CategorizeModule.IsActive) AddChild(LockButton);

        PositionButtons();
    }

    private void PositionButtons()
    {
        var delta = ModEntry.IsAndroid ? 100 + ModEntry.Config.MobileOffset : 106;

        LockButton.Position = new Point(
            _gameMenu.xPositionOnScreen + _gameMenu.width / 2 - LockButton.Width - delta * Game1.pixelZoom,
            _gameMenu.yPositionOnScreen + 30 * Game1.pixelZoom);
    }

    private void ToggleMenu()
    {
        if (LockMenu == null)
            OpenLockMenu();

        else
            CloseLockMenu();
    }

    private void OpenLockMenu()
    {
        var inventoryData = InventoryManager.GetInventoryData(_player);
        LockMenu = new LockMenu(inventoryData, TooltipManager, _gameMenu.width);
        LockMenu.Position = new Point(
            (Game1.uiViewport.Width - LockMenu.Width) / 2,
            (Game1.uiViewport.Height - LockMenu.Height) / 2);

        LockMenu.OnClose += CloseLockMenu;
        AddChild(LockMenu);

        SetItemsClickable(false);
    }

    private void CloseLockMenu()
    {
        RemoveChild(LockMenu);
        LockMenu = null;

        SetItemsClickable(true);
    }

    public override bool ReceiveLeftClick(Point point)
    {
        var hit = PropagateLeftClick(point);
        if (!hit && LockMenu != null)
            // 如果点击菜单外部，尝试关闭菜单。
            // If clicking outside the menu, try to close it.
            CloseLockMenu();
        return hit;
    }

    /// <summary>
    /// 设置物品是否可点击。
    /// Set whether items are clickable.
    /// </summary>
    private void SetItemsClickable(bool clickable) // todo
    {
        ModEntry.Log(
            clickable ? "Set items clickable." : "Set items not clickable.",
            LogLevel.Debug
        );
    }
}