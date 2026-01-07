using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using UI;

namespace ConvenientChests.Framework.UserInterfaceService;

internal sealed class MenuHost<T> : BaseOverlay
{
    public readonly IOverlay<T> SideLabelButton;

    public MenuHost(IModEvents events, IInputHelper input, IReflectionHelper reflection, IOverlay<T> sideLabelButton)
        : base(events, input, reflection, assumeUiMode: true) =>
        SideLabelButton = sideLabelButton;

    protected override void DrawUi(SpriteBatch batch)
    {
        if (Game1.activeClickableMenu is not T) return;
        SideLabelButton.Draw(batch);
    }

    protected override bool ReceiveLeftClick(int x, int y) => SideLabelButton.ReceiveLeftClick(x, y);

    protected override bool ReceiveCursorHover(int x, int y) => SideLabelButton.ReceiveCursorHover(x, y);
}