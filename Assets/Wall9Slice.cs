using UnityEngine;
using System.Collections;

public class Wall9Slice : MonoBehaviour 
{
    public Sprite SpriteTopLeft;
    public Sprite SpriteTop;
    public Sprite SpriteTopRight;
    public Sprite SpriteRight;
    public Sprite SpriteBottomRight;
    public Sprite SpriteBottom;
    public Sprite SpriteBottomLeft;
    public Sprite SpriteLeft;
    public Sprite SpriteCenter;
    public Sprite SpriteTopLeftInside;
    public Sprite SpriteTopRightInside;
    public Sprite SpriteBottomRightInside;
    public Sprite SpriteBottomLeftInside;

	// Use this for initialization
	void Start () 
    {
	    // look for other walls around me and set each slice to the appropriate sprite
        var gridPos = Arena.Instance.WorldToGridPosition(transform.position);
        var x = (int)gridPos.x;
        var y = (int)gridPos.y;

        if(Arena.Instance.IsThereAWallAt(x + 1, y + 1))
        {

        }
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

}
