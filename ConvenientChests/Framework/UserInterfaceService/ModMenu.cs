using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using UI.UserInterface;
using Background = UI.UserInterface.Background;

namespace ConvenientChests.Framework.UserInterfaceService;

internal class ModMenu : Widget
{
    // Styling settings
    protected const int MaxItemRows = 7;
    protected const int MaxItemColumns = 12;
    protected const int MaxItemsPage = MaxItemColumns * MaxItemRows;
    protected static int Padding => 2 * Game1.pixelZoom;
    protected static SpriteFont HeaderFont => Game1.dialogueFont;
    
    // pagination
    protected int Row { get; set; }
    
    // Elements
    protected Widget Body { get; set; }
    protected Widget TopRow { get; set; }
    protected SpriteButton CloseButton { get; set; }
    protected ScrollBar ScrollBar { get; set; }
    protected Background Background { get; set; }
    protected Label TitleLabel { get; set; }
    protected WrapBag ToggleBag { get; set; }
    protected TooltipManager TooltipManager { get; init; }
}