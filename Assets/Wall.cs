using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[ExecuteInEditMode]
public class Wall : MonoBehaviour
{
    public Vector2 Size;

    private Sprite bg;

    private Sprite topL;
    private Sprite topR;
    private Sprite leftT;
    private Sprite leftB;
    private Sprite rightT;
    private Sprite rightB;
    private Sprite bottomL;
    private Sprite bottomR;

    private Sprite topLeft;
    private Sprite topRight;
    private Sprite bottomLeft;
    private Sprite bottomRight;

    private Sprite topLeftInner;
    private Sprite topRightInner;
    private Sprite bottomLeftInner;
    private Sprite bottomRightInner;

    public Texture2D SkinBg;
    public Texture2D SkinTopLeft;
    public Texture2D SkinTopRight;
    public Texture2D SkinBottomLeft;
    public Texture2D SkinBottomRight;

    private Texture2D z_SkinBg;
    private Texture2D z_SkinTopLeft;
    private Texture2D z_SkinTopRight;
    private Texture2D z_SkinBottomLeft;
    private Texture2D z_SkinBottomRight;
    private Vector3 z_position;

    void Start()
    {
        z_SkinBg = z_SkinBg;
        z_SkinTopLeft = SkinTopLeft;
        z_SkinTopRight = SkinTopRight;
        z_SkinBottomLeft = SkinBottomLeft;
        z_SkinBottomRight = SkinBottomRight;
        z_position = transform.position;
    }

    void Update()
    {
        if (Helper.IsEditMode())
        {
            if (z_SkinBg != z_SkinBg
                || z_SkinBg != SkinBg
                || z_SkinTopLeft != SkinTopLeft
                || z_SkinTopRight != SkinTopRight
                || z_SkinBottomLeft != SkinBottomLeft
                || z_SkinBottomRight != SkinBottomRight
                || z_position != transform.position
            )
            {
                z_SkinBg = z_SkinBg;
                z_SkinTopLeft = SkinTopLeft;
                z_SkinTopRight = SkinTopRight;
                z_SkinBottomLeft = SkinBottomLeft;
                z_SkinBottomRight = SkinBottomRight;
                z_position = transform.position;

                var allWalls = GameObject.FindObjectsOfType<Wall>();
                var walls = allWalls.Where(w => Mathf.Abs(w.transform.position.x - transform.position.x) <= 1 || Mathf.Abs(w.transform.position.y - transform.position.y) <= 1).ToList();
                foreach (var w in walls)
                    w.UpdateEdges(allWalls);

                //UpdateEdges();
            }
        }
    }

    void LoadSprites()
    {
        var atlas = GameSettings.WallSprites;

        bg = atlas.First(x => x.name == "wall_bg");
        topL = atlas.First(x => x.name == "wall_top_l");
        topR = atlas.First(x => x.name == "wall_top_r");
        leftT = atlas.First(x => x.name == "wall_left_t");
        leftB = atlas.First(x => x.name == "wall_left_b");
        rightT = atlas.First(x => x.name == "wall_right_t");
        rightB = atlas.First(x => x.name == "wall_right_b");
        bottomL = atlas.First(x => x.name == "wall_bottom_l");
        bottomR = atlas.First(x => x.name == "wall_bottom_r");

        topLeft = atlas.First(x => x.name == "wall_topleft");
        topRight = atlas.First(x => x.name == "wall_topright");
        bottomLeft = atlas.First(x => x.name == "wall_bottomleft");
        bottomRight = atlas.First(x => x.name == "wall_bottomright");

        topLeftInner = atlas.First(x => x.name == "wall_topleft_inner");
        topRightInner = atlas.First(x => x.name == "wall_topright_inner");
        bottomLeftInner = atlas.First(x => x.name == "wall_bottomleft_inner");
        bottomRightInner = atlas.First(x => x.name == "wall_bottomright_inner");
    }

    [ContextMenu("Update Edges")]
    public void UpdateEdges(Wall[] walls = null)
    {
        RevealSprites();
        ApplySkin();

        if (walls == null)
        {
            walls = GameObject.FindObjectsOfType<Wall>();
        }

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

        HideSprites();
    }

    [ContextMenu("Reveal Sprites")]
    private void RevealSprites()
    {
        Helper.SetHideFlags(gameObject, HideFlags.None, true);
    }

    [ContextMenu("Hide Sprites")]
    private void HideSprites()
    {
        Helper.SetHideFlags(gameObject, HideFlags.HideInHierarchy, true);
    }

    [ContextMenu("Update All Wall Edges")]
    public void UpdateAllWallEdgesNow()
    {
        UpdateAllWallEdges();
    }

    public static void UpdateAllWallEdges()
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

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 0, 0);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 0.1f));
    }

    // skinning
    [ContextMenu("Apply Skin")]
    public void ApplySkin()
    {
        LoadSprites();

        if (SkinBg != null)
        {
            bg = SpriteCache.GetOrCreateSprite(SkinBg, bg);
        }

        if (SkinTopLeft != null)
        {
            topL = SpriteCache.GetOrCreateSprite(SkinTopLeft, topL);
            topLeft = SpriteCache.GetOrCreateSprite(SkinTopLeft, topLeft);
            topLeftInner = SpriteCache.GetOrCreateSprite(SkinTopLeft, topLeftInner);
            leftT = SpriteCache.GetOrCreateSprite(SkinTopLeft, leftT);
        }

        if (SkinTopRight != null)
        {
            topR = SpriteCache.GetOrCreateSprite(SkinTopRight, topR);
            topRight = SpriteCache.GetOrCreateSprite(SkinTopRight, topRight);
            topRightInner = SpriteCache.GetOrCreateSprite(SkinTopRight, topRightInner);
            rightT = SpriteCache.GetOrCreateSprite(SkinTopRight, rightT);
        }

        if (SkinBottomLeft != null)
        {
            bottomL = SpriteCache.GetOrCreateSprite(SkinBottomLeft, bottomL);
            bottomLeft = SpriteCache.GetOrCreateSprite(SkinBottomLeft, bottomLeft);
            bottomLeftInner = SpriteCache.GetOrCreateSprite(SkinBottomLeft, bottomLeftInner);
            leftB = SpriteCache.GetOrCreateSprite(SkinBottomLeft, leftB);
        }

        if (SkinBottomRight != null)
        {
            bottomR = SpriteCache.GetOrCreateSprite(SkinBottomRight, bottomR);
            bottomRight = SpriteCache.GetOrCreateSprite(SkinBottomRight, bottomRight);
            bottomRightInner = SpriteCache.GetOrCreateSprite(SkinBottomRight, bottomRightInner);
            rightB = SpriteCache.GetOrCreateSprite(SkinBottomRight, rightB);
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
        foreach (var f in flags)
        {
            if ((val & f) != f)
                return false;
        }

        return true;
    }
}