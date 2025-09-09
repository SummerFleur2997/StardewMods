namespace WhyNotJumpInThatMineShaft.Framework;

public class ModConfig
{
    public bool ShaftPrompter { get; set; } = true;
    public bool ShaftIndicator { get; set; } = true;
    public bool StairIndicator { get; set; } = true;
    public bool TextPrompter { get; set; } = true;
    public int TextPositionX { get; set; } = 32;
    public int TextPositionY { get; set; } = 96;
    public float TextScale { get; set; } = 1.0f;
    public bool ShowDirection { get; set; } = true;
    public bool ShowDistance { get; set; } = true;
}