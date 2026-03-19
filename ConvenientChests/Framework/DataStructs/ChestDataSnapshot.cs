namespace ConvenientChests.Framework.DataStructs;

[Serializable]
internal class ChestDataSnapshot : IChestData
{
    /// <summary>
    /// The name of the snapshot. Defined by the user.
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// A series of accepted items.
    /// </summary>
    public HashSet<ItemKey> AcceptedItemKinds { get; set; } = new();

    /// <summary>
    /// The unique ID of the snapshot, used to identify it.
    /// </summary>
    public long UniqueID { get; set; }

    public ChestDataSnapshot() { }

    public ChestDataSnapshot(string note, long uniqueID, IEnumerable<ItemKey> acceptedItemKinds)
    {
        Note = note;
        UniqueID = uniqueID;
        AcceptedItemKinds = acceptedItemKinds.ToHashSet();
    }

    public void ToggleItem(ItemKey itemKey)
    {
        if (this.Accepts(itemKey))
            this.RemoveAccepted(itemKey);
        else
            this.AddAccepted(itemKey);
    }
}