using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WhyNotJumpInThatMineShaft.Framework;

internal static class Assets
{
    public static readonly Texture2D IndicatorTexture
        = ModEntry.ModHelper.ModContent.Load<Texture2D>(Path.Combine("assets", "Indicator.png"));

    public static readonly Rectangle ShaftIndicator = new(0, 0, 12, 12);
    public static readonly Rectangle StairIndicator = new(0, 12, 12, 12);
}