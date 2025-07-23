using System;

namespace BetterRetainingSoils.Framework.MultiplayerService;

[Serializable]
public struct SyncData(string locationName, int x, int y)
{
    public string LocationName { get; set; } = locationName;
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
}