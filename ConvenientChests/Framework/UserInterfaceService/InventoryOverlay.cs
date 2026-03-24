using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using UI.Component;
using UI.Sprite;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class InventoryOverlay : IOverlay<GameMenu>
{
    public GameMenu RootMenu { get; }
    public Tooltip? Tooltip { get; private set; }

    private bool EditMode
    {
        get => _editMode;
        set
        {
            if (_editMode == value)
                return;

            LockButton.Texture = value ? _okSprite : _lockSprite;
            _editMode = value;
        }
    }

    private bool _editMode;
    private SpriteButton LockButton { get; }
    private readonly TextureRegion _lockSprite;
    private readonly TextureRegion _okSprite;

    public InventoryOverlay(GameMenu menu)
    {
        RootMenu = menu;
        LockButton = UIHelper.SideButton(0, 0, SideButtonVariant.Lock);

        _lockSprite = LockButton.Texture;
        _okSprite = new TextureRegion(Game1.mouseCursors, 128, 256, 64, 64);

        // var delta = ModEntry.IsAndroid ? 100 + ModEntry.Config.MobileOffset : 106;
        LockButton.SetPosition(RootMenu.xPositionOnScreen - LockButton.Width - 16, RootMenu.yPositionOnScreen + 128);
        LockButton.OnPress += () => EditMode = !EditMode;
    }

    public void DrawUi(SpriteBatch b)
    {
        if (ModEntry.Config.HideSideTab)
            return;

        if (ModEntry.StashModule.IsActive || ModEntry.CategorizeModule.IsActive)
            LockButton.Draw(b);

        if (EditMode)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(RootMenu.xPositionOnScreen, RootMenu.yPositionOnScreen,
                RootMenu.width, RootMenu.height), Color.Black * 0.5f);
        }

        Tooltip?.Draw(b);
        RootMenu.drawMouse(b);
    }

    public bool ReceiveLeftClick(int x, int y)
    {
        if ((ModEntry.StashModule.IsActive || ModEntry.CategorizeModule.IsActive) && LockButton.Contains(x, y))
            return LockButton.ReceiveLeftClick(x, y);
        if (EditMode)
            return RootMenu.isWithinBounds(x, y);
        return false;
    }

    public bool ReceiveCursorHover(int x, int y)
    {
        if (!ModEntry.StashModule.IsActive && !ModEntry.CategorizeModule.IsActive)
            return false;

        if (EditMode)
            return true;

        if (LockButton.Contains(x, y))
        {
            Tooltip = LockButton.Tooltip;
            return true;
        }

        return false;
    }
}