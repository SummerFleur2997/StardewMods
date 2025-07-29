using ConvenientChests.Framework.SaveService;

namespace ConvenientChests.Framework;

public static class QuickSaveIntegration
{
    public static void Register()
    {
        if (!ModEntry.ModHelper.ModRegistry.IsLoaded("DLX.QuickSave")) return;
        var api = ModEntry.ModHelper.ModRegistry.GetApi<IQuickSaveAPI>("DLX.QuickSave");
        if (api != null) api.SavingEvent += Api_SavingEvent;

        ModEntry.Log("Successfully registered QuickSave events.");
    }

    /// <summary>
    /// 兼容 Quick Save
    /// Compatible to Quick Save
    /// </summary>
    private static void Api_SavingEvent(object sender, ISavingEventArgs e)
    {
        SaveManager.Save();
    }
}