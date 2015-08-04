using UnityEngine;
using System.Collections;


public class SkinableSprite : MonoBehaviour 
{
    public Texture2D Skin;

    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

	// Use this for initialization
	void Awake () 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;

        if(Skin != null)
        {
            SetSkin(Skin);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    public void SetSkin(Texture2D skin)
    {
        if(originalSprite != null)
        {
            Skin = skin;
            var rect = originalSprite.rect;
            var pivot = new Vector2(originalSprite.pivot.x / originalSprite.rect.width, originalSprite.pivot.y / originalSprite.rect.height);
            var newSprite = SpriteCache.GetOrCreateSprite(skin, rect, pivot);
            spriteRenderer.sprite = newSprite;
        }
    }
}
