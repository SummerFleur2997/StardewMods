using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using UI;

namespace ConvenientChests.Framework.UserInterfaceService;

internal sealed class MenuHost<T> : BaseOverlay
{
    private readonly IOverlay<T> _sideLabelButton;

    public MenuHost(IModEvents events, IInputHelper input, IReflectionHelper reflection, IOverlay<T> sideLabelButton)
        : base(events, input, reflection, assumeUiMode: true)
    {
        _sideLabelButton = sideLabelButton;
    }

    protected override void DrawUi(SpriteBatch batch)
    {
        if (Game1.activeClickableMenu is not T) return;
        _sideLabelButton.Draw(batch);
    }

    protected override bool ReceiveLeftClick(int x, int y) => _sideLabelButton.ReceiveLeftClick(x, y);
}