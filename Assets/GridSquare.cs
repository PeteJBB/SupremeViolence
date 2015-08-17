using UnityEngine;
using System.Collections;

public class GridSquare: MonoBehaviour 
{
    public GridSquareState State;
    public DoorPosition Door;

	// Use this for initialization
	void Start () 
    {
	
	}

    void OnDrawGizmos()
    {
        Color c = Color.white;
        switch(State)
        {
            case GridSquareState.Void:
                c = Color.red;
                break;
            case GridSquareState.Room:
                c = Color.green;
                break;
            case GridSquareState.Hallway:
                c = Color.blue;
                break;
        }

        Gizmos.color = c;
        Helper.DrawGizmoSquare(transform.position, 1);
        Gizmos.color = new Color(c.r, c.g, c.b, 0.1f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 0.01f));

        Gizmos.color = new Color(0,1,0,1f);
        switch(Door)
        {
            case DoorPosition.Top:
                Gizmos.DrawCube(transform.position + new Vector3(0, 0.45f, 0), new Vector3(1, 0.1f, 1));
                break;
            case DoorPosition.Right:
                Gizmos.DrawCube(transform.position + new Vector3(0.45f, 0, 0), new Vector3(0.1f, 1, 1));
                break;
            case DoorPosition.Bottom:
                Gizmos.DrawCube(transform.position + new Vector3(0, -0.45f, 0), new Vector3(1, 0.1f, 1));
                break;
            case DoorPosition.Left:
                Gizmos.DrawCube(transform.position + new Vector3(-0.45f, 0, 0), new Vector3(0.1f, 1, 1));
                break;
        }
    }
}

public enum DoorPosition
{
    None,
    Top,
    Right,
    Bottom,
    Left
}