using UnityEngine;
using System.Collections;

public class GridSquare: MonoBehaviour 
{
    public GridSquareType SquareType;

    void OnDrawGizmos()
    {
        Helper.DrawGridSquareGizmos(transform.position, SquareType, false);
    }

    void OnDrawGizmosSelected()
    {
        Helper.DrawGridSquareGizmos(transform.position, SquareType, true);
    }
}

public enum GridSquareType
{
    Empty,
    Wall,
    Void // Nothing can spawn here
}