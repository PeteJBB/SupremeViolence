using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// After the arena generates a room it boradcasts a message to all its children saying which position it is in
/// </summary>
public class RoomPositionCondition: MonoBehaviour 
{
    public bool TopLeft = true;
    public bool Top = true;
    public bool TopRight = true;
    public bool Left = true;
    public bool Center = true;
    public bool Right = true;
    public bool BottomLeft = true;
    public bool Bottom = true;
    public bool BottomRight = true;

    void SetRoomPositionFlags(RoomPositionFlags flags)
    {
        if (!TopLeft && ((flags & RoomPositionFlags.Top) == RoomPositionFlags.Top) && ((flags & RoomPositionFlags.Left) == RoomPositionFlags.Left))
            Destroy(gameObject);

        else if (!TopRight && ((flags & RoomPositionFlags.Top) == RoomPositionFlags.Top) && ((flags & RoomPositionFlags.Right) == RoomPositionFlags.Right))
            Destroy(gameObject);

        else if (!BottomLeft && ((flags & RoomPositionFlags.Bottom) == RoomPositionFlags.Bottom) && ((flags & RoomPositionFlags.Left) == RoomPositionFlags.Left))
            Destroy(gameObject);

        else if (!BottomRight && ((flags & RoomPositionFlags.Bottom) == RoomPositionFlags.Bottom) && ((flags & RoomPositionFlags.Right) == RoomPositionFlags.Right))
            Destroy(gameObject);

        else if (!Top && flags == RoomPositionFlags.Top)
            Destroy(gameObject);

        else if (!Bottom && flags == RoomPositionFlags.Bottom)
            Destroy(gameObject);

        else if (!Left && flags == RoomPositionFlags.Left)
            Destroy(gameObject);

        else if (!Right && flags == RoomPositionFlags.Right)
            Destroy(gameObject);
        
        else if(!Center && flags == RoomPositionFlags.Center)
            Destroy(gameObject);
    }
}