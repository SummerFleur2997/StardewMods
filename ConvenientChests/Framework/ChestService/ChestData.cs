using System.Collections.Generic;
using ConvenientChests.Framework.ItemService;
using StardewValley.Objects;

namespace ConvenientChests.Framework.ChestService;

/// <summary>
/// The extra data associated with a chest object, such as the list of
/// items it should accept.
/// </summary>
internal class ChestData
{
    public Chest Chest { get; }
    public HashSet<ItemKey> AcceptedItemKinds { get; set; } = new();

    public ChestData(Chest chest)
    {
        Chest = chest;
    }

    /// <summary>
    /// Set this chest to accept the specified kind of item.
    /// </summary>
    private void AddAccepted(ItemKey itemKey)
    {
        AcceptedItemKinds.Add(itemKey);
    }

    /// <summary>
    /// Set this chest to not accept the specified kind of item.
    /// </summary>
    private void RemoveAccepted(ItemKey itemKey)
    {
        if (AcceptedItemKinds.Contains(itemKey))
            AcceptedItemKinds.Remove(itemKey);
    }

    /// <summary>
    /// Toggle whether this chest accepts the specified kind of item.
    /// </summary>
    public void Toggle(ItemKey itemKey)
    {
        if (Accepts(itemKey))
            RemoveAccepted(itemKey);

        else
            AddAccepted(itemKey);
    }

    /// <summary>
    /// Return whether this chest accepts the given kind of item.
    /// </summary>
    public bool Accepts(ItemKey itemKey)
    {
        return AcceptedItemKinds.Contains(itemKey);
    }
}