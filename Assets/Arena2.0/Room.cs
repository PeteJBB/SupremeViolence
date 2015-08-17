using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[ExecuteInEditMode]
public class Room: MonoBehaviour
{
    private Transform gridContainer;

	// Use this for initialization
	void Start () 
    {
        gridContainer = transform.Find("grid");
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