#nullable enable
using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using UI.Component;

namespace ConvenientChests.CategorizeChests.UI.SubMenus;

internal sealed class DoubleConfirmSubMenu : SubMenu, IHaveParentMenu
{
    /// <summary>
    /// The parent menu of this sub-menu.
    /// </summary>
    public IHaveSubMenu Parent { get; set; }

    public DoubleConfirmSubMenu(IHaveSubMenu parent, string hint)
        : base(320, 160)
    {
        Parent = parent;

        var confirmLabel = new TextLabel(hint, Color.Black, Game1.smallFont);
        confirmLabel.SetInCenterOfTheBounds(Bounds);
        confirmLabel.OffsetPosition(y: Y - confirmLabel.Y + 32);

        OkButton.Background = UIHelper.RedButtonBackground(OkButton.Bounds);
        SetOkButtonSound("trashcan");
        Components.Add(confirmLabel);
    }

    public override void Dispose()
    {
        var parentMenu = Parent;
        Parent = null!;
        parentMenu.SubMenu = null;

        base.Dispose();
    }
}