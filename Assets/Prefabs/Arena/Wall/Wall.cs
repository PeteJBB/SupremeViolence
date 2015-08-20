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

        // where are other walls
        var isWallAbove = IsThereAWallAt(x, y+1, walls);
        var isWallBelow = IsThereAWallAt(x, y-1, walls);
        var isWallLeft = IsThereAWallAt(x-1, y, walls);
        var isWallRight = IsThereAWallAt(x+1, y, walls);

        // sides
        transform.FindChild("Left").GetComponent<SpriteRenderer>().enabled = !isWallLeft;
        transform.FindChild("Right").GetComponent<SpriteRenderer>().enabled = !isWallRight;
        transform.FindChild("Top").GetComponent<SpriteRenderer>().enabled = !isWallAbove;
        transform.FindChild("Bottom").GetComponent<SpriteRenderer>().enabled = !isWallBelow;
        
        // outside corners
        transform.FindChild("TopLeft").GetComponent<SpriteRenderer>().enabled = !isWallAbove && !isWallLeft;
        transform.FindChild("TopRight").GetComponent<SpriteRenderer>().enabled = !isWallAbove && !isWallRight;
        transform.FindChild("BottomLeft").GetComponent<SpriteRenderer>().enabled = !isWallBelow && !isWallLeft;
        transform.FindChild("BottomRight").GetComponent<SpriteRenderer>().enabled = !isWallBelow && !isWallRight;

        // inside corners
        transform.FindChild("TopLeftInner").GetComponent<SpriteRenderer>().enabled = isWallAbove && isWallLeft && !IsThereAWallAt(x-1, y+1, walls);
        transform.FindChild("TopRightInner").GetComponent<SpriteRenderer>().enabled = isWallAbove && isWallRight && !IsThereAWallAt(x+1, y+1, walls);
        transform.FindChild("BottomLeftInner").GetComponent<SpriteRenderer>().enabled = isWallBelow && isWallLeft && !IsThereAWallAt(x+1, y-1, walls);
        transform.FindChild("BottomRightInner").GetComponent<SpriteRenderer>().enabled = isWallBelow && isWallRight && !IsThereAWallAt(x-1, y-1, walls);
    }

    private bool IsThereAWallAt(int x, int y, Wall[] walls)
    {
        var match = walls.FirstOrDefault(w => Mathf.RoundToInt(w.transform.position.x) == x && Mathf.RoundToInt(w.transform.position.y) == y);
        return match != null;
    }
}
