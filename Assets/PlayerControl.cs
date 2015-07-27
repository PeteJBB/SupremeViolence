using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

	private float MoveSpeed = 1f; // 0.01 = 1px per second
    public int PlayerNumber;

    public List<Pickup> Pickups = new List<Pickup>();

    private Animator animator;
    private int walkUpHash = Animator.StringToHash("WalkUp");
    private int walkLeftHash = Animator.StringToHash("WalkLeft");
    private int walkDownHash = Animator.StringToHash("WalkDown");
    private int walkRightHash = Animator.StringToHash("WalkRight");
    private int currentAnimHash;

    public float AimingAngle = 0;
    private bool triggerDown = false;

	// Use this for initialization
	void Start () 
    {
        animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        var inputX = Input.GetAxis("XboxAxisXJoy" + PlayerNumber);
        var inputY = Input.GetAxis("XboxAxisYJoy" + PlayerNumber);

		// move
		var pos = transform.position;
        var multiplier = GetTotalMoveMultiplier();
        pos.x += inputX * multiplier * MoveSpeed * Time.deltaTime;
        pos.y += inputY * multiplier * MoveSpeed * Time.deltaTime;

        var speed = new Vector2(inputX, inputY).magnitude;
        animator.SetFloat("Speed", speed);

		if(pos != transform.position)
		{
			transform.position = pos;

			// rotate to face movement dir
//			float angle = Mathf.Atan2(-inputX, inputY);
//			var rot = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.forward);
//			transform.rotation = rot;

            float angle = Mathf.Rad2Deg * Mathf.Atan2(-inputX, inputY);
            if(angle >= -45 && angle < 45 && currentAnimHash != walkUpHash)
            {
                animator.SetTrigger(walkUpHash);
                currentAnimHash = walkUpHash;
            }
            else if(angle >= 45 && angle < 135 && currentAnimHash != walkLeftHash)
            {
                animator.SetTrigger(walkLeftHash);
                currentAnimHash = walkLeftHash;
            }
            else if(angle >= -135 && angle < -45 && currentAnimHash != walkRightHash)
            {
                animator.SetTrigger(walkRightHash);
                currentAnimHash = walkRightHash;
            }
            else if((angle >= 135 || angle < -135) && currentAnimHash != walkDownHash)
            {
                animator.SetTrigger(walkDownHash);
                currentAnimHash = walkDownHash;
            }

            AimingAngle = angle;
		}

		// shoot?
        //if (Input.GetKeyDown("joystick " + (playerNumber)+ " button 0"))
        if (Input.GetAxis("XboxAxis3Joy" + PlayerNumber) < 0)
		{
            if(!triggerDown)
            {
                triggerDown = true;
                foreach(var p in Pickups)
                {
                    p.OnFireDown(this);
                }
            }
		}
        else if(triggerDown)
        {
            triggerDown = false;
            foreach(var p in Pickups)
            {
                p.OnFireUp(this);
            }
        }
	}

    float GetTotalMoveMultiplier()
    {
        var speed = MoveSpeed;
        foreach(var pickup in Pickups)
        {
            speed *= pickup.GetMoveMultiplier();
        }

        return speed;
    }
}
