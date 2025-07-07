using Microsoft.Xna.Framework;

namespace UI.UserInterface;

/// <summary>
/// A button shown as text on a background.
/// </summary>
internal class TextButton : Button
{
    private readonly Background _background;
    private readonly Label _label;

    private int LeftPadding => _background.Graphic.LeftBorderThickness;
    private int RightPadding => _background.Graphic.RightBorderThickness;
    private int TopPadding => _background.Graphic.TopBorderThickness;
    private int BottomPadding => _background.Graphic.BottomBorderThickness;

    public TextButton(string text, NineSlice backgroundTexture)
    {
        _label = new Label(text, Color.Black);
        _background = new Background(backgroundTexture);

        Width = _background.Width = _label.Width + LeftPadding + RightPadding;
        Height = _background.Height = _label.Height + TopPadding + BottomPadding;

        AddChild(_background);
        AddChild(_label);

        CenterLabel();
    }

    protected override void OnDimensionsChanged()
    {
        base.OnDimensionsChanged();

        if (_background != null)
        {
            _background.Width = Width;
            _background.Height = Height;
        }

        if (_label != null)
            CenterLabel();
    }

    private void CenterLabel()
    {
        _label.Position = new Point(
            LeftPadding + (Width - RightPadding - LeftPadding) / 2 - _label.Width / 2,
            TopPadding + (Height - BottomPadding - TopPadding) / 2 - _label.Height / 2
        );
    }
}