using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GridTrackedObject: MonoBehaviour 
{    
    [HideInInspector]
    public Vector2 CurrentGridPos;

    public Color MapColor = Color.white;
    public int MinimapPriority = 0;

    public TrackGridPositionChangedEvent OnGridPositionChanged; // OldPosition, NewPosition

    void Start()
    {
        if (OnGridPositionChanged == null)
			OnGridPositionChanged = new TrackGridPositionChangedEvent();

        //CurrentGridPos = Arena.WorldToGridPosition(transform.position);
        Arena.Instance.RegisterTrackableObject(this);
    }

	// Update is called once per frame
	void LateUpdate () 
    {
        // update grid pos
        var gridpos = Arena.WorldToGridPosition(transform.position);
        if (gridpos != CurrentGridPos)
        {
            //Debug.Log("TrackGridPosition changed: " + gridpos.x + ", " + gridpos.y);
        
            var oldPos = CurrentGridPos;
            CurrentGridPos = gridpos;
            OnGridPositionChanged.Invoke(oldPos, CurrentGridPos);
        }
	}
}

public class TrackGridPositionChangedEvent : UnityEvent<Vector2, Vector2>
{ }

