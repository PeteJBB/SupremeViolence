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

//    [ContextMenu ("Rebuild grid")]
//	void RebuildGrid()
//    {
//        Debug.Log("Rebuilding grid");
//
//        Helper.DestroyAllChildren(gridContainer, true);
//
//        // set up squares
//        for(var x=0; x<SizeX; x++)
//        {
//            for(var y=0; y<SizeY; y++)
//            {
//                var tile = Instantiate(GridSquarePrefab);
//                tile.transform.SetParent(gridContainer);
//                tile.transform.localPosition = new Vector3(x, y, 0);
//            }
//        }
//    }
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
    MiddleLeft
}