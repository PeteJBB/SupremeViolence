using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

    private float baseLegStrength = 4f;
    private float baseMass = 1;

    public int PlayerNumber = 1;

    public List<GameObject> StartingPickups = new List<GameObject>();
    public List<Pickup> Pickups = new List<Pickup>();

    public AudioClip[] CollectPickupSounds;

    private Animator animator;
    private int walkUpHash = Animator.StringToHash("WalkUp");
    private int walkLeftHash = Animator.StringToHash("WalkLeft");
    private int walkDownHash = Animator.StringToHash("WalkDown");
    private int walkRightHash = Animator.StringToHash("WalkRight");
    private int currentAnimHash;

    public float AimingAngle = 0;
    private bool triggerDown = false;

    private Rigidbody2D rigidbody;

    private bool muteSounds = true;

	// Use this for initialization
	void Start () 
    {
        animator = this.GetComponent<Animator>();
        rigidbody = this.GetComponent<Rigidbody2D>();

        // turn startingpickups into actual pickup instances
        foreach(var p in StartingPickups)
        {
            var instance = Instantiate(p);
            var pickup = instance.GetComponent<Pickup>();
            pickup.BaseStart();
            pickup.PickupSound = null;
            pickup.CollectPickup(this);
        }

        // make sure mass is right
        rigidbody.mass = baseMass;

        // set initial location
        var arena = Transform.FindObjectOfType<Arena>();
        var emptySpots = arena.GetEmptyGridSpots();
        
        // choose a random spot
        var spot = emptySpots[Random.Range(0,emptySpots.Count)];
        transform.position = arena.GridToWorldPosition(spot);

        muteSounds = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(GameBrain.Instance.State == GameState.GameOn)
        {
            var input = new Vector2(Input.GetAxis("XboxAxisXJoy" + PlayerNumber), Input.GetAxis("XboxAxisYJoy" + PlayerNumber));
            rigidbody.AddForce(input * baseLegStrength * GetLegStrengthMultiplier());
            animator.SetFloat("Speed", rigidbody.velocity.magnitude);

            if(input.magnitude > 0)
            {
        		// rotate to face input dir
                float angle = Mathf.Rad2Deg * Mathf.Atan2(-input.x, input.y);
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

                // update aim angle for pickups to use
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
	}

    float GetLegStrengthMultiplier()
    {
        var strength = baseLegStrength;
        foreach(var pickup in Pickups)
        {
            strength *= pickup.GetLegStrengthMultiplier();
        }
        
        return strength;
    }

    public void AddPickup(Pickup pickup)
    {
        Pickups.Add(pickup);
        RecalculateMass();

        if (!muteSounds && CollectPickupSounds.Length > 0)
        {
            var sound = CollectPickupSounds [Random.Range(0, CollectPickupSounds.Length)];
            AudioSource.PlayClipAtPoint(sound, transform.position);
        }
    }

    public void RemovePickup(Pickup pickup)
    {
        Pickups.Remove(pickup);
        RecalculateMass();
    }

    public void RecalculateMass()
    {
        var mass = baseMass;
        foreach(var pu in Pickups)
        {
            mass += pu.GetMass();
        }
        rigidbody.mass = mass;
    }
}
