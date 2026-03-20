using Microsoft.Xna.Framework;
using UI.Component;

namespace ConvenientChests.CategorizeChests.UI.SubMenus;

internal sealed class DoubleConfirmSubMenu : SubMenu
{
    public DoubleConfirmSubMenu(CategoryChestMenu parent, string hint)
        : base(320, 160, parent)
    {
        var confirmLabel = new TextLabel(hint, Color.Black, Game1.smallFont);
        confirmLabel.SetInCenterOfTheBounds(Bounds);
        confirmLabel.OffsetPosition(y: Y - confirmLabel.Y + 32);

        OkButton.Background = UIHelper.RedButtonBackground(OkButton.Bounds);
        SetOkButtonSound("trashcan");
        Components.Add(confirmLabel);
    }
}