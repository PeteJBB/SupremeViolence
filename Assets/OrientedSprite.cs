using UnityEngine;
using System.Collections;

public class OrientedSprite : MonoBehaviour 
{
    public Sprite SpriteUp;
    public Sprite SpriteDown;
    public Sprite SpriteLeft;
    public Sprite SpriteRight;

    SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
    public void SetOrientation(Orientation o)
    {
        switch(o)
        {
            case Orientation.Up:
                spriteRenderer.sprite = SpriteUp;
                break;
            case Orientation.Down:
                spriteRenderer.sprite = SpriteDown;
                break;
            case Orientation.Left:
                spriteRenderer.sprite = SpriteLeft;
                break;
            case Orientation.Right:
                spriteRenderer.sprite = SpriteRight;
                break;
        }
    }
}

public enum Orientation
{
    Up,
    Down,
    Left,
    Right
}
