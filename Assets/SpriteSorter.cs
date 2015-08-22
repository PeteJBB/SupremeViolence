using UnityEngine;
using System.Collections;

public class SpriteSorter: MonoBehaviour 
{
    public int SortOrderOffset = 0;

    private SpriteRenderer spriteRenderer;
    private Renderer renderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        renderer = GetComponent<Renderer>();

        SetSortOrder();
    }

	void Update () 
    {
        SetSortOrder();
	}

    private void SetSortOrder()
    {
        int pos = GetOrderByYPosition(transform.position.y);

        if(spriteRenderer != null)
            spriteRenderer.sortingOrder = pos + SortOrderOffset;

        if(renderer != null)
            renderer.sortingOrder = pos + SortOrderOffset;
    }

    public static int GetOrderByYPosition(float y)
    {
        return Mathf.RoundToInt(y * -100);
    }
}