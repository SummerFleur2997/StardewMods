namespace ConvenientChests.Framework;

public interface IModule
{
    public bool IsActive { get; }
    public void Activate();
    public void Deactivate();
}