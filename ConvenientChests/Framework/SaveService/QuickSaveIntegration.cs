using Common.ConfigurationServices;

namespace ConvenientChests.Framework.SaveService;

public static class QuickSaveIntegration
{
    public static void Register()
    {
        var api = IntegrationHelper.GetValidatedApi<IQuickSaveAPI>(
            "Quick Save",
            "DLX.QuickSave",
            "1.2.1",
            ModEntry.ModHelper.ModRegistry,
            ModEntry.Log);
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