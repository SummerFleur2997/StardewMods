using ConvenientChests.Framework.UserInterfaceService;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.CategorizeChests.Framework.UI;

internal class SubMenu : IClickableMenu, IComponent
{
    /// <inheritdoc/>
    public Rectangle Bounds => new(X, Y, Width, Height);

    /// <inheritdoc/>
    public int X { get; set; }

    /// <inheritdoc/>
    public int Y { get; set; }

    /// <inheritdoc/>
    public int Width { get; set; }

    /// <inheritdoc/>
    public int Height { get; set; }

    public event Action OnOk;
    public event Action OnCancel;

    public readonly List<IComponent> Components = new();
    public readonly Button OkButton;
    public readonly Button CancelButton;

    private readonly NineSlice _background;

    /// <summary>
    /// The parent menu of this sub-menu.
    /// </summary>
    private IHaveSubMenu _parentMenu;

    public SubMenu(int width, int height, IHaveSubMenu parent, bool showOk = true, bool showCancel = true)
    {
        _parentMenu = parent;

        var screen = new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
        this.SetSize(width, height);
        this.SetInCenterOfTheBounds(screen);

        _background = NineSlice.CommonMenu(Bounds);

        if (showOk)
        {
            var okTexture = UIHelper.YellowButtonBackground();
            OkButton = new Button(okTexture, I18n.UI_Yes(), Color.Black, Game1.smallFont);
            OkButton.SetDestination(X + Width / 2 + 8, Y + Height - 80, 108, 60);
            OkButton.SoundCue = "trashcan";
            OkButton.OnPress += FireOkEvent;
            Components.Add(OkButton);
        }

        if (showCancel)
        {
            var cancelTexture = UIHelper.YellowButtonBackground();
            CancelButton = new Button(cancelTexture, I18n.UI_No(), Color.Black, Game1.smallFont);
            CancelButton.SetDestination(X + Width / 2 - 116, Y + Height - 80, 108, 60);
            CancelButton.SoundCue = "bigDeSelect";
            CancelButton.OnPress += FireCancelEvent;
            Components.Add(CancelButton);
        }
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch b)
    {
        if (!Game1.options.showClearBackgrounds)
            b.Draw(Game1.fadeToBlackRect,
                new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

        _background.Draw(b);
        foreach (var component in Components) component.Draw(b);
    }

    /// <inheritdoc/>
    public virtual bool ReceiveLeftClick(int x, int y)
    {
        // Travel from back to front
        for (var i = Components.Count - 1; i >= 0; i--)
        {
            var component = Components[i];
            if (component is not IClickableComponent c)
                continue;

            if (c.ReceiveLeftClick(x, y))
                return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public virtual bool ReceiveCursorHover(int x, int y) => Bounds.Contains(x, y);

    /// <inheritdoc/>
    public virtual bool ReceiveScrollWheelAction(int amount) => false;

    public void ReceiveKeyPress(Keys key)
    {
        if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
        {
            FireCancelEvent();
            Game1.playSound("bigDeSelect");
            return;
        }

        if (key is Keys.Enter)
        {
            FireOkEvent();
            Game1.playSound("trashcan");
        }
    }

    /// <summary>
    /// Things to do when press ok button to exit this sub-menu.
    /// </summary>
    private void FireOkEvent()
    {
        OnOk?.Invoke();
        Dispose();
    }

    /// <summary>
    /// Things to do when press cancel button to exit this sub-menu.
    /// </summary>
    private void FireCancelEvent()
    {
        OnCancel?.Invoke();
        Dispose();
    }

    public virtual void Dispose()
    {
        var parentMenu = _parentMenu;
        _parentMenu = null;
        parentMenu.SubMenu = null;

        foreach (var component in Components)
            if (component is IDisposable d)
                d.Dispose();

        Components.Clear();

        OnOk = null;
        OnCancel = null;
    }
}

internal interface IHaveSubMenu
{
    SubMenu SubMenu { get; set; }
}