using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent (typeof (SpriteRenderer))]
public class OrientedSprite : MonoBehaviour 
{
    public Sprite SpriteUp;
    public Sprite SpriteDown;
    public Sprite SpriteLeft;
    public Sprite SpriteRight;

    private Animator anim;

    public Orientation orientation = Orientation.Down;
    private Orientation lastOrientation = Orientation.Down;

    SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        lastOrientation = orientation;
	}

    public void SetAnimationSpeed(float speed)
    {
        if(anim != null)
        {
            anim.SetFloat("Speed", speed);
        }
    }

    void Update()
    {
        if(orientation != lastOrientation)
        {
            switch(orientation)
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
                anim.SetTrigger(orientation.ToString());
            }
            lastOrientation = orientation;
        }
    }

    public void SetOrientation(Orientation o)
    {
        orientation = o;
    }
}

public enum Orientation
{
    Up,
    Down,
    Left,
    Right
}
