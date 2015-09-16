using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class GridSquareInfo
{
    public readonly int x;
    public readonly int y;

    public GridSquareType GridSquareType;
    public List<GridTrackedObject> Objects;

    public GridSquareInfo(int x, int y)
    {
        this.x = x;
        this.y = y;
        Objects = new List<GridTrackedObject>();
    }
}

public enum GridSquareType
{
    Empty,
    Wall,
    Void, // Nothing can spawn here,
    OutOfBounds
}

public class GridContentsChangedEvent : UnityEvent<GridSquareInfo>
{ }