using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace UI.Component;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Tooltip
{
    public Item? Item;
    public string? Name;
    public string? Description;

    public Tooltip(string? name = null, string? desc = null, int maxWidth = 320)
    {
        Name = Game1.parseText(name ?? "", Game1.dialogueFont, maxWidth);
        Description = Game1.parseText(desc ?? "", Game1.smallFont, maxWidth);
    }

    public Tooltip(Item item)
    {
        Name = item.DisplayName;
        Description = item.getDescription();
    }

    public void Draw(SpriteBatch b) => IClickableMenu.drawToolTip(b, Description, Name, null);
}

public interface IHaveTooltip
{
    Tooltip? Tooltip { get; }
}