using System;
using Microsoft.Xna.Framework;

namespace UI.UserInterface;

/// <summary>
/// A simple clickable widget.
/// </summary>
internal abstract class Button : Widget
{
    public event Action OnPress;

    public override bool ReceiveLeftClick(Point point)
    {
        OnPress?.Invoke();
        return true;
    }
}