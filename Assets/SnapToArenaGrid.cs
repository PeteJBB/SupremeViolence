using UnityEngine;
using System.Collections;
//using UnityEditor;

[ExecuteInEditMode]
public class SnapToArenaGrid: MonoBehaviour 
{
    public Vector2 Offset;

    void Update() 
    {
        if(Helper.IsEditMode())
        {
            // snap position
            var gpos = Arena.WorldToGridPosition(transform.localPosition - Offset.ToVector3());
            var wpos = Arena.GridToWorldPosition(gpos, transform.localPosition.z);

            transform.localPosition = wpos + Offset.ToVector3();
        }
    }
}