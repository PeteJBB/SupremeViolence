using UnityEngine;
using System.Collections;

public class OrientedSprite : MonoBehaviour 
{
    public Sprite SpriteUp;
    public Sprite SpriteDown;
    public Sprite SpriteLeft;
    public Sprite SpriteRight;

    private Animator anim;

    private Orientation orientation;

    SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
	}

    public void SetAnimationSpeed(float speed)
    {
        if(anim != null)
        {
            anim.SetFloat("Speed", speed);
        }
    }

    public void SetOrientation(Orientation o)
    {
        if(orientation != o)
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

            if(anim != null)
            {
                anim.SetTrigger(o.ToString());
            }

            orientation = o;
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
