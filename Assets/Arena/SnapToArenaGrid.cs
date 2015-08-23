﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
public class SnapToArenaGrid: MonoBehaviour 
{
    public SnapToGridPlacement SnapPlacement;

    void Update() 
    {
        if(!EditorApplication.isPlaying)
        {
            // snap position
            var gpos = Arena.WorldToGridPosition(transform.localPosition);
            var wpos = Arena.GridToWorldPosition(gpos, transform.localPosition.z);

//            switch(SnapPlacement)
//            {
//                case SnapToGridPlacement.Center:
//                default:
//                    wpos -= new Vector3(0.5f, 0.5f, 0);
//                    break;
//                case SnapToGridPlacement.BottomLeft:
//                    break;
//                case SnapToGridPlacement.LeftCenter:
//                    wpos -= new Vector3(0, 0.5f, 0);
//                    break;
//                case SnapToGridPlacement.BottomCenter:
//                    wpos -= new Vector3(0.5f, 0, 0);
//                    break;
//            }

            transform.localPosition = wpos;
        }
    }
}

public enum SnapToGridPlacement
{
    Center,
    BottomLeft,
    LeftCenter,
    BottomCenter
}