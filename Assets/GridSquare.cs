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
    Empty,
    Wall,
}