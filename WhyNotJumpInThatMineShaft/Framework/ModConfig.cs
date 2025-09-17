namespace WhyNotJumpInThatMineShaft.Framework;

internal class ModConfig
{
    public bool ShaftPrompter { get; set; } = true;
    public bool ShaftIndicator { get; set; } = true;
    public bool StairIndicator { get; set; } = true;
    public int HideDistance { get; set; } = 3;
    public float IndicatorScale { get; set; } = 1;
    public bool TextPrompter { get; set; } = true;
    public int TextPositionX { get; set; } = 32;
    public int TextPositionY { get; set; } = 96;
    public float TextScale { get; set; } = 1.0f;
    public bool ShowDirection { get; set; } = true;
    public bool ShowDistance { get; set; } = true;
    public bool ShaftGeneratableIndicator { get; set; }
}