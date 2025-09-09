using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WhyNotJumpInThatMineShaft.Framework;

public static class Sprites
{
    public static readonly Texture2D IndicatorTexture 
        = ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/Indicator.png");
    
    public static readonly Rectangle ShaftIndicator = new (0, 0, 12, 12);
    public static readonly Rectangle StairIndicator = new (0, 12, 12, 24);
}