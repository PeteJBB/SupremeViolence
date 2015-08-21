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
	
    [ContextMenu("Apply Skin")]
    public void SetSkin(Texture2D skin)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(originalSprite == null)
            originalSprite = spriteRenderer.sprite;

        if (skin == originalSprite)
                return;

        if (originalSprite != null)
        {
            if (skin != null)
            {
                Skin = skin;
                var rect = originalSprite.rect;
                var pivot = new Vector2(originalSprite.pivot.x / originalSprite.rect.width, originalSprite.pivot.y / originalSprite.rect.height);
                var newSprite = SpriteCache.GetOrCreateSprite(skin, rect, pivot);
                spriteRenderer.sprite = newSprite;
            }
            else
            {
                // restore original
                spriteRenderer.sprite = originalSprite;
            }
        }
    }
}
