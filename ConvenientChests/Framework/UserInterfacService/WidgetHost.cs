using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ConvenientChests.Framework.UserInterfacService;

internal class WidgetHost : BaseOverlay
{
    public readonly Widget RootWidget;
    public readonly TooltipManager TooltipManager;

    public WidgetHost(IModEvents events, IInputHelper input, IReflectionHelper reflection)
        : base(events, input, reflection, assumeUiMode: true)
    {
        RootWidget = new Widget { Width = Game1.uiViewport.Width, Height = Game1.uiViewport.Height };
        TooltipManager = new TooltipManager();
    }

    protected override void DrawUi(SpriteBatch batch)
    {
        RootWidget.Draw(batch);
        DrawCursor();
        TooltipManager.Draw(batch);
    }

    protected override void ReceiveButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        foreach (var button in e.Released)
            ReceiveButtonPress(button);
    }

    protected bool ReceiveButtonPress(SButton input)
    {
        return RootWidget.ReceiveButtonPress(input);
    }

    protected override bool ReceiveLeftClick(int x, int y)
    {
        return RootWidget.ReceiveLeftClick(new Point(x, y));
    }

    protected override bool ReceiveCursorHover(int x, int y)
    {
        return RootWidget.ReceiveCursorHover(new Point(x, y));
    }

    protected override bool ReceiveScrollWheelAction(int amount)
    {
        return RootWidget.ReceiveScrollWheelAction(amount);
    }


    public override void Dispose()
    {
        base.Dispose();
        RootWidget?.Dispose();
    }
}