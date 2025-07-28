#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using UI.UserInterface;

// This code is copied and modified from Pathoschild.Stardew.Common.UI
// in https://github.com/Pathoschild/StardewMods, available under the
// MIT License. See that repository for the latest version.

namespace UI;

/// <summary>A dropdown UI component which lets the player choose from a list of values.</summary>
/// <typeparam name="TValue">The item value type.</typeparam>
internal class DropdownList<TValue> : ClickableComponent
{
    /*********
    ** Fields
    *********/
    /****
    ** Constants
    ****/
    /// <summary>The padding applied to dropdown lists.</summary>
    private const int DropdownPadding = 5;

    /****
    ** Items
    ****/
    /// <summary>The selected option.</summary>
    private DropListOption SelectedOption;

    /// <summary>The options in the list.</summary>
    private readonly DropListOption[] Options;

    /// <summary>The item index shown at the top of the list.</summary>
    private int FirstVisibleIndex;

    /// <summary>The maximum items to display.</summary>
    private int MaxItems;

    /// <summary>The item index shown at the bottom of the list.</summary>
    private int LastVisibleIndex => FirstVisibleIndex + MaxItems - 1;

    /// <summary>The maximum index that can be shown at the top of the list.</summary>
    private int MaxFirstVisibleIndex => Options.Length - MaxItems;

    /// <summary>Whether the player can scroll up in the list.</summary>
    private bool CanScrollUp => FirstVisibleIndex > 0;

    /// <summary>Whether the player can scroll down in the list.</summary>
    private bool CanScrollDown => FirstVisibleIndex < MaxFirstVisibleIndex;


    /****
    ** Rendering
    ****/
    /// <summary>The font with which to render text.</summary>
    private readonly SpriteFont Font;

    /// <summary>The up arrow to scroll results.</summary>
    private ClickableTextureComponent UpArrow;

    /// <summary>The bottom arrow to scroll results.</summary>
    private ClickableTextureComponent DownArrow;


    /*********
    ** Accessors
    *********/
    /// <summary>The selected value.</summary>
    public TValue SelectedValue => SelectedOption.Value;

    /// <summary>The display label for the selected value.</summary>
    public string SelectedLabel => SelectedOption.label;

    /// <summary>The maximum height for the possible labels.</summary>
    public int MaxLabelHeight { get; }

    /// <summary>The maximum width for the possible labels.</summary>
    public int MaxLabelWidth { get; private set; }

    /// <summary>The <see cref="ClickableComponent.myID"/> value for the top entry in the dropdown.</summary>
    public int TopComponentId => Options.First(p => p.visible).myID;


    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="selectedValue">The selected value.</param>
    /// <param name="items">The items in the list.</param>
    /// <param name="getLabel">Get the display label for an item.</param>
    /// <param name="x">The X-position from which to render the list.</param>
    /// <param name="y">The Y-position from which to render the list.</param>
    /// <param name="font">The font with which to render text.</param>
    public DropdownList(TValue? selectedValue, TValue[] items, Func<TValue, string> getLabel, int x, int y, SpriteFont font)
        : base(new Rectangle(), nameof(DropdownList<TValue>))
    {
        // save values
        Options = items
            .Select((value, index) => new DropListOption(Rectangle.Empty, index, getLabel(value), value, font))
            .ToArray();
        Font = font;
        MaxLabelHeight = Options.Max(p => p.LabelHeight);

        // set initial selection
        int selectedIndex = Array.IndexOf(items, selectedValue);
        SelectedOption = selectedIndex >= 0
            ? Options[selectedIndex]
            : Options.First();

        // initialize UI
        bounds.X = x;
        bounds.Y = y;
        ReinitializeComponents();
    }

    /// <summary>A method invoked when the player scrolls the dropdown using the mouse wheel.</summary>
    /// <param name="direction">The scroll direction.</param>
    public void ReceiveScrollWheelAction(int direction)
    {
        Scroll(direction > 0 ? -1 : 1); // scrolling down moves first item up
    }

    /// <summary>Handle a click at the given position, if applicable.</summary>
    /// <param name="x">The X-position that was clicked.</param>
    /// <param name="y">The Y-position that was clicked.</param>
    /// <param name="itemClicked">Whether a dropdown item was clicked.</param>
    /// <returns>Returns whether the click was handled.</returns>
    public bool TryClick(int x, int y, out bool itemClicked)
    {
        // dropdown value
        DropListOption? option = Options.FirstOrDefault(p => p.visible && p.containsPoint(x, y));
        if (option != null)
        {
            SelectedOption = option;
            itemClicked = true;
            return true;
        }
        itemClicked = false;

        // arrows
        if (UpArrow.containsPoint(x, y))
        {
            Scroll(-1);
            return true;
        }
        if (DownArrow.containsPoint(x, y))
        {
            Scroll(1);
            return true;
        }

        return false;
    }

    /// <summary>Select an item in the list matching the given value.</summary>
    /// <param name="value">The value to search.</param>
    /// <returns>Returns whether an item was selected.</returns>
    public bool TrySelect(TValue value)
    {
        DropListOption? entry = Options.FirstOrDefault(p =>
            (p.Value == null && value == null)
            || p.Value?.Equals(value) == true
        );

        if (entry == null)
            return false;

        SelectedOption = entry;
        return true;
    }

    /// <inheritdoc />
    public override bool containsPoint(int x, int y)
    {
        return
            base.containsPoint(x, y)
            || UpArrow.containsPoint(x, y)
            || DownArrow.containsPoint(x, y);
    }

    /// <summary>Render the UI.</summary>
    /// <param name="sprites">The sprites to render.</param>
    /// <param name="opacity">The opacity at which to draw.</param>
    public void Draw(SpriteBatch sprites, float opacity = 1)
    {
        // draw dropdown items
        foreach (DropListOption option in Options)
        {
            if (!option.visible)
                continue;

            if (option.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                sprites.Draw(Sprites.HoverBackground, option.bounds, Color.White * opacity);
            else if (option.Index == SelectedOption.Index)
                sprites.Draw(Sprites.ActiveBackground, option.bounds, Color.White * opacity);
            else
                sprites.Draw(Sprites.InactiveBackground, option.bounds, Color.White * opacity);

            // draw text
            var position = new Vector2(option.bounds.X + DropdownPadding, option.bounds.Y + (int)(Game1.tileSize / 16f));
            sprites.DrawString(Font, option.label, position, Color.Black * opacity);
        }

        // draw up/down arrows
        if (CanScrollUp)
            UpArrow.draw(sprites, Color.White * opacity, 1);
        if (CanScrollDown)
            DownArrow.draw(sprites, Color.White * opacity, 1);
    }

    /// <summary>Recalculate dimensions and components for rendering.</summary>
    [MemberNotNull(nameof(UpArrow), nameof(DownArrow))]
    public void ReinitializeComponents()
    {
        int x = bounds.X;
        int y = bounds.Y;

        // get item size
        int itemWidth = MaxLabelWidth = Math.Max(Options.Max(p => p.LabelWidth), Game1.tileSize * 2) + DropdownPadding * 2;
        int itemHeight = MaxLabelHeight;

        // get pagination
        MaxItems = Math.Min((Game1.uiViewport.Height - y) / itemHeight, Options.Length);
        FirstVisibleIndex = GetValidFirstItem(FirstVisibleIndex, MaxFirstVisibleIndex);

        // get dropdown size
        bounds.Width = itemWidth;
        bounds.Height = itemHeight * MaxItems;

        // update components
        {
            int itemY = y;
            foreach (DropListOption option in Options)
            {
                option.visible = option.Index >= FirstVisibleIndex && option.Index <= LastVisibleIndex;
                if (option.visible)
                {
                    option.bounds = new Rectangle(x, itemY, itemWidth, itemHeight);
                    itemY += itemHeight;
                }
            }
        }

        // add arrows
        {
            Rectangle upSource = Sprites.UpArrow.Region;
            Rectangle downSource = Sprites.DownArrow.Region;

            UpArrow = new ClickableTextureComponent(
                "up-arrow", 
                new Rectangle(x - upSource.Width, y, upSource.Width, upSource.Height), 
                "", "", Sprites.CursorSheet, upSource, 1);
            DownArrow = new ClickableTextureComponent(
                "down-arrow", 
                new Rectangle(x - downSource.Width, y + bounds.Height - downSource.Height, downSource.Width, downSource.Height), 
                "", "",Sprites.CursorSheet, downSource, 1);
        }

        // update controller flow
        ReinitializeControllerFlow();
    }

    /// <summary>Set the fields to support controller snapping.</summary>
    public void ReinitializeControllerFlow()
    {
        int firstIndex = FirstVisibleIndex;
        int lastIndex = LastVisibleIndex;

        int initialId = 1_100_000;
        foreach (DropListOption option in Options)
        {
            int index = option.Index;
            int id = initialId + index;

            option.myID = id;
            option.upNeighborID = index > firstIndex
                ? id - 1
                : -99999;
            option.downNeighborID = index < lastIndex
                ? id + 1
                : -1;
        }
    }

    /// <summary>Get the nested components for controller snapping.</summary>
    public IEnumerable<ClickableComponent> GetChildComponents()
    {
        return Options;
    }


    /*********
    ** Private methods
    *********/
    /// <summary>Scroll the dropdown by the specified amount.</summary>
    /// <param name="amount">The number of items to scroll.</param>
    private void Scroll(int amount)
    {
        // recalculate first item
        int firstItem = GetValidFirstItem(FirstVisibleIndex + amount, MaxFirstVisibleIndex);
        if (firstItem == FirstVisibleIndex)
            return;
        FirstVisibleIndex = firstItem;

        // update displayed items
        ReinitializeComponents();
    }

    /// <summary>Calculate a valid index for the first displayed item in the list.</summary>
    /// <param name="value">The initial value, which may not be valid.</param>
    /// <param name="maxIndex">The maximum first index.</param>
    private int GetValidFirstItem(int value, int maxIndex)
    {
        return Math.Max(Math.Min(value, maxIndex), 0);
    }


    /*********
    ** Private models
    *********/
    /// <summary>A clickable option in the dropdown.</summary>
    private class DropListOption : ClickableComponent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The option's index in the list.</summary>
        public int Index { get; }

        /// <summary>The option value.</summary>
        public TValue Value { get; }

        /// <summary>The label text width in pixels.</summary>
        public int LabelWidth { get; }

        /// <summary>The label text height in pixels.</summary>
        public int LabelHeight { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="bounds">The pixel bounds on screen.</param>
        /// <param name="index">The option's index in the list.</param>
        /// <param name="label">The display text.</param>
        /// <param name="value">The option value.</param>
        /// <param name="font">The font with which to measure the label.</param>
        public DropListOption(Rectangle bounds, int index, string label, TValue value, SpriteFont font)
            : base(bounds: bounds, name: index.ToString(), label: label)
        {
            Index = index;
            Value = value;

            Vector2 labelSize = font.MeasureString(label);
            LabelWidth = (int)labelSize.X;
            LabelHeight = (int)labelSize.Y;
        }
    }
}
