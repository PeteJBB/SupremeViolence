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

    public bool UseParentRotation = false;

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
        if (UseParentRotation)
        {
            var angle = (transform.parent.rotation.eulerAngles.z + 360) % 360;
            transform.rotation = Quaternion.identity;

            if (angle >= 315 || angle < 45)
            {
                orientation = Orientation.Up;                    
            }
            else if (angle >= 45 && angle < 135)
            {
                orientation = Orientation.Left;
            }
            else if (angle >= 225 && angle < 315)
            {
                orientation = Orientation.Right;
            }
            else if (angle >= 135 || angle < 225)
            {
                orientation = Orientation.Down;
            }
        }

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
