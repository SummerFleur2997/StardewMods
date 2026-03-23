using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace ConvenientChests.CraftFromChests.Framework;

internal class CraftingMenuArgs : EventArgs
{
    public CraftingPage Page { get; private set; }
    public bool IsCookingPage { get; private set; }

    public CraftingMenuArgs(CraftingPage craftingPage, bool isCookingPage)
    {
        Page = craftingPage;
        IsCookingPage = isCookingPage;
    }
}

internal class MenuListener
{
    private static IModEvents Events => ModEntry.ModHelper.Events;

    public event EventHandler<CraftingMenuArgs>? CraftingMenuShown;
    public event EventHandler? GameMenuTabChanged;

    private int _previousTab = -1;

    public void RegisterEvents()
    {
        ModEntry.Log("Register");
        Events.Display.MenuChanged += OnMenuChanged;
    }

    public void UnregisterEvents()
    {
        ModEntry.Log("UnRegister");
        Events.Display.MenuChanged -= OnMenuChanged;
    }

    /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu == e.OldMenu)
            return;

        switch (e.OldMenu)
        {
            case GameMenu:
                UnregisterTabEvent();
                break;
        }

        switch (e.NewMenu)
        {
            case GameMenu:
                Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
                break;

            case CraftingPage m when m.GetType().ToString() == "CookingSkill.NewCraftingPage":
                CraftingMenuShown?.Invoke(sender, new CraftingMenuArgs(m, true));
                break;

            case CraftingPage p:
                CraftingMenuShown?.Invoke(sender, new CraftingMenuArgs(p, p.cooking));
                break;
        }
    }

    /// <summary>
    /// When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null),
    /// raised after that menu is drawn to the sprite batch, but before it's
    /// rendered to the screen.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        switch (Game1.activeClickableMenu)
        {
            case null: // Game Menu closed
            case TitleMenu: // Quit to title
                UnregisterTabEvent();
                return;

            case GameMenu gameMenu:
                if (gameMenu.currentTab == _previousTab)
                    // Nothing changed
                    return;

                // Tab changed!
                GameMenuTabChanged?.Invoke(null, EventArgs.Empty);

                // check current page
                var currentPage = gameMenu.GetCurrentPage();
                if (currentPage is CraftingPage p)
                    CraftingMenuShown?.Invoke(sender, new CraftingMenuArgs(p, false));

                _previousTab = gameMenu.currentTab;
                break;

            default:
                // How did we get here?
                ModEntry.Log($"Unexpected menu: {Game1.activeClickableMenu.GetType()}");
                UnregisterTabEvent();
                return;
        }
    }

    private void UnregisterTabEvent()
    {
        Events.Display.RenderedActiveMenu -= OnRenderedActiveMenu;
        _previousTab = -1;
    }
}