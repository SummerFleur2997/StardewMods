namespace ConvenientChests.Framework;

internal interface IModule
{
    public bool IsActive { get; }
    public void Activate();
    public void Deactivate();
}