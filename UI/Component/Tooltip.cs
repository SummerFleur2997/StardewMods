using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Tooltip : IComponent
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

    public NineSlice Background = NineSlice.TooltipBackground();
    public TextLabel Name;
    public TextLabel Description;

    public Tooltip(Item item)
    {
        Name = new TextLabel(item.DisplayName, Color.Black, Game1.dialogueFont);
        Description = new TextLabel(item.getDescription(), Color.Black, Game1.smallFont);

        var borderThickness = Background.LeftBorderThickness;
        Width = Math.Max(Name.Width, Description.Width) + borderThickness * 2;
        Height = Name.Height + Description.Height + borderThickness * 2;
    }

    public Tooltip(string name = null, string desc = null)
    {
        if (name != null)
            Name = new TextLabel(name, Color.Black, Game1.dialogueFont);
        if (desc != null)
            Description = new TextLabel(desc, Color.Black, Game1.smallFont);

        var borderThickness = Background.LeftBorderThickness;
        Width = Math.Max(Name?.Width ?? 0, Description?.Width ?? 0) + borderThickness * 2;
        Height = Name?.Height ?? 0 + Description?.Height ?? 0 + borderThickness * 2;
    }

    /// <summary>
    /// Draw the tooltip at the right bottom corner of the mouse cursor.
    /// </summary>
    public virtual void Draw(SpriteBatch b)
    {
        // Move this to the correct position
        var borderThickness = Background.LeftBorderThickness;
        var mousePosition = Game1.getMousePosition(true);
        this.SetPosition(mousePosition.X + 32, mousePosition.Y + 32);
        Background.SetDestination(Bounds);

        // Calculate the position of the text
        var startX = Bounds.X + borderThickness;
        var startY = Bounds.Y + borderThickness;
        Name?.SetPosition(startX, startY);
        Description?.SetPosition(startX, startY + Name?.Height ?? 0);

        // Draw the tooltip
        Background.Draw(b);
        Name?.Draw(b);
        Description?.Draw(b);
    }
}