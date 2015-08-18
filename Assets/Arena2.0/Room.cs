using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[ExecuteInEditMode]
public class Room: MonoBehaviour
{
    public RoomPosition RoomPosition;

    private Transform gridContainer;

	// Use this for initialization
	void Awake () 
    {
        gridContainer = transform.Find("grid");
	}

    public int GetGridSizeX()
    {
        var squares = gridContainer.GetComponentsInChildren<GridSquare>();
        return (int)squares.Max(sq => sq.transform.localPosition.x) - (int)squares.Min(sq => sq.transform.localPosition.y);
    }

    public int GetGridSizeY()
    {
        var squares = gridContainer.GetComponentsInChildren<GridSquare>();
        return (int)squares.Max(sq => sq.transform.localPosition.y) - (int)squares.Min(sq => sq.transform.localPosition.y);
    }

    public GridSquare[] GetGridSquares()
    {
        return gridContainer.GetComponentsInChildren<GridSquare>();
    }

    void OnDrawGizmos()
    {
        switch(RoomPosition)
        {
            case RoomPosition.TopLeft:
                DrawDoorGizmo("right");
                DrawDoorGizmo("bottom");
                break;
            case RoomPosition.TopMiddle:
                DrawDoorGizmo("right");
                DrawDoorGizmo("bottom");
                DrawDoorGizmo("left");
                break;
            case RoomPosition.TopRight:
                DrawDoorGizmo("bottom");
                DrawDoorGizmo("left");
                break;
            case RoomPosition.MiddleLeft:
                DrawDoorGizmo("top");
                DrawDoorGizmo("right");
                DrawDoorGizmo("bottom");
                break;
            case RoomPosition.Center:
                DrawDoorGizmo("top");
                DrawDoorGizmo("right");
                DrawDoorGizmo("bottom");
                DrawDoorGizmo("left");
                break;
            case RoomPosition.MiddleRight:
                DrawDoorGizmo("top");
                DrawDoorGizmo("bottom");
                DrawDoorGizmo("left");
                break;
            case RoomPosition.BottomLeft:
                DrawDoorGizmo("top");
                DrawDoorGizmo("right");
                break;
            case RoomPosition.BottomMiddle:
                DrawDoorGizmo("top");
                DrawDoorGizmo("right");
                DrawDoorGizmo("left");
                break;
            case RoomPosition.BottomRight:
                DrawDoorGizmo("top");
                DrawDoorGizmo("left");
                break;
        }
    }

    void DrawDoorGizmo(string pos)
    {
        var roomSize = 5f;
        if(pos == "top")
            Gizmos.DrawCube(transform.position + new Vector3(2.5f, 5f, 0), new Vector3(1, 0.1f, 1));
        else if(pos == "right")
            Gizmos.DrawCube(transform.position + new Vector3(5f, 2.5f, 0), new Vector3(0.1f, 1, 1));
        else if(pos == "bottom")
            Gizmos.DrawCube(transform.position + new Vector3(2.5f, 0, 0), new Vector3(1, 0.1f, 1));
        else if(pos == "left")
            Gizmos.DrawCube(transform.position + new Vector3(0f, 2.5f, 0), new Vector3(0.1f, 1, 1));
    }

}

public enum RoomPosition
{
    Center,
    TopLeft,
    TopMiddle,
    TopRight,
    MiddleRight,
    BottomRight,
    BottomMiddle,
    BottomLeft,
    MiddleLeft,
    DoorwayVertical,
    DoorwayHorizontal
}