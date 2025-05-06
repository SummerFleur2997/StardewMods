using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConvenientChests.Framework.UserInterfacService;

/// <summary>
/// A checkbox with a label next to it, like so: [x] Foo
/// </summary>
internal class LabeledCheckbox : Widget
{
    public event Action<bool> OnChange;
    public bool Checked { get; set; }

    private readonly Widget _checkedBox;
    private readonly Widget _uncheckedBox;
    private readonly Label _label;

    public LabeledCheckbox(string labelText)
    {
        _checkedBox = AddChild(new Stamp(Sprites.FilledCheckbox));
        _uncheckedBox = AddChild(new Stamp(Sprites.EmptyCheckbox));

        _label = AddChild(new Label(labelText, Color.Black));
        var padding = (int)_label.Font.MeasureString(" ").X;

        Height = Math.Max(_checkedBox.Height, _label.Height);
        _checkedBox.CenterVertically();
        _uncheckedBox.CenterVertically();
        _label.CenterVertically();
        _label.X = _checkedBox.X + _checkedBox.Width + padding;
        Width = _label.X + _label.Width;
    }

    public override bool ReceiveLeftClick(Point point)
    {
        Checked = !Checked;
        OnChange?.Invoke(Checked);
        return true;
    }

    public override void Draw(SpriteBatch batch)
    {
        var box = Checked ? _checkedBox : _uncheckedBox;
        box.Draw(batch);
        _label.Draw(batch);
    }
}