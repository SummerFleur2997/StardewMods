using Common;
using StardewModdingAPI.Events;

namespace ConvenientChests.WhereIsMyItem;

public class WhereIsMyItemModule : IModule
{
    public bool IsActive { get; private set; }

    public void Activate()
    {
        IsActive = true;
        ModEntry.ModHelper.Events.Input.ButtonsChanged += OnButtonChanged;
    }

    public void Deactivate()
    {
        IsActive = false;
        ModEntry.ModHelper.Events.Input.ButtonsChanged -= OnButtonChanged;
    }

    /// <summary>
    /// Response the shortcut of opening the guide book.
    /// </summary>
    private static void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!ModEntry.Config.WhereIsMyItemKey.JustPressed())
            return;
    }
}