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
    private Orientation lastOrientation = Orientation.Unknown;

    SpriteRenderer spriteRenderer;


    public bool AdjustSortingLayer = false;

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
                    if(AdjustSortingLayer)
                        spriteRenderer.sortingLayerName = "Bg_Front";
                    break;
                case Orientation.Down:
                    spriteRenderer.sprite = SpriteDown;
                    if(AdjustSortingLayer)
                        spriteRenderer.sortingLayerName = "Fore_Back";
                    break;
                case Orientation.Left:
                    spriteRenderer.sprite = SpriteLeft;
                    if(AdjustSortingLayer)
                        spriteRenderer.sortingLayerName = "Fore_Back";
                    break;
                case Orientation.Right:
                    spriteRenderer.sprite = SpriteRight;
                    if(AdjustSortingLayer)
                        spriteRenderer.sortingLayerName = "Fore_Back";
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
    Unknown,
    Up,
    Down,
    Left,
    Right
}
