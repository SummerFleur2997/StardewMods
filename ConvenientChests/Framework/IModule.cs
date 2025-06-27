namespace ConvenientChests.Framework;

public interface IModule
{
    /// <summary>
    /// 模块启用状态。
    /// Whether the module is active.
    /// </summary>
    public bool IsActive { get; }

    /// <summary>
    /// 启用模块时的操作。
    /// Actions when active this module.
    /// </summary>
    public void Activate();

    /// <summary>
    /// 禁用模块时的操作。
    /// Actions when deactive this module.
    /// </summary>
    public void Deactivate();
}