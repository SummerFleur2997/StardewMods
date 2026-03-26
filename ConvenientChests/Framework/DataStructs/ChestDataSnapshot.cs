using ConvenientChests.Framework.DataService;
using Newtonsoft.Json;

namespace ConvenientChests.Framework.DataStructs;

[Serializable]
internal class ChestDataSnapshot : IChestData
{
    /// <summary>
    /// The name of the snapshot. Defined by the user.
    /// </summary>
    public string Alias
    {
        get => _alias;
        set => _alias = SnapshotManager.GetAValidAlias(value, UniqueID);
    }

    private string _alias = "";

    /// <summary>
    /// The unique ID of the snapshot, used to identify it.
    /// </summary>
    public long UniqueID { get; init; }

    /// <summary>
    /// A series of accepted items.
    /// </summary>
    [JsonConverter(typeof(DataConverter))]
    public HashSet<string> AcceptedItems { get; set; } = new();

    public ChestDataSnapshot() { }

    public ChestDataSnapshot(string alias, long uniqueID, IEnumerable<string> acceptedItemKinds)
    {
        Alias = alias;
        UniqueID = uniqueID;
        AcceptedItems = acceptedItemKinds.ToHashSet();
    }

    public void Toggle(string itemKey)
    {
        if (this.Accepts(itemKey))
            this.RemoveAccepted(itemKey);
        else
            this.AddAccepted(itemKey);
    }
}