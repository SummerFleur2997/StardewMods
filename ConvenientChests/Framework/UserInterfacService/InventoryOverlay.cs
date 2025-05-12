using ConvenientChests.Framework.InventoryService;
using ConvenientChests.StashToChests.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientChests.Framework.UserInterfacService;

internal class InventoryOverlay : Widget
{
    private Farmer Player { get; }
    private GameMenu GameMenu { get; }
    private TextButton LockButton { get; set; }
    private LockMenu LockMenu { get; set; }
    private TooltipManager TooltipManager { get; }

    public InventoryOverlay(GameMenu menu)
    {
        Player = Game1.player;
        GameMenu = menu;
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
        LockButton.Position = new Point(
            GameMenu.xPositionOnScreen + GameMenu.width / 2 - LockButton.Width - 106 * Game1.pixelZoom,
            GameMenu.yPositionOnScreen + 30 * Game1.pixelZoom);
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
        var inventoryData = InventoryManager.GetInventoryData(Player);
        LockMenu = new LockMenu(inventoryData, TooltipManager, GameMenu.width - 24);
        LockMenu.Position = new Point(
            GameMenu.xPositionOnScreen - GlobalBounds.X - 12,
            GameMenu.yPositionOnScreen - GlobalBounds.Y - 60);

        LockMenu.OnClose += CloseLockMenu;
        AddChild(LockMenu);
    }

    private void CloseLockMenu()
    {
        RemoveChild(LockMenu);
        LockMenu = null;
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
}