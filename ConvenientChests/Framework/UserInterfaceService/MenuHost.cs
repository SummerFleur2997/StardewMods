using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using UI;

namespace ConvenientChests.Framework.UserInterfaceService;

internal sealed class MenuHost<T> : BaseOverlay
{
    public readonly IOverlay<T> SideLabelButton;

    public MenuHost(IModEvents events, IInputHelper input, IReflectionHelper reflection, IOverlay<T> sideLabelButton)
        : base(events, input, reflection, assumeUiMode: true)
    {
        SideLabelButton = sideLabelButton;
    }

    protected override void DrawUi(SpriteBatch b)
    {
        SideLabelButton.DrawAboveUi(b);
        SideLabelButton.Tooltip?.Draw(b);
    }

    protected override bool ReceiveLeftClick(int x, int y) => SideLabelButton.ReceiveLeftClick(x, y);

    protected override bool ReceiveCursorHover(int x, int y) => SideLabelButton.ReceiveCursorHover(x, y);

    protected override bool ReceiveKeyPressed(Keys key) => SideLabelButton.ReceiveKeyPress(key);
}