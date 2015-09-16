using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GridSquare: MonoBehaviour 
{
    public GridSquareType SquareType;

    public Texture2D WallSkinBg;
    public Texture2D WallSkinTopLeft;
    public Texture2D WallSkinTopRight;
    public Texture2D WallSkinBottomLeft;
    public Texture2D WallSkinBottomRight;

    private GridSquareType z_SquareType;
    private Texture2D z_WallSkinBg;
    private Texture2D z_WallSkinTopLeft;
    private Texture2D z_WallSkinTopRight;
    private Texture2D z_WallSkinBottomLeft;
    private Texture2D z_WallSkinBottomRight;

    void Update()
    {
        if (z_SquareType != SquareType ||
            z_WallSkinBg != WallSkinBg ||
            z_WallSkinTopLeft != WallSkinTopLeft ||
            z_WallSkinTopRight != WallSkinTopRight ||
            z_WallSkinBottomLeft != WallSkinBottomLeft ||
            z_WallSkinBottomRight != WallSkinBottomRight)
        {
            z_SquareType = SquareType;

            var room = Helper.GetComponentInParentsRecursive<VariableRoom>(transform);
            if (room != null)
            {
                room.GenerateOnNextUpdate = true;
            }

        }
    }

    void OnDrawGizmos()
    {
        Helper.DrawGridSquareGizmos(transform.position, SquareType, false);
    }

    void OnDrawGizmosSelected()
    {
        Helper.DrawGridSquareGizmos(transform.position, SquareType, true);
    }
}

public enum GridSquareType
{
    Empty,
    Wall,
    Void, // Nothing can spawn here,
    OutOfBounds
}

public class GridSquareInfo
{
    public readonly int x;
    public readonly int y;

    public GridSquareType GridSquareType;
    public Room Room;
    public List<GridTrackedObject> Objects;

    public GridSquareInfo(int x, int y)
    {
        this.x = x;
        this.y = y;
        Objects = new List<GridTrackedObject>();
    }
}

public class GridContentsChangedEvent : UnityEvent<GridSquareInfo>
{ }