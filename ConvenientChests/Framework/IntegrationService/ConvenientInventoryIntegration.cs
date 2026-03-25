using Common.ConfigurationServices;

namespace ConvenientChests.Framework.IntegrationService;

public static class ConvenientInventoryIntegration
{
    public static IConvenientInventoryAPI? CIApi;

    public static void Register()
    {
        var api = IntegrationHelper.GetValidatedApi<IConvenientInventoryAPI>(
            "Convenient Inventory",
            "gaussfire.ConvenientInventory",
            "1.6.1",
            ModEntry.ModHelper.ModRegistry,
            ModEntry.Log);

        if (api == null) return;

        CIApi = api;
        ModEntry.Log("Successfully integrated with Convenient Inventory.");
    }
}