using ConvenientChests.CategorizeChests.Framework;
using ConvenientChests.Framework;

namespace ConvenientChests.CategorizeChests;

internal class CategorizeChestsModule : IModule
{
    public bool IsActive { get; private set; }
    public CategoryDataManager CategoryDataManager { get; } = new();

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}