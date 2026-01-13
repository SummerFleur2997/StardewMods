using BetterHatsAPI.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI.Component;

namespace BetterHatsAPI.GuideBook;

public class HatDataTextPanel : IComponent
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

    private readonly TextLabel _dataDesc = new("", Color.Black, Game1.smallFont);
    private readonly TextLabel _dataCondition = new("", Color.Black, Game1.smallFont);
    private readonly TextLabel _dataAction = new("", Color.Black, Game1.smallFont);
    private readonly TextLabel _dataModifier = new("", Color.Black, Game1.smallFont);

    private bool _isCombinedData;

    public HatDataTextPanel(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public void UpdateData(HatData data, bool isCombinedData = false)
    {
        // if it is combined data, we shouldn't go further.
        _isCombinedData = isCombinedData;
        if (isCombinedData) return;

        // the y position of the next first label
        var y = Y;
        // have a manual set description
        if (!string.IsNullOrWhiteSpace(data.Description))
        {
            var dText = I18n.String_Connect(I18n.String_Description(), data.Description);
            _dataDesc.Text = Game1.parseText(dText, Game1.smallFont, 640);
            _dataDesc.SetPosition(X, y);
            y += _dataDesc.Height + Game1.pixelZoom * 2;
        }
        else
        {
            var hint = data.GetNoDescriptionWarning();
            // no desc, but have advanced attributes, so we need to show a warning.
            if (!string.IsNullOrWhiteSpace(hint))
            {
                var dText = I18n.String_NoDesc(hint);
                _dataDesc.Text = Game1.parseText(dText, Game1.smallFont, 640);
                _dataDesc.SetPosition(X, y);
                y += _dataDesc.Height + Game1.pixelZoom * 2;
            }
            // no desc, and have no advanced attributes, nothing else to do.
            else
            {
                _dataDesc.Text = "";
            }
        }

        // get the condition description
        var cText = data.ConditionDescription;
        if (!string.IsNullOrWhiteSpace(cText))
        {
            cText = I18n.String_Connect(I18n.String_Condition(), cText);
            _dataCondition.Text = Game1.parseText(cText, Game1.smallFont, 640);
            _dataCondition.SetPosition(X, y);
            y += _dataCondition.Height + Game1.pixelZoom * 2;
        }
        else
        {
            _dataCondition.Text = "";
        }

        // get the action description
        var aText = data.ActionDescription;
        if (!string.IsNullOrWhiteSpace(aText))
        {
            aText = I18n.String_Connect(I18n.String_Action(), aText);
            _dataAction.Text = Game1.parseText(aText, Game1.smallFont, 640);
            _dataAction.SetPosition(X, y);
            y += _dataAction.Height + Game1.pixelZoom * 2;
        }
        else
        {
            _dataAction.Text = "";
        }

        // get the modifier description
        if (data.CustomModifier is not null)
        {
            var mText = I18n.String_Connect(I18n.String_Modifier(), data.ModifierDescription);
            _dataModifier.Text = Game1.parseText(mText, Game1.smallFont, 640);
            _dataModifier.SetPosition(X, y);
            return;
        }

        _dataModifier.Text = "";
    }

    public void Draw(SpriteBatch b)
    {
        if (_isCombinedData) return;

        if (!string.IsNullOrWhiteSpace(_dataDesc.Text))
            _dataDesc.Draw(b);
        if (!string.IsNullOrWhiteSpace(_dataCondition.Text))
            _dataCondition.Draw(b);
        if (!string.IsNullOrWhiteSpace(_dataAction.Text))
            _dataAction.Draw(b);
        if (!string.IsNullOrWhiteSpace(_dataModifier.Text))
            _dataModifier.Draw(b);
    }
}