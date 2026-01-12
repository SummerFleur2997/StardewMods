using JetBrains.Annotations;
using UI.Menu;

namespace BetterHatsAPI.GuideBook;

public class GuideMenu : BaseMenu
{
    /// <summary>
    /// Used to compatible with LookupAnything.
    /// </summary>
    [UsedImplicitly] public Item HoveredItem;

    public GuideMenu(int x, int y, int width, int height)
        : base(x, y, width, height) { }
}