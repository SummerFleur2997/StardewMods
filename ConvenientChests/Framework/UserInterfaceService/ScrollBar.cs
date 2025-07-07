using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using UI.UserInterface;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class ScrollBar : Widget
{
    private ScrollBarRunner Runner { get; }

    private int _scrollPosition;

    public int ScrollPosition
    {
        get => _scrollPosition;
        set
        {
            _scrollPosition = value;
            UpdateScroller();
        }
    }


    private int _scrollMax;

    public int ScrollMax
    {
        get => _scrollMax;
        set
        {
            _scrollMax = value;
            UpdateScroller();
        }
    }

    public int Step { get; set; } = 1;

    public bool Visible { get; set; } = true;

    private SpriteButton ScrollUpButton { get; }
    private SpriteButton ScrollDownButton { get; }
    private Rectangle _scrollBackground;

    public ScrollBar()
    {
        ScrollUpButton = new SpriteButton(Sprites.UpArrow);
        ScrollDownButton = new SpriteButton(Sprites.DownArrow);
        Runner = new ScrollBarRunner { Width = 24 };

        AddChild(ScrollUpButton);
        AddChild(ScrollDownButton);
        AddChild(Runner);

        PositionElements();

        ScrollUpButton.OnPress += () => Scroll(-1);
        ScrollDownButton.OnPress += () => Scroll(+1);

        ModEntry.ModHelper.Events.Input.ButtonReleased += InputOnButtonReleased;
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked += GameLoopOnUpdateTicked;
    }

    public override void Dispose()
    {
        ModEntry.ModHelper.Events.Input.ButtonReleased -= InputOnButtonReleased;
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked -= GameLoopOnUpdateTicked;

        base.Dispose();
    }

    protected override void OnDimensionsChanged()
    {
        if (Width != 64)
        {
            Width = 64;
            return;
        }

        base.OnDimensionsChanged();
        PositionElements();
    }

    private void PositionElements()
    {
        if (ScrollDownButton == null)
            return;

        ScrollUpButton.X = 0;
        ScrollDownButton.X = 0;
        ScrollDownButton.Y = Height - ScrollDownButton.Height;

        _scrollBackground.X = 20;
        _scrollBackground.Y = ScrollUpButton.Height - 4;
        _scrollBackground.Height = Height - ScrollUpButton.Height - ScrollDownButton.Height + 8;
        _scrollBackground.Width = Runner.Width;
        Runner.X = _scrollBackground.X;


        _scrollBackground.Location = Globalize(_scrollBackground.Location);
        UpdateScroller();
    }

    private void UpdateScroller()
    {
        if (Step == 0)
            return;

        Runner.Height = (int)(_scrollBackground.Height * (Step / (float)ScrollMax));
        Runner.Y = 60 + (int)((_scrollBackground.Height - Runner.Height) *
                              Math.Min(1, ScrollPosition / (float)(ScrollMax - Step)));
    }


    public override void Draw(SpriteBatch batch)
    {
        if (!Visible)
            return;

        // draw background
        IClickableMenu.drawTextureBox(batch, Game1.mouseCursors,
            new Rectangle(403, 383, 6, 6),
            _scrollBackground.X, _scrollBackground.Y, _scrollBackground.Width, _scrollBackground.Height,
            Color.White, 4f, false);

        base.Draw(batch);
    }

    public void Scroll(int direction)
    {
        if (ScrollMax == 0)
            return;

        ScrollPosition = Math.Max(0, Math.Min(ScrollMax, ScrollPosition + direction * Step));
        OnScroll?.Invoke(this, new ScrollBarEventArgs(ScrollPosition));
        UpdateScroller();
    }


    private bool _scrolling;

    public override bool ReceiveLeftClick(Point point)
    {
        var localPoint = new Point(point.X - Runner.Position.X, point.Y - Runner.Position.Y);
        if (Runner.LocalBounds.Contains(localPoint))
            _scrolling = true;

        return true;
    }

    /// <summary>
    /// Update ScrollRunner position and dispatch scrolling events
    /// </summary>
    private void GameLoopOnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (!_scrolling)
            return;

        // check if scroll buttons are still active
        var buttons = new List<SButton> { SButton.MouseLeft }
            .Concat(Game1.options.useToolButton.Select(SButtonExtensions.ToSButton));

        if (!buttons.Any(b => ModEntry.ModHelper.Input.IsDown(b) || ModEntry.ModHelper.Input.IsSuppressed(b)))
        {
            _scrolling = false;
            return;
        }

        var mouseY = Game1.getMouseY(true);
        var progress = Math.Min(Math.Max(0f, mouseY - _scrollBackground.Y) / Height, 1);
        ScrollPosition = (int)(progress * ScrollMax);

        OnScroll?.Invoke(this, new ScrollBarEventArgs(ScrollPosition));
    }

    /// <summary>
    /// Cancel scrolling on button release
    /// </summary>
    private void InputOnButtonReleased(object sender, ButtonReleasedEventArgs e)
    {
        if (!_scrolling || e.IsSuppressed())
            // also check if the released button was suppressed
            return;

        _scrolling = false;
    }

    public event EventHandler<ScrollBarEventArgs> OnScroll;

    public class ScrollBarEventArgs : EventArgs
    {
        public ScrollBarEventArgs(int position)
        {
            Position = position;
        }

        public int Position { get; }
    }

    private class ScrollBarRunner : Widget
    {
        private static readonly TextureRegion TextureTop = new(Game1.mouseCursors, new Rectangle(435, 463, 6, 3), true);

        private static readonly TextureRegion TextureMid = new(Game1.mouseCursors, new Rectangle(435, 466, 6, 4), true);

        private static readonly TextureRegion TextureBot = new(Game1.mouseCursors, new Rectangle(435, 470, 6, 3), true);

        private bool Visible { get; } = true;

        public override void Draw(SpriteBatch batch)
        {
            if (!Visible)
                return;

            base.Draw(batch);

            var rect = GlobalBounds;
            batch.Draw(TextureMid, rect.X, rect.Y, rect.Width, rect.Height);
            batch.Draw(TextureTop, rect.X, rect.Y, TextureTop.Width, TextureTop.Height);
            batch.Draw(TextureBot, rect.X, rect.Bottom - TextureBot.Height, TextureBot.Width, TextureBot.Height);
        }
    }
}