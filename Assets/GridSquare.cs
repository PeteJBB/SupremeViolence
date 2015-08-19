using UnityEngine;
using System.Collections;

public class GridSquare: MonoBehaviour 
{
    public GridSquareState State;

    void OnDrawGizmos()
    {
        Helper.DrawGridSquareGizmos(transform.position, State, false);
    }

    void OnDrawGizmosSelected()
    {
        Helper.DrawGridSquareGizmos(transform.position, State, true);
    }
}

public enum GridSquareState
{
    Empty,
    Wall,
}