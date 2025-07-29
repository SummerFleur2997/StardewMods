#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

// This code is copied and modified from Pathoschild.Stardew.Common.UI
// in https://github.com/Pathoschild/StardewMods, available under the
// MIT License. See that repository for the latest version.

namespace UI.UserInterface;

/// <summary>A button UI component which lets the player trigger a dropdown list.</summary>
internal class Dropdown<TItem> : ClickableComponent
{
    /// <summary>The width of the borders drawn by <see cref="DrawTab"/>.</summary>
    private const int ButtonBorderWidth = 4 * Game1.pixelZoom;

    /// <summary>The size of the rendered button borders.</summary>
    private readonly int _borderWidth = Sprites.TabBackground.TopLeft.Width * 2 * Game1.pixelZoom;

    /*********
     ** Fields
     *********/
    /// <summary>The font with which to render text.</summary>
    private readonly SpriteFont _font;

    /// <summary>The dropdown list.</summary>
    private readonly DropdownList<TItem> _list;

    /// <summary>The maximum width in pixels for the dropdown label.</summary>
    private readonly int? _maxLabelWidth;

    /// <summary>The display label for the selected value.</summary>
    private string? _displayLabel;

    /// <summary>The backing field for <see cref="IsExpanded"/>.</summary>
    private bool _isExpandedImpl;


    /*********
     ** Public methods
     *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="x">The X-position at which to draw the tab.</param>
    /// <param name="y">The Y-position at which to draw the tab.</param>
    /// <param name="font">The font with which to render text.</param>
    /// <param name="selectedItem">The selected item.</param>
    /// <param name="items">The items in the list.</param>
    /// <param name="getLabel">Get the display label for an item.</param>
    /// <param name="maxLabelWidth">The maximum width in pixels for the dropdown label.</param>
    public Dropdown(int x, int y, SpriteFont font, TItem? selectedItem, TItem[] items, Func<TItem, string> getLabel,
        int? maxLabelWidth = null)
        : base(Rectangle.Empty, selectedItem != null ? getLabel(selectedItem) : string.Empty)
    {
        _font = font;
        _list = new DropdownList<TItem>(selectedItem, items, getLabel, x, y, font);
        bounds.X = x;
        bounds.Y = y;
        _maxLabelWidth = maxLabelWidth;

        ReinitializeComponents();
        OnValueSelected();
    }

    /// <summary>Whether the menu is being displayed on Android.</summary>
    private bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;


    /*********
     ** Accessors
     *********/
    /// <summary>Whether the dropdown list is expanded.</summary>
    public bool IsExpanded
    {
        get => _isExpandedImpl;
        set
        {
            _isExpandedImpl = value;
            downNeighborID = value
                ? _list.TopComponentId
                : DefaultDownNeighborId;
        }
    }

    /// <summary>The selected option.</summary>
    public TItem Selected => _list.SelectedValue;

    /// <summary>The downward neighbor ID when the dropdown is closed for controller snapping.</summary>
    public int DefaultDownNeighborId { get; set; } = -99999;

    /// <inheritdoc />
    public override bool containsPoint(int x, int y)
    {
        return
            base.containsPoint(x, y)
            || (IsExpanded && _list.containsPoint(x, y));
    }

    /// <summary>Handle a click at the given position, if applicable.</summary>
    /// <param name="x">The X-position that was clicked.</param>
    /// <param name="y">The Y-position that was clicked.</param>
    /// <returns>Returns whether the click was handled.</returns>
    public bool TryClick(int x, int y)
    {
        return TryClick(x, y, out _, out _);
    }

    /// <summary>Handle a click at the given position, if applicable.</summary>
    /// <param name="x">The X-position that was clicked.</param>
    /// <param name="y">The Y-position that was clicked.</param>
    /// <param name="itemClicked">Whether a dropdown item was clicked.</param>
    /// <param name="dropdownToggled">Whether the dropdown was expanded or collapsed.</param>
    /// <returns>Returns whether the click was handled.</returns>
    public bool TryClick(int x, int y, out bool itemClicked, out bool dropdownToggled)
    {
        itemClicked = false;
        dropdownToggled = false;

        // click dropdown item
        if (IsExpanded && _list.TryClick(x, y, out itemClicked))
        {
            if (itemClicked)
            {
                IsExpanded = false;
                dropdownToggled = true;

                OnValueSelected();
            }

            return true;
        }

        // toggle expansion
        if (bounds.Contains(x, y) || IsExpanded)
        {
            IsExpanded = !IsExpanded;
            dropdownToggled = true;
            return true;
        }

        // not handled
        return false;
    }

    /// <summary>Select an item in the list matching the given value.</summary>
    /// <param name="value">The value to search.</param>
    /// <returns>Returns whether an item was selected.</returns>
    public bool TrySelect(TItem value)
    {
        return _list.TrySelect(value);
    }

    /// <summary>A method invoked when the player scrolls the dropdown using the mouse wheel.</summary>
    /// <param name="direction">The scroll direction.</param>
    public void ReceiveScrollWheelAction(int direction)
    {
        if (IsExpanded)
            _list.ReceiveScrollWheelAction(direction);
    }

    /// <summary>Render the tab UI.</summary>
    /// <param name="sprites">The sprites to render.</param>
    /// <param name="opacity">The opacity at which to draw.</param>
    public void Draw(SpriteBatch sprites, float opacity = 1)
    {
        // get selected label
        DrawTab(sprites, bounds.X, bounds.Y, bounds.Width, _list.MaxLabelHeight, out var textPos,
            drawShadow: IsAndroid);
        sprites.DrawString(_font, _displayLabel, textPos, Color.Black * opacity);

        // draw dropdown
        if (IsExpanded)
            _list.Draw(sprites, opacity);
    }

    /// <summary>Recalculate dimensions and components for rendering.</summary>
    public void ReinitializeComponents()
    {
        bounds.Height =
            (int)_font.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ").Y - 10 +
            _borderWidth; // adjust for font's broken measurement
        bounds.Width = _list.MaxLabelWidth + _borderWidth;

        if (bounds.Width > _maxLabelWidth)
            bounds.Width = _maxLabelWidth.Value;

        _list.bounds.X = bounds.X;
        _list.bounds.Y = bounds.Bottom;

        _list.ReinitializeComponents();
        ReinitializeControllerFlow();
    }

    /// <summary>Set the fields to support controller snapping.</summary>
    public void ReinitializeControllerFlow()
    {
        _list.ReinitializeControllerFlow();
        IsExpanded = IsExpanded; // force-update down ID
    }

    /// <summary>Get the nested components for controller snapping.</summary>
    public IEnumerable<ClickableComponent> GetChildComponents()
    {
        return _list.GetChildComponents();
    }


    /*********
     ** Private methods
     *********/
    /// <summary>Handle a dropdown value being selected.</summary>
    private void OnValueSelected()
    {
        var displayLabel = _list.SelectedLabel;

        if (_maxLabelWidth.HasValue && _font.MeasureString(displayLabel).X > _maxLabelWidth)
        {
            // this is inefficient, but it only runs when the player selects an unusually long value
            const string ellipsis = "...";
            var ellipsisWidth = _font.MeasureString(ellipsis).X;
            var maxWidth = _maxLabelWidth.Value - (int)ellipsisWidth;

            var truncated = false;
            while (displayLabel.Length > 10 && _font.MeasureString(displayLabel).X > maxWidth)
            {
                displayLabel = displayLabel[..^1];
                truncated = true;
            }

            if (truncated)
                displayLabel += ellipsis;
        }

        _displayLabel = displayLabel;
    }

    /// <summary>Draw a tab texture to the screen.</summary>
    /// <param name="spriteBatch">The sprite batch to which to draw.</param>
    /// <param name="x">The X position at which to draw.</param>
    /// <param name="y">The Y position at which to draw.</param>
    /// <param name="innerWidth">The width of the button's inner content.</param>
    /// <param name="innerHeight">The height of the button's inner content.</param>
    /// <param name="innerDrawPosition">The position at which the content should be drawn.</param>
    /// <param name="align">The button's horizontal alignment relative to <paramref name="x"/>. The possible values are 0 (left), 1 (center), or 2 (right).</param>
    /// <param name="alpha">The button opacity, as a value from 0 (transparent) to 1 (opaque).</param>
    /// <param name="forIcon">Whether the button will contain an icon instead of text.</param>
    /// <param name="drawShadow">Whether to draw a shadow under the tab.</param>
    public static void DrawTab(SpriteBatch spriteBatch, int x, int y, int innerWidth, int innerHeight,
        out Vector2 innerDrawPosition, int align = 0, float alpha = 1, bool forIcon = false, bool drawShadow = true)
    {
        // calculate outer coordinates
        var outerWidth = innerWidth + ButtonBorderWidth;
        var outerHeight = innerHeight + Game1.tileSize / 3;
        var offsetX = align switch
        {
            1 => -outerWidth / 2,
            2 => -outerWidth,
            _ => 0
        };

        // calculate inner coordinates
        {
            var iconOffsetX = forIcon ? -Game1.pixelZoom : 0;
            var iconOffsetY = forIcon ? 2 * -Game1.pixelZoom : 0;
            innerDrawPosition = new Vector2(x + ButtonBorderWidth + offsetX + iconOffsetX,
                y + ButtonBorderWidth + iconOffsetY);
        }

        // draw texture
        IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y,
            outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
    }
}