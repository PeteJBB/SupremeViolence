using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Wall: MonoBehaviour 
{
    private SpriteRenderer bg;
    private SpriteRenderer topL;
    private SpriteRenderer topR;
    private SpriteRenderer topLeft;
    private SpriteRenderer topRight;
    private SpriteRenderer left;
    private SpriteRenderer right;
    private SpriteRenderer bottomL;
    private SpriteRenderer bottomR;
    private SpriteRenderer bottomLeft;
    private SpriteRenderer bottomRight;

    private SpriteRenderer topLeftInner;
    private SpriteRenderer topRightInner;
    private SpriteRenderer bottomLeftInner;
    private SpriteRenderer bottomRightInner;

    void Start()
    {
        UpdateEdges();
    }

    [ContextMenu("Update Edges")]
	public void UpdateEdges()
    {
        UpdateEdges(null);
    }

    private void FindSpriteRenderers()
    {
        bg = transform.FindChild("Bg").GetComponent<SpriteRenderer>();
        topL = transform.FindChild("TopL").GetComponent<SpriteRenderer>();
        topR = transform.FindChild("TopR").GetComponent<SpriteRenderer>();
        topLeft = transform.FindChild("TopLeft").GetComponent<SpriteRenderer>();
        topRight = transform.FindChild("TopRight").GetComponent<SpriteRenderer>();
        left = transform.FindChild("Left").GetComponent<SpriteRenderer>();
        right = transform.FindChild("Right").GetComponent<SpriteRenderer>();
        bottomL = transform.FindChild("BottomL").GetComponent<SpriteRenderer>();
        bottomR = transform.FindChild("BottomR").GetComponent<SpriteRenderer>();
        bottomLeft = transform.FindChild("BottomLeft").GetComponent<SpriteRenderer>();
        bottomRight = transform.FindChild("BottomRight").GetComponent<SpriteRenderer>();

        topLeftInner = transform.FindChild("TopLeftInner").GetComponent<SpriteRenderer>();
        topRightInner = transform.FindChild("TopRightInner").GetComponent<SpriteRenderer>();
        bottomLeftInner = transform.FindChild("BottomLeftInner").GetComponent<SpriteRenderer>();
        bottomRightInner = transform.FindChild("BottomRightInner").GetComponent<SpriteRenderer>();
    }

    public void UpdateEdges(Wall[] walls)
    {
        FindSpriteRenderers();

        if(walls == null)
            walls = GameObject.FindObjectsOfType<Wall>();

        var x = Mathf.RoundToInt(transform.position.x);
        var y = Mathf.RoundToInt(transform.position.y);

        // where are other walls
        var isWallAbove = IsThereAWallAt(x, y+1, walls);
        var isWallBelow = IsThereAWallAt(x, y-1, walls);
        var isWallLeft = IsThereAWallAt(x-1, y, walls);
        var isWallRight = IsThereAWallAt(x+1, y, walls);

        // outside corners
        topLeft.enabled = !isWallAbove && !isWallLeft;        
        topRight.enabled = !isWallAbove && !isWallRight;
        bottomLeft.enabled = !isWallBelow && !isWallLeft;
        bottomRight.enabled = !isWallBelow && !isWallRight;

        // inside corners
        topLeftInner.enabled = isWallAbove && isWallLeft && !IsThereAWallAt(x-1, y+1, walls);
        topRightInner.enabled = isWallAbove && isWallRight && !IsThereAWallAt(x+1, y+1, walls);
        bottomLeftInner.enabled = isWallBelow && isWallLeft && !IsThereAWallAt(x-1, y-1, walls);
        bottomRightInner.enabled = isWallBelow && isWallRight && !IsThereAWallAt(x+1, y-1, walls);

        // sides
        topL.enabled = !isWallAbove && !topLeft.enabled;
        topR.enabled = !isWallAbove && !topRight.enabled;
        bottomL.enabled = !isWallBelow && !bottomLeft.enabled;
        bottomR.enabled = !isWallBelow && !bottomRight.enabled;

        left.enabled = !isWallLeft;// && (!topLeft.enabled || !bottomLeft.enabled);
        right.enabled = !isWallRight;// && (!topRight.enabled || !bottomRight.enabled);




        //left.enabled = !isWallLeft;
        //right.enabled = !isWallRight;
        //top.enabled = !isWallAbove;
        //bottom.enabled = !isWallBelow;

        // kill unnecessary overlapping
        //if (topLeft.enabled && topRight.enabled)
        //    top.enabled = false;
        //if (bottomLeft.enabled && bottomRight.enabled)
        //    bottom.enabled = false;

        //if (topLeft.enabled && bottomLeft.enabled)
        //    left.enabled = false;
        //if (topRight.enabled && bottomRight.enabled)
        //    right.enabled = false;
    }

    [ContextMenu("Update All Walls Edges (Slow!)")]
    public void UpdateAllWallEdges()
    {
        UpdateAllWallEdgesFunc();
    }

    private static void UpdateAllWallEdgesFunc()
    {
        var walls = GameObject.FindObjectsOfType<Wall>();
        foreach (var wall in walls)
        {
            wall.UpdateEdges();
        }
    }

    public static bool IsThereAWallAt(int x, int y, Wall[] walls = null)
    {
        if (walls == null)
        {
            // this is dangerous...
            walls = GameObject.FindObjectsOfType<Wall>();
        }
        var match = walls.FirstOrDefault(w => Mathf.RoundToInt(w.transform.position.x) == x && Mathf.RoundToInt(w.transform.position.y) == y);
        return match != null;
    }

    // skinning
    public void SetSkin(Texture2D skin, WallSideFlags sides)
    {
        if(sides == WallSideFlags.All)
            transform.FindChild("Bg").GetComponent<SkinableSprite>().SetSkin(skin);

        if (sides.HasFlags(WallSideFlags.Top))
        {
            transform.FindChild("TopL").GetComponent<SkinableSprite>().SetSkin(skin);
            transform.FindChild("TopR").GetComponent<SkinableSprite>().SetSkin(skin);
        }

        if (sides.HasFlags(WallSideFlags.Left))
            transform.FindChild("Left").GetComponent<SkinableSprite>().SetSkin(skin);

        if (sides.HasFlags(WallSideFlags.Bottom))
        {
            transform.FindChild("BottomL").GetComponent<SkinableSprite>().SetSkin(skin);
            transform.FindChild("BottomR").GetComponent<SkinableSprite>().SetSkin(skin);
        }

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