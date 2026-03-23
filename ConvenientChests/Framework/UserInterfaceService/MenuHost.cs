using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using UI;

namespace ConvenientChests.Framework.UserInterfaceService;

internal sealed class MenuHost<T> : BaseOverlay
{
    public readonly IOverlay<T> Overlay;

    public MenuHost(IModEvents events, IInputHelper input, IReflectionHelper reflection, IOverlay<T> overlay)
        : base(events, input, reflection, assumeUiMode: true)
    {
        Overlay = overlay;
    }

    protected override void DrawUi(SpriteBatch b) => Overlay.DrawUi(b);

    protected override bool ReceiveLeftClick(int x, int y) => Overlay.ReceiveLeftClick(x, y);

    protected override bool ReceiveCursorHover(int x, int y) => Overlay.ReceiveCursorHover(x, y);

    protected override bool ReceiveKeyPressed(Keys key) => Overlay.ReceiveKeyPress(key);
}