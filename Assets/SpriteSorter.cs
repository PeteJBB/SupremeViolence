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
    }

	void Update () 
    {
	    int pos = Mathf.RoundToInt(transform.position.y * -100);

        if(spriteRenderer != null)
            spriteRenderer.sortingOrder = pos + SortOrderOffset;

        if(renderer != null)
            renderer.sortingOrder = pos + SortOrderOffset;
	}
}