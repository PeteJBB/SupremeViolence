using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Wall: MonoBehaviour 
{
    [ContextMenu("Update Edges")]
	public void UpdateEdges()
    {
        var walls = GameObject.FindObjectsOfType<Wall>();

        var x = Mathf.RoundToInt(transform.position.x);
        var y = Mathf.RoundToInt(transform.position.y);

        var left = !IsThereAWallAt(x - 1, y, walls);
        transform.FindChild("LeftTop").GetComponent<SpriteRenderer>().enabled = left;
        transform.FindChild("LeftBottom").GetComponent<SpriteRenderer>().enabled = left;

        var right = !IsThereAWallAt(x + 1, y, walls);
        transform.FindChild("RightTop").GetComponent<SpriteRenderer>().enabled = right;
        transform.FindChild("RightBottom").GetComponent<SpriteRenderer>().enabled = right;

        var top = !IsThereAWallAt(x, y + 1, walls);
        transform.FindChild("Top").GetComponent<SpriteRenderer>().enabled = top;

        var bottom = !IsThereAWallAt(x, y - 1, walls);
        transform.FindChild("Bottom").GetComponent<SpriteRenderer>().enabled = bottom;
        
        // now do inside corners
        var topLeft = IsThereAWallAt(x-1, y, walls) && IsThereAWallAt(x, y+1, walls) && !IsThereAWallAt(x-1, y+1, walls);
        transform.FindChild("TopLeft").GetComponent<SpriteRenderer>().enabled = topLeft;

        var topRight = IsThereAWallAt(x+1, y, walls) && IsThereAWallAt(x, y+1, walls) && !IsThereAWallAt(x+1, y+1, walls);
        transform.FindChild("TopRight").GetComponent<SpriteRenderer>().enabled = topRight;

        var bottomRight = IsThereAWallAt(x+1, y, walls) && IsThereAWallAt(x, y-1, walls) && !IsThereAWallAt(x+1, y-1, walls);
        transform.FindChild("BottomRight").GetComponent<SpriteRenderer>().enabled = bottomRight;

        var bottomLeft = IsThereAWallAt(x-1, y, walls) && IsThereAWallAt(x, y-1, walls) && !IsThereAWallAt(x-1, y-1, walls);
        transform.FindChild("BottomLeft").GetComponent<SpriteRenderer>().enabled = bottomLeft;
    }

    private bool IsThereAWallAt(int x, int y, Wall[] walls)
    {
        var match = walls.FirstOrDefault(w => Mathf.RoundToInt(w.transform.position.x) == x && Mathf.RoundToInt(w.transform.position.y) == y);
        return match != null;
    }
}
