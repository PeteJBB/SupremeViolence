using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Wall: MonoBehaviour 
{
    void Start()
    {
        // is there another wall here already?
        //var walls = GameObject.FindObjectsOfType<Wall>();
        //var x = Mathf.RoundToInt(transform.position.x);
        //var y = Mathf.RoundToInt(transform.position.y);
        //if (IsThereAWallAt(x, y, walls))
        //{
        //    Destroy(gameObject);
        //}
        //else
            UpdateEdges();
    }

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
        transform.FindChild("BottomLeftInner").GetComponent<SpriteRenderer>().enabled = isWallBelow && isWallLeft && !IsThereAWallAt(x-1, y-1, walls);
        transform.FindChild("BottomRightInner").GetComponent<SpriteRenderer>().enabled = isWallBelow && isWallRight && !IsThereAWallAt(x+1, y-1, walls);
    }


    private bool IsThereAWallAt(int x, int y, Wall[] walls)
    {
        var match = walls.FirstOrDefault(w => Mathf.RoundToInt(w.transform.position.x) == x && Mathf.RoundToInt(w.transform.position.y) == y);
        return match != null;
    }

    // skinning
    public void SetSkin(Texture2D skin, WallSideFlags sides)
    {
        if(sides == WallSideFlags.All)
            transform.FindChild("Bg").GetComponent<SkinableSprite>().SetSkin(skin);

        if (sides.HasFlags(WallSideFlags.Top))
            transform.FindChild("Top").GetComponent<SkinableSprite>().SetSkin(skin);

        if (sides.HasFlags(WallSideFlags.Left))
            transform.FindChild("Left").GetComponent<SkinableSprite>().SetSkin(skin);

        if (sides.HasFlags(WallSideFlags.Bottom))
            transform.FindChild("Bottom").GetComponent<SkinableSprite>().SetSkin(skin);

        if (sides.HasFlags(WallSideFlags.Right))
            transform.FindChild("Right").GetComponent<SkinableSprite>().SetSkin(skin);

        if (sides.HasFlags(WallSideFlags.Top, WallSideFlags.Left))
        {
            transform.FindChild("TopLeft").GetComponent<SkinableSprite>().SetSkin(skin);
            transform.FindChild("TopLeftInner").GetComponent<SkinableSprite>().SetSkin(skin);
        }   

        if (sides.HasFlags(WallSideFlags.Top, WallSideFlags.Right))
        {
            transform.FindChild("TopRight").GetComponent<SkinableSprite>().SetSkin(skin);
            transform.FindChild("TopRightInner").GetComponent<SkinableSprite>().SetSkin(skin);
        }  

        if (sides.HasFlags(WallSideFlags.Bottom, WallSideFlags.Left))
        {
            transform.FindChild("BottomLeft").GetComponent<SkinableSprite>().SetSkin(skin);
            transform.FindChild("BottomLeftInner").GetComponent<SkinableSprite>().SetSkin(skin);
        }   

        if (sides.HasFlags(WallSideFlags.Bottom, WallSideFlags.Right))
        {
            transform.FindChild("BottomRight").GetComponent<SkinableSprite>().SetSkin(skin);
            transform.FindChild("BottomRightInner").GetComponent<SkinableSprite>().SetSkin(skin);
        }  
    }

    
}

[Flags]
public enum WallSideFlags
{
    Top = 1,
    Bottom = 2,
    Left = 4,
    Right = 8,
    All = 15
}

public static class WallSideFlagsExtentions
{
    public static bool HasFlags(this WallSideFlags val, params WallSideFlags[] flags)
    {        
        var b = true;
        foreach (var f in flags)
        {
            if ((val & f) != f)
                return false;
        }

        return true;
    }
}