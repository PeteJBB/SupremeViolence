using UnityEngine;
using System.Collections;
//using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class OrientedSprite : MonoBehaviour
{
    public Orientation orientation = Orientation.Down;

    public Sprite SpriteUp;
    public Sprite SpriteDown;
    public Sprite SpriteLeft;
    public Sprite SpriteRight;

    private Orientation lastOrientation = Orientation.Unknown;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private string defaultSortingLayer;
    private Sprite defaultSprite;

    // Use this for initialization
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        lastOrientation = orientation;

        defaultSortingLayer = spriteRenderer.sortingLayerName;
    }

    public void SetAnimationSpeed(float speed)
    {
        if (anim != null && anim.parameters.Any(x => x.name == "Speed"))
        {
            anim.SetFloat("Speed", speed);
        }
    }

    void Update()
    {
        if (orientation != lastOrientation)
        {
            switch (orientation)
            {
                case Orientation.Up:
                    spriteRenderer.sprite = SpriteUp == null ? defaultSprite : SpriteUp;
                    break;
                case Orientation.Down:
                    spriteRenderer.sprite = SpriteDown == null ? defaultSprite : SpriteDown;
                    break;
                case Orientation.Left:
                    spriteRenderer.sprite = SpriteLeft == null ? defaultSprite : SpriteLeft;
                    break;
                case Orientation.Right:
                    spriteRenderer.sprite = SpriteRight == null ? defaultSprite : SpriteRight;
                    break;
            }

            if (anim != null && anim.parameters.Any(x => x.name == orientation.ToString()))
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
