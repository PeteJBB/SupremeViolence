﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpriteSorter: MonoBehaviour 
{
    public int SortOrderOffset = 0;
    public Transform SourceTransform;
    public bool UseParentTransform = false;

    private SpriteRenderer spriteRenderer;
    private TrailRenderer trail;
    private Renderer renderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
        renderer = GetComponent<Renderer>();

        SetSortOrder();
    }

	void Update () 
    {
        SetSortOrder();
	}

    private void SetSortOrder()
    {
        int pos;
        if (SourceTransform != null)
            pos = GetOrderByYPosition(SourceTransform.position.y);
        else if(UseParentTransform)
            pos = GetOrderByYPosition(transform.parent.position.y);
        else
            pos = GetOrderByYPosition(transform.position.y);

        if(spriteRenderer != null)
            spriteRenderer.sortingOrder = pos + SortOrderOffset;

        if(trail != null)
            trail.sortingOrder = pos + SortOrderOffset;

        if(renderer != null)
            renderer.sortingOrder = pos + SortOrderOffset;
    }

    public static int GetOrderByYPosition(float y)
    {
        // objects are sorted thusly
        // everything is ordered from 1-100 by its y pos in the square
        // the next 100 numbers are skipped - this allows for an offset which will place
        // this sprite above all others in the square without overlapping the next square

        // lights are offset at +100, top and side walls at +150

        var yoffset = y - 0.5f;
        //var rounded = Mathf.FloorToInt(yoffset);
        //var rem = yoffset % 1f;
        //var z = rounded * -200 + rem * -100;
        var z = yoffset * -100;
        return Mathf.RoundToInt(z);
    }
}