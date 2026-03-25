using ConvenientChests.CategorizeChests;
using ConvenientChests.Framework.Extensions;
using ConvenientChests.StashToChests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using UI.Component;
using UI.Sprite;

namespace ConvenientChests.Framework.UserInterfaceService;

public class LockItemMenu : IHaveTooltip
{
    public Tooltip? Tooltip { get; private set; }

    private bool EditMode
    {
        get => _editMode;
        set
        {
            if (_editMode == value)
                return;

            LockButton.Texture = value ? _okSprite : _lockSprite;
            Tooltip = null;
            _editMode = value;
        }
    }

    private bool _editMode;

    private SpriteButton LockButton { get; }

    private int _count;
    private readonly Tooltip _editTooltip;
    private readonly TextureRegion _lockSprite;
    private readonly TextureRegion _okSprite;
    private readonly TextureRegion _lockFrame;

    public LockItemMenu(int x, int y)
    {
        LockButton = UIHelper.SideButton(0, 0, SideButtonVariant.Lock);

        _editTooltip = new Tooltip(desc: I18n.UI_LockItems_Desc());
        _lockSprite = LockButton.Texture;
        _okSprite = new TextureRegion(Game1.mouseCursors, 128, 256, 64, 64);

        // var delta = ModEntry.IsAndroid ? 100 + ModEntry.Config.MobileOffset : 106;
        LockButton.SetPosition(x, y);
        LockButton.OnPress += () => EditMode = !EditMode;
        _lockFrame = new TextureRegion(UIHelper.Texture, 112, UIHelper.YOffset, 16, 16);
    }

    public void Draw(SpriteBatch b, InventoryMenu playerInventory, InventoryMenu? chestInventory = null)
    {
        var drawTooltip = true;
        if ((StashToChestsModule.Instance.IsActive || CategorizeChestsModule.Instance.IsActive) &&
            !ModEntry.Config.HideSideTab)
        {
            LockButton.Draw(b);
        }
        else drawTooltip = false;

        if (!EditMode && _count == 0)
            return;

        // only draw the tooltip for about 1s
        _count++;
        if (_count > 60 && !EditMode)
            Tooltip = null;

        // draw black overlay
        if (EditMode)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height),
                Color.Black * 0.33f);
        }

        // draw item frames in player inventory
        var playerSlots = playerInventory.GetSlotDrawPositions();
        for (var i = 0; i < Game1.player.Items.Count; i++)
        {
            var item = Game1.player.Items[i];
            if (item is null || !item.LockedInInventory())
                continue;
            _lockFrame.Draw(b, new Rectangle((int)playerSlots[i].X, (int)playerSlots[i].Y, 64, 64));
        }

        // draw item frames in chest inventory
        if (chestInventory is not null)
        {
            var chestSlots = chestInventory.GetSlotDrawPositions();
            var items = chestInventory.actualInventory;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item is null || !item.LockedInInventory())
                    continue;
                _lockFrame.Draw(b, new Rectangle((int)chestSlots[i].X, (int)chestSlots[i].Y, 64, 64));
            }
        }

        if (drawTooltip)
            Tooltip?.Draw(b);
    }

    public bool ReceiveLeftClick(int x, int y, InventoryMenu playerInventory, InventoryMenu? chestInventory = null)
    {
        if ((StashToChestsModule.Instance.IsActive || CategorizeChestsModule.Instance.IsActive) &&
            LockButton.Contains(x, y))
        {
            return LockButton.ReceiveLeftClick(x, y);
        }

        if (!EditMode)
            return false;

        var clickPos1 = playerInventory.getInventoryPositionOfClick(x, y);
        if (clickPos1 >= 0 && clickPos1 < Game1.player.Items.Count)
        {
            var item1 = Game1.player.Items[clickPos1];
            item1?.ChangeLockStatus();
        }

        if (chestInventory is not null)
        {
            var clickPos2 = chestInventory.getInventoryPositionOfClick(x, y);
            var items = chestInventory.actualInventory;
            if (clickPos2 >= 0 && clickPos2 < items.Count)
            {
                var item2 = chestInventory.actualInventory[clickPos2];
                item2?.ChangeLockStatus();
            }
        }

        return true;
    }

    public bool ReceiveCursorHover(int x, int y)
    {
        if (EditMode)
        {
            Tooltip = _editTooltip;
            return true;
        }

        Tooltip = null;
        if (!StashToChestsModule.Instance.IsActive && !CategorizeChestsModule.Instance.IsActive)
            return false;

        if (LockButton.Contains(x, y))
        {
            // only draw for about 1s
            if (_count < 60)
            {
                _count++;
                Tooltip = LockButton.Tooltip;
            }
            else
                Tooltip = null;

            return true;
        }

        _count = 0;
        return false;
    }

    public bool ReceiveKeyPress(Keys key)
    {
        if (EditMode && key == Keys.Escape)
        {
            EditMode = false;
            return true;
        }

        return false;
    }
}