using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace WhyNotJumpInThatMineShaft.Framework;

internal static class Assets
{
    public static readonly Texture2D IndicatorTexture
        = ModEntry.ModHelper.ModContent.Load<Texture2D>(Path.Combine("assets", "Indicator.png"));
}