using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Wall : MonoBehaviour
{
    public Sprite bg;

    public Sprite topL;
    public Sprite topR;
    public Sprite leftT;
    public Sprite leftB;
    public Sprite rightT;
    public Sprite rightB;
    public Sprite bottomL;
    public Sprite bottomR;

    public Sprite topLeft;
    public Sprite topRight;
    public Sprite bottomLeft;
    public Sprite bottomRight;

    public Sprite topLeftInner;
    public Sprite topRightInner;
    public Sprite bottomLeftInner;
    public Sprite bottomRightInner;

    public Texture2D Skin;
    public WallSideFlags SkinSides;

    void Start()
    {
        UpdateEdges();
    }

    [ContextMenu("Update Edges")]
    public void UpdateEdges()
    {
        if (Skin != null)
            ApplySkin();

        var walls = GameObject.FindObjectsOfType<Wall>();

        var x = Mathf.RoundToInt(transform.position.x);
        var y = Mathf.RoundToInt(transform.position.y);

        // where are other walls
        var isWallAbove = IsThereAWallAt(x, y + 1, walls);
        var isWallBelow = IsThereAWallAt(x, y - 1, walls);
        var isWallLeft = IsThereAWallAt(x - 1, y, walls);
        var isWallRight = IsThereAWallAt(x + 1, y, walls);

        // topleft
        var sr = transform.FindChild("TopLeft").GetComponent<SpriteRenderer>();
        sr.enabled = true;
        if (!isWallAbove && !isWallLeft)
            sr.sprite = topLeft;
        else if (isWallAbove && isWallLeft && !IsThereAWallAt(x - 1, y + 1, walls))
            sr.sprite = topLeftInner;
        else if (!isWallAbove)
            sr.sprite = topL;
        else if (!isWallLeft)
            sr.sprite = leftT;
        else
            sr.enabled = false;

        // topright
        sr = transform.FindChild("TopRight").GetComponent<SpriteRenderer>();
        sr.enabled = true;
        if (!isWallAbove && !isWallRight)
            sr.sprite = topRight;
        else if (isWallAbove && isWallRight && !IsThereAWallAt(x + 1, y + 1, walls))
            sr.sprite = topRightInner;
        else if (!isWallAbove)
            sr.sprite = topR;
        else if (!isWallRight)
            sr.sprite = rightT;
        else
            sr.enabled = false;

        // bottomleft
        sr = transform.FindChild("BottomLeft").GetComponent<SpriteRenderer>();
        sr.enabled = true;
        if (!isWallBelow && !isWallLeft)
            sr.sprite = bottomLeft;
        else if (isWallBelow && isWallLeft && !IsThereAWallAt(x - 1, y - 1, walls))
            sr.sprite = bottomLeftInner;
        else if (!isWallBelow)
            sr.sprite = bottomL;
        else if (!isWallLeft)
            sr.sprite = leftB;
        else
            sr.enabled = false;

        // bottomright
        sr = transform.FindChild("BottomRight").GetComponent<SpriteRenderer>();
        sr.enabled = true;
        if (!isWallBelow && !isWallRight)
            sr.sprite = bottomRight;
        else if (isWallBelow && isWallRight && !IsThereAWallAt(x + 1, y - 1, walls))
            sr.sprite = bottomRightInner;
        else if (!isWallBelow)
            sr.sprite = bottomR;
        else if (!isWallRight)
            sr.sprite = rightB;
        else
            sr.enabled = false;
    }

    [ContextMenu("Update All Wall Edges")]
    public void UpdateAllWallEdges()
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
    [ContextMenu("Apply Skin")]
    public void ApplySkin()
    {
        if (SkinSides == WallSideFlags.All)
        {
            bg = SpriteCache.GetOrCreateSprite(Skin, bg);
        }

        if (SkinSides.HasFlags(WallSideFlags.Top))
        {
            topL = SpriteCache.GetOrCreateSprite(Skin, topL);
            topR = SpriteCache.GetOrCreateSprite(Skin, topR);
        }

        if (SkinSides.HasFlags(WallSideFlags.Left))
        {
            leftT = SpriteCache.GetOrCreateSprite(Skin, leftT);
            leftB = SpriteCache.GetOrCreateSprite(Skin, leftB);
        }

        if (SkinSides.HasFlags(WallSideFlags.Right))
        {
            rightT = SpriteCache.GetOrCreateSprite(Skin, rightT);
            rightB = SpriteCache.GetOrCreateSprite(Skin, rightB);
        }

        if (SkinSides.HasFlags(WallSideFlags.Bottom))
        {
            bottomL = SpriteCache.GetOrCreateSprite(Skin, bottomL);
            bottomR = SpriteCache.GetOrCreateSprite(Skin, bottomR);
        }

        if (SkinSides.HasFlags(WallSideFlags.Top) && SkinSides.HasFlags(WallSideFlags.Left))
        {
            topLeft = SpriteCache.GetOrCreateSprite(Skin, topLeft);
            topLeftInner = SpriteCache.GetOrCreateSprite(Skin, topLeftInner);
        }

        if (SkinSides.HasFlags(WallSideFlags.Top) && SkinSides.HasFlags(WallSideFlags.Right))
        {
            topRight = SpriteCache.GetOrCreateSprite(Skin, topRight);
            topRightInner = SpriteCache.GetOrCreateSprite(Skin, topRightInner);
        }

        if (SkinSides.HasFlags(WallSideFlags.Bottom) && SkinSides.HasFlags(WallSideFlags.Left))
        {
            bottomLeft = SpriteCache.GetOrCreateSprite(Skin, bottomLeft);
            bottomLeftInner = SpriteCache.GetOrCreateSprite(Skin, bottomLeftInner);
        }

        if (SkinSides.HasFlags(WallSideFlags.Bottom) && SkinSides.HasFlags(WallSideFlags.Right))
        {
            bottomRight = SpriteCache.GetOrCreateSprite(Skin, bottomRight);
            bottomRightInner = SpriteCache.GetOrCreateSprite(Skin, bottomRightInner);
        }


        // if(SkinSides == WallSideFlags.All)
        //    transform.FindChild("Bg").GetComponent<SkinableSprite>().SetSkin(Skin);

        //if (SkinSides.HasFlags(WallSideFlags.Top) && SkinSides.HasFlags(WallSideFlags.Left))
        //    transform.FindChild("TopLeft").GetComponent<SkinableSprite>().SetSkin(Skin);

        //if (SkinSides.HasFlags(WallSideFlags.Top) && SkinSides.HasFlags(WallSideFlags.Right))
        //    transform.FindChild("TopRight").GetComponent<SkinableSprite>().SetSkin(Skin);

        //if (SkinSides.HasFlags(WallSideFlags.Bottom) && SkinSides.HasFlags(WallSideFlags.Right))
        //    transform.FindChild("BottomRight").GetComponent<SkinableSprite>().SetSkin(Skin);

        //if (SkinSides.HasFlags(WallSideFlags.Bottom) && SkinSides.HasFlags(WallSideFlags.Left))
        //    transform.FindChild("BottomLeft").GetComponent<SkinableSprite>().SetSkin(Skin);
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
        foreach (var f in flags)
        {
            if ((val & f) != f)
                return false;
        }

        return true;
    }
}