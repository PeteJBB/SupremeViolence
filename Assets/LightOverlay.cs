using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class LightOverlay : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        var gridPos = Arena.WorldToGridPosition(transform.position);
        if (Wall.IsThereAWallAt((int)(gridPos.x + 1), (int)gridPos.y))
        {
            var wallEdge = gridPos.x + 0.5f;
            var offset = wallEdge - transform.position.x;
            var boundary = (spriteRenderer.sprite.texture.width / 2f) + (offset * spriteRenderer.sprite.pixelsPerUnit) ;

            TrimLight(Mathf.RoundToInt(boundary), spriteRenderer.sprite.texture.width);
        }
        if (Wall.IsThereAWallAt((int)(gridPos.x - 1), (int)gridPos.y))
        {
            var wallEdge = gridPos.x - 0.5f;
            var offset = wallEdge - transform.position.x;
            var boundary = (spriteRenderer.sprite.texture.width / 2f) + (offset * spriteRenderer.sprite.pixelsPerUnit) ;

            TrimLight(0, Mathf.RoundToInt(boundary));
        }
        
        
        //gridPos.x += 1;
        //var sq = Arena.Instance.GetGridSquare(gridPos);
        //if (sq.GridSquareType == GridSquareType.Wall)
        //{
        //    Debug.Log("Trimming!");
        //    TrimLight();
        //}
    }

    // Update is called once per frame
    void TrimLight(int from, int to)
    {
        // duplicate the original texture and assign to the material
        var texture = Instantiate(spriteRenderer.sprite.texture) as Texture2D;

        // colors used to tint the first 3 mip levels
        Color[] colors = new Color[3];
        colors[0] = Color.red;
        colors[1] = Color.green;
        colors[2] = Color.blue;

        var cols = texture.GetPixels();
        for (var i = 0; i < cols.Length; i++)
        {
            var col = i % texture.width;
            if(col >= from && col < to)
                cols[i] = new Color(0,0,0,0);
        }
        texture.SetPixels(cols);

        // actually apply all SetPixels, don't recalculate mip levels
        texture.Apply(false);

        var sprite = Sprite.Create(texture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
    }
}