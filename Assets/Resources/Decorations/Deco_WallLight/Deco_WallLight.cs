using UnityEngine;
using System.Collections;

public class Deco_WallLight : Decoration 
{
	public override Vector3? GetSpawnLocationForGridSquare(int gridx, int gridy, System.Collections.Generic.List<GameObject> items)
    {
        // check if the spot below is empty
        if(Arena.Instance.IsThereAWallAt(gridx, gridy + 1))
        {
            // spawn a light here
            var pos = Arena.Instance.GridToWorldPosition(gridx, gridy);
            pos.y += 0.5f;
            
            return pos;
        }

        return null;
    }
}
