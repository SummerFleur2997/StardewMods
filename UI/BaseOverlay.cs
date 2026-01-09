#nullable enable
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using Rectangle = xTile.Dimensions.Rectangle;

// This code is copied and modified from Pathoschild.Stardew.Common.UI
// in https://github.com/Pathoschild/StardewMods, available under the
// MIT License. See that repository for the latest version.

namespace UI;

/// <summary>An interface which supports user interaction and overlays the active menu (if any).</summary>
internal abstract class BaseOverlay : IDisposable
{
    /*********
     ** Fields
     *********/
    /// <summary>The SMAPI events available for mods.</summary>
    private readonly IModEvents _events;

    /// <summary>An API for checking and changing input state.</summary>
    private readonly IInputHelper _inputHelper;

    /// <summary>Simplifies access to private code.</summary>
    private readonly IReflectionHelper _reflection;

    /// <summary>The screen ID for which the overlay was created, to support split-screen mode.</summary>
    private readonly int _screenId;

    /// <summary>The last viewport bounds.</summary>
    private Rectangle _lastViewport;

    /// <summary>Indicates whether to keep the overlay active. If <c>null</c>, the overlay is kept until explicitly disposed.</summary>
    private readonly Func<bool>? _keepAliveCheck;

    /// <summary>The UI mode to use for pixel coordinates in <see cref="ReceiveLeftClick"/> and <see cref="ReceiveCursorHover"/>, or <c>null</c> to use the current UI mode at the time the event is raised.</summary>
    private readonly bool? _assumeUiMode;


    /*********
     ** Public methods
     *********/
    /// <summary>Release all resources.</summary>
    public virtual void Dispose()
    {
        _events.Display.RenderedActiveMenu -= OnRendered;
        _events.Display.RenderedWorld -= OnRenderedWorld;
        _events.GameLoop.UpdateTicked -= OnUpdateTicked;
        _events.Input.ButtonPressed -= OnButtonPressed;
        _events.Input.ButtonsChanged -= OnButtonsChanged;
        _events.Input.CursorMoved -= OnCursorMoved;
        _events.Input.MouseWheelScrolled -= OnMouseWheelScrolled;
    }


    /*********
     ** Protected methods
     *********/
    /****
     ** Implementation
     ****/
    /// <summary>Construct an instance.</summary>
    /// <param name="events">The SMAPI events available for mods.</param>
    /// <param name="inputHelper">An API for checking and changing input state.</param>
    /// <param name="reflection">Simplifies access to private code.</param>
    /// <param name="keepAlive">Indicates whether to keep the overlay active. If <c>null</c>, the overlay is kept until explicitly disposed.</param>
    /// <param name="assumeUiMode">The UI mode to use for pixel coordinates in <see cref="ReceiveLeftClick"/> and <see cref="ReceiveCursorHover"/>, or <c>null</c> to use the current UI mode at the time the event is raised.</param>
    protected BaseOverlay(IModEvents events, IInputHelper inputHelper, IReflectionHelper reflection,
        Func<bool>? keepAlive = null, bool? assumeUiMode = null)
    {
        _events = events;
        _inputHelper = inputHelper;
        _reflection = reflection;
        _keepAliveCheck = keepAlive;
        _lastViewport = new Rectangle(Game1.uiViewport.X, Game1.uiViewport.Y, Game1.uiViewport.Width,
            Game1.uiViewport.Height);
        _screenId = Context.ScreenId;
        _assumeUiMode = assumeUiMode;

        events.GameLoop.UpdateTicked += OnUpdateTicked;

        if (IsMethodOverridden(nameof(DrawUi)))
            events.Display.RenderedActiveMenu += OnRendered;
        if (IsMethodOverridden(nameof(DrawWorld)))
            events.Display.RenderedWorld += OnRenderedWorld;
        if (IsMethodOverridden(nameof(ReceiveLeftClick)))
            events.Input.ButtonPressed += OnButtonPressed;
        if (IsMethodOverridden(nameof(ReceiveButtonsChanged)))
            events.Input.ButtonsChanged += OnButtonsChanged;
        if (IsMethodOverridden(nameof(ReceiveCursorHover)))
            events.Input.CursorMoved += OnCursorMoved;
        if (IsMethodOverridden(nameof(ReceiveScrollWheelAction)))
            events.Input.MouseWheelScrolled += OnMouseWheelScrolled;
    }

    /// <summary>Update the menu state each tick if needed.</summary>
    protected virtual void Update() { }

    /// <summary>Draw the overlay to the screen over the UI.</summary>
    /// <param name="batch">The sprite batch being drawn.</param>
    protected virtual void DrawUi(SpriteBatch batch) { }

    /// <summary>Draw the overlay to the screen under the UI.</summary>
    /// <param name="batch">The sprite batch being drawn.</param>
    protected virtual void DrawWorld(SpriteBatch batch) { }

    /// <summary>The method invoked when the player left-clicks.</summary>
    /// <param name="x">The X-position of the cursor.</param>
    /// <param name="y">The Y-position of the cursor.</param>
    /// <returns>Whether the event has been handled and shouldn't be propagated further.</returns>
    protected virtual bool ReceiveLeftClick(int x, int y)
    {
        return false;
    }

    /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected virtual void ReceiveButtonsChanged(object? sender, ButtonsChangedEventArgs e) { }

    /// <summary>The method invoked when the player uses the mouse scroll wheel.</summary>
    /// <param name="amount">The scroll amount.</param>
    /// <returns>Whether the event has been handled and shouldn't be propagated further.</returns>
    protected virtual bool ReceiveScrollWheelAction(int amount)
    {
        return false;
    }

    /// <summary>The method invoked when the cursor is hovered.</summary>
    /// <param name="x">The cursor's X position.</param>
    /// <param name="y">The cursor's Y position.</param>
    /// <returns>Whether the event has been handled and shouldn't be propagated further.</returns>
    protected virtual bool ReceiveCursorHover(int x, int y)
    {
        return false;
    }

    /// <summary>The method invoked when the player resizes the game window.</summary>
    protected virtual void ReceiveGameWindowResized() { }

    /****
     ** Helpers
     ****/
    /// <summary>Draw the mouse cursor.</summary>
    /// <remarks>Derived from <see cref="StardewValley.Menus.IClickableMenu.drawMouse"/>.</remarks>
    protected void DrawCursor()
    {
        if (Game1.options.hardwareCursor)
            return;

        var cursorPos = new Vector2(Game1.getMouseX(), Game1.getMouseY());
        if (Constants.TargetPlatform == GamePlatform.Android)
            cursorPos *= Game1.options.zoomLevel /
                         _reflection.GetProperty<float>(typeof(Game1), "NativeZoomLevel").GetValue();

        Game1.spriteBatch.Draw(Game1.mouseCursors, cursorPos,
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16),
            Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero,
            Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
    }

    /****
     ** Event listeners
     ****/
    /// <inheritdoc cref="IDisplayEvents.Rendered"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnRendered(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (Context.ScreenId != _screenId)
            return;

        DrawUi(Game1.spriteBatch);
    }

    /// <inheritdoc cref="IDisplayEvents.RenderedWorld"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
    {
        if (Context.ScreenId != _screenId)
            return;

        DrawWorld(e.SpriteBatch);
    }

    /// <inheritdoc cref="IGameLoopEvents.UpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Context.ScreenId == _screenId)
        {
            // detect end of life
            if (_keepAliveCheck != null && !_keepAliveCheck())
            {
                Dispose();
                return;
            }

            // trigger window resize event
            var newViewport = Game1.uiViewport;
            if (_lastViewport.Width != newViewport.Width || _lastViewport.Height != newViewport.Height)
            {
                newViewport = new Rectangle(newViewport.X, newViewport.Y, newViewport.Width, newViewport.Height);
                ReceiveGameWindowResized();
                _lastViewport = newViewport;
            }
        }
        else if (!Context.HasScreenId(_screenId))
        {
            Dispose();
        }
    }

    /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Context.ScreenId != _screenId)
            return;

        ReceiveButtonsChanged(sender, e);
    }

    /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (Context.ScreenId != _screenId)
            return;

        if (e.Button != SButton.MouseLeft && !e.Button.IsUseToolButton())
            return;

        var uiMode = _assumeUiMode ?? Game1.uiMode;
        bool handled;
        if (Constants.TargetPlatform == GamePlatform.Android)
        {
            var nativeZoomLevel = _reflection.GetProperty<float>(typeof(Game1), "NativeZoomLevel").GetValue();
            handled = ReceiveLeftClick((int)(Game1.getMouseX() * Game1.options.zoomLevel / nativeZoomLevel),
                (int)(Game1.getMouseY() * Game1.options.zoomLevel / nativeZoomLevel));
        }
        else
        {
            handled = ReceiveLeftClick(Game1.getMouseX(uiMode), Game1.getMouseY(uiMode));
        }

        if (handled)
            _inputHelper.Suppress(e.Button);
    }

    /// <inheritdoc cref="IInputEvents.MouseWheelScrolled"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        if (Context.ScreenId != _screenId)
            return;

        var scrollHandled = ReceiveScrollWheelAction(e.Delta);
        if (scrollHandled)
        {
            var cur = Game1.oldMouseState;
            Game1.oldMouseState = new MouseState(
                cur.X,
                cur.Y,
                e.NewValue,
                cur.LeftButton,
                cur.MiddleButton,
                cur.RightButton,
                cur.XButton1,
                cur.XButton2
            );
        }
    }

    /// <inheritdoc cref="IInputEvents.CursorMoved"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnCursorMoved(object? sender, CursorMovedEventArgs e)
    {
        if (Context.ScreenId != _screenId)
            return;

        var uiMode = _assumeUiMode ?? Game1.uiMode;
        var hoverHandled = ReceiveCursorHover(Game1.getMouseX(uiMode), Game1.getMouseY(uiMode));
        if (hoverHandled)
            Game1.InvalidateOldMouseMovement();
    }

    /// <summary>Get whether a method has been overridden by a subclass.</summary>
    /// <param name="name">The method name.</param>
    private bool IsMethodOverridden(string name)
    {
        var method = GetType()
            .GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (method == null)
            throw new InvalidOperationException($"Can't find method {GetType().FullName}.{name}.");

        return method.DeclaringType != typeof(BaseOverlay);
    }
}