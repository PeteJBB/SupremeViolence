using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
public class SnapToArenaGrid: MonoBehaviour 
{
    void Update() 
    {
        if(Helper.IsEditMode())
        {
            // snap position
            var gpos = Arena.WorldToGridPosition(transform.localPosition);
            var wpos = Arena.GridToWorldPosition(gpos, transform.localPosition.z);

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