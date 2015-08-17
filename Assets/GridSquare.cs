﻿using UnityEngine;
using System.Collections;

public class GridSquare: MonoBehaviour 
{
    public GridSquareState State;
    public DoorPosition DoorPosition;

	// Use this for initialization
	void Start () 
    {
	
	}

    void OnDrawGizmos()
    {
        Helper.DrawGridSquareGizmos(transform.position, State, DoorPosition);
    }
}

public enum GridSquareState
{
    Empty,  // nothing here, available space
    Void,   // nothing here but dont allow anything to be put here
    Room,   // space is part of a room
    Hallway // space is part of a hallway
}

public enum DoorPosition
{
    None,
    Top,
    Right,
    Bottom,
    Left
}