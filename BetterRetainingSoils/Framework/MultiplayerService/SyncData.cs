namespace BetterRetainingSoils.Framework.MultiplayerService;

[Serializable]
internal struct SyncData
{
    public string LocationName;
    public int X;
    public int Y;

    public SyncData(string locationName, int x, int y)
    {
        LocationName = locationName;
        X = x;
        Y = y;
    }
}