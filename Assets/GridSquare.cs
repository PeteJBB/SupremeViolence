using UnityEngine;
using System.Collections;

public class GridSquare: MonoBehaviour 
{
    public GridSquareState State;

    void OnDrawGizmos()
    {
        Helper.DrawGridSquareGizmos(transform.position, State);
    }
}

public enum GridSquareState
{
    Empty,  // nothing here, available space
    Void,   // unusable space - nothing will be spawned here
    Room,   // space is part of a room
    Hallway // space is part of a hallway
}