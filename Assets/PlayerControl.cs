using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerControl : MonoBehaviour {

    private float baseLegStrength = 4f;
    private float baseMass = 1;

    public int PlayerNumber = 1;

    public List<GameObject> StartingPickups = new List<GameObject>();
    public List<Pickup> Pickups = new List<Pickup>();
    public Pickup CurrentWeapon;

    public AudioClip[] CollectPickupSounds; // one sound is chosen at random when collecting a pickup

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

    public Vector2 CurrentGridPos;

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

        SelectNextWeapon();

        // make sure mass is right
        rigidbody.mass = baseMass;

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

            // update grid pos
            var gridpos = Arena.Instance.WorldToGridPosition(transform.position);
            if(gridpos != CurrentGridPos)
            {
                CurrentGridPos = gridpos;
                Arena.Instance.SetGridObject(CurrentGridPos, gameObject);
            }

            // change weapon
            if (Input.GetKeyDown("joystick " + (PlayerNumber)+ " button 4"))
            {
                Debug.Log("Prev Weapon");
                SelectNextWeapon(-1);
            }
            if (Input.GetKeyDown("joystick " + (PlayerNumber)+ " button 5"))
            {
                Debug.Log("Next Weapon");
                SelectNextWeapon();
            }

    		// shoot?
            if (Input.GetAxis("XboxAxis3Joy" + PlayerNumber) < 0)
    		{
                if(!triggerDown)
                {
                    triggerDown = true;
                    CurrentWeapon.OnFireDown(this);
                }
    		}
            else if(triggerDown)
            {
                triggerDown = false;
                CurrentWeapon.OnFireUp(this);
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

    public void SelectNextWeapon(int dir = 1) // dir = 1 for next weapon, dir = -1 for prev
    {
        if(triggerDown)
        {
            triggerDown = false;
            CurrentWeapon.OnFireUp(this);
        }

        var weapons = Pickups.Where(x => x.IsWeapon()).ToList();
        if(CurrentWeapon == null)
        {
            // just pick the first available weapon
            CurrentWeapon = weapons.FirstOrDefault();
        }
        else
        {
            CurrentWeapon.OnDeselectWeapon();

            // find the current weapon index and move to the next one
            var index = weapons.IndexOf(CurrentWeapon);
            if(index < 0)
                CurrentWeapon = weapons.FirstOrDefault();
            else
            {
                index = (index + dir) % weapons.Count;
                CurrentWeapon = weapons[index];
            }
        }

        if(CurrentWeapon != null)
            CurrentWeapon.OnSelectWeapon();
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
