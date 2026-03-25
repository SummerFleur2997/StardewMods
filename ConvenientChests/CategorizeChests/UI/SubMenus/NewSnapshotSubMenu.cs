using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UI.Component;

namespace ConvenientChests.CategorizeChests.UI.SubMenus;

internal class NewSnapshotSubMenu : SubMenu, IHaveParentMenu
{
    /// <inheritdoc />
    public IHaveSubMenu Parent { get; set; }

    /// <summary>
    /// The text box used for input the name of new snapshot.
    /// </summary>
    public readonly TextBox TextBox;

    public NewSnapshotSubMenu(IHaveSubMenu parent) : base(320, 240)
    {
        Parent = parent;

        var y = Y + 24;

        // init title label
        var textLabel = new TextLabel(I18n.UI_ChestAlias(), Color.Black, Game1.smallFont);
        textLabel.SetInCenterOfTheBounds(Bounds);
        textLabel.Y = y;
        Components.Add(textLabel);

        // init item button and textbox
        y += textLabel.Height + 16;
        TextBox = new TextBox(X + 20, y, 280, 64, "", UIHelper.TextBubble());
        Game1.keyboardDispatcher.Subscriber = TextBox;
        Components.Add(TextBox);
    }

    /// <inheritdoc/>
    public override bool ReceiveKeyPress(Keys key)
    {
        if (!TextBox.Selected)
        {
            switch (key)
            {
                case Keys.Escape:
                    TextBox.Selected = false;
                    Game1.playSound("bigDeSelect");
                    return true;
                case Keys.Enter:
                    TextBox.Selected = false;
                    break; // fall back to base method to handle enter key
                default:
                    return true;
            }
        }

        return base.ReceiveKeyPress(key);
    }

    public override void Dispose()
    {
        var parentMenu = Parent;
        Parent = null!;
        parentMenu.SubMenu = null;

        TextBox.Dispose();
        base.Dispose();
    }
}