namespace ConvenientChests.Framework.IntegrationService;

public interface IQuickSaveAPI
{
    /* Save Event Order:
     * 1. QS-Saving (IsSaving = true)
     * 2. QS-Saved (IsSaving = false)
     */

    /// <summary>Fires before a Quicksave is being created</summary>
    public event SavingDelegate SavingEvent;

    public delegate void SavingDelegate(object sender, ISavingEventArgs e);
}

public interface ISavingEventArgs { }