using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteCache
{
    public static Dictionary<SpriteCacheKey, Sprite> spriteDic = new Dictionary<SpriteCacheKey, Sprite>();
    public static Sprite GetOrCreateSprite(Texture2D tex, Rect rect, Vector2 pivot)
    {
        var key = new SpriteCacheKey(tex,rect,pivot);
        if(!Helper.IsEditMode() && spriteDic.ContainsKey(key))
        {
            return spriteDic[key];
        }

        // create a new sprite
        //Debug.Log("New sprite added to cache: " + tex.name + ", " + rect + ", " + pivot);
        var sprite = Sprite.Create(tex, rect, pivot);

        if (!Helper.IsEditMode())
        {
            spriteDic.Add(key, sprite);
        }
        return sprite;
    }

    public static Sprite GetOrCreateSprite(Texture2D tex, Sprite original)
    {
        if (tex == null)
            return original;

        return GetOrCreateSprite(tex, original.rect, new Vector2(original.pivot.x / original.rect.width, original.pivot.y / original.rect.height));
    }
	
    public class SpriteCacheKey
    {
        Texture2D tex;
        Rect rect;
        Vector2 pivot;

        public SpriteCacheKey(Texture2D tex, Rect rect, Vector2 pivot)
        {
            this.tex = tex;
            this.rect = rect;
            this.pivot = pivot;
        }
        public override bool Equals(object obj)
        {
            var key = obj as SpriteCacheKey;
            if(key == null)
                return false;

            return this.tex == key.tex 
                && this.rect == key.rect 
                && this.pivot == key.pivot;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + tex.GetHashCode();
            hash = hash * 23 + rect.GetHashCode();
            hash = hash * 23 + pivot.GetHashCode();
            return hash;
        }
    }
}
