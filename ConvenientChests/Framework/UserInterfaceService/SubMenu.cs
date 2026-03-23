using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UI.Component;
using UI.Menu;

namespace ConvenientChests.Framework.UserInterfaceService;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
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

    public event Action<SubMenu>? OnOk;
    public event Action<SubMenu>? OnCancel;

    public readonly List<IComponent> Components = new();

    public Button OkButton;
    public Button CancelButton;

    private NineSlice _background;
    private string _okButtonCue = "money";
    private string _cancelButtonCue = "bigDeSelect";

    public SubMenu(int width, int height)
    {
        var screen = new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
        this.SetSize(width, height);
        this.SetInCenterOfTheBounds(screen);

        _background = NineSlice.SmallMenuBackground(Bounds);

        var okTexture = UIHelper.YellowButtonBackground();
        OkButton = new Button(okTexture, I18n.UI_Yes(), Color.Black, Game1.smallFont);
        OkButton.SetDestination(X + Width / 2 + 8, Y + Height - 80, 108, 60);
        OkButton.SoundCue = _okButtonCue;
        OkButton.OnPress += FireOkEvent;
        Components.Add(OkButton);

        var cancelTexture = UIHelper.YellowButtonBackground();
        CancelButton = new Button(cancelTexture, I18n.UI_No(), Color.Black, Game1.smallFont);
        CancelButton.SetDestination(X + Width / 2 - 116, Y + Height - 80, 108, 60);
        CancelButton.SoundCue = _cancelButtonCue;
        CancelButton.OnPress += FireCancelEvent;
        Components.Add(CancelButton);
    }

    public void SetOkButtonSound(string cue)
    {
        _okButtonCue = cue;
        OkButton.SoundCue = cue;
    }

    public void SetCancelButtonSound(string cue)
    {
        _cancelButtonCue = cue;
        CancelButton.SoundCue = cue;
    }

    /// <inheritdoc/>
    public virtual void Draw(SpriteBatch b)
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

    /// <inheritdoc cref="StardewValley.Menus.IClickableMenu.receiveKeyPress"/>
    public virtual void ReceiveKeyPress(Keys key)
    {
        if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
        {
            FireCancelEvent();
            Game1.playSound(_cancelButtonCue);
            return;
        }

        if (key is Keys.Enter)
        {
            FireOkEvent();
            Game1.playSound(_okButtonCue);
        }
    }

    /// <summary>
    /// Things to do when press ok button to exit this sub-menu.
    /// </summary>
    private void FireOkEvent()
    {
        OnOk?.Invoke(this);
        Dispose();
    }

    /// <summary>
    /// Things to do when press cancel button to exit this sub-menu.
    /// </summary>
    private void FireCancelEvent()
    {
        OnCancel?.Invoke(this);
        Dispose();
    }

    public virtual void Dispose()
    {
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
    public SubMenu? SubMenu { get; set; }
}

internal interface IHaveParentMenu
{
    public IHaveSubMenu Parent { get; set; }
}