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


    public float AimingAngle = 90;
    private bool triggerDown = false;

    private Rigidbody2D rbody;

    private bool muteSounds = true; // sounds muted while we assign pickups etc
    private Orientation orientation = Orientation.Down;

    public Vector2 CurrentGridPos;
    public int Score = 0;

    public Vector2 AimingOffsetUp;
    public Vector2 AimingOffsetDown;
    public Vector2 AimingOffsetLeft;
    public Vector2 AimingOffsetRight;


	// Use this for initialization
	void Start () 
    {
        // make sure mass is right
        rbody = this.GetComponent<Rigidbody2D>();
        rbody.mass = baseMass;

        // turn startingpickups into actual pickup instances
        muteSounds = true;
        foreach(var p in StartingPickups)
        {
            var instance = Instantiate(p);
            var pickup = instance.GetComponent<Pickup>();
            pickup.BaseStart();
            pickup.PickupSound = null;
            pickup.CollectPickup(this);
        }

        SelectNextWeapon();

        // enable sounds
        muteSounds = false;

        BroadcastMessage("SetOrientation", orientation);
        BroadcastMessage("SetAnimationSpeed", 0f);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(GameBrain.Instance.State == GameState.GameOn)
        {
            var input = new Vector2(Input.GetAxis("XboxAxisXJoy" + PlayerNumber), Input.GetAxis("XboxAxisYJoy" + PlayerNumber));
            rbody.AddForce(input * baseLegStrength * GetLegStrengthMultiplier());
            BroadcastMessage("SetAnimationSpeed", rbody.velocity.magnitude);

            if(input.magnitude > 0)
            {
        		// rotate to face input dir
                float angle = Mathf.Rad2Deg * Mathf.Atan2(-input.x, input.y);
                if(angle >= -45 && angle < 45)// && currentAnimHash != walkUpHash)
                {
                    orientation = Orientation.Up;
                }
                else if(angle >= 45 && angle < 135)// && currentAnimHash != walkLeftHash)
                {
                    orientation = Orientation.Left;
                }
                else if(angle >= -135 && angle < -45)// && currentAnimHash != walkRightHash)
                {
                    orientation = Orientation.Right;
                }
                else if(angle >= 135 || angle < -135)// && currentAnimHash != walkDownHash)
                {
                    orientation = Orientation.Down;
                }
                BroadcastMessage("SetOrientation", orientation);

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
                SelectNextWeapon(-1);
            }
            if (Input.GetKeyDown("joystick " + (PlayerNumber)+ " button 5"))
            {
                SelectNextWeapon();
            }

    		// shoot?
            if (Input.GetAxis("XboxAxis3Joy" + PlayerNumber) < 0)
    		{
                if(!triggerDown)
                {
                    triggerDown = true;
                    if(CurrentWeapon != null)
                    {
                        CurrentWeapon.OnFireDown(GetAimingOrigin());
                    }
                }
    		}
            else if(triggerDown)
            {
                triggerDown = false;
                if(CurrentWeapon != null)
                {
                    CurrentWeapon.OnFireUp(GetAimingOrigin());
                }
            }
        }
	}

    /// <summary>
    /// Get the position of the player's gun
    /// This changes depending on player orientation
    /// </summary>
    /// <returns>The aiming origin.</returns>
    public Vector2 GetAimingOrigin()
    {
        switch(orientation)
        {
            case Orientation.Left:
                return transform.position.ToVector2() + AimingOffsetLeft;
            case Orientation.Right:
                return transform.position.ToVector2() + AimingOffsetRight;
                break;
            case Orientation.Up:
                return transform.position.ToVector2() + AimingOffsetUp;
                break;
            case Orientation.Down:
            default:
                return transform.position.ToVector2() + AimingOffsetDown;
        }
    }

    void OnDrawGizmos()
    {
        var cubeSize = 0.01f;
        Gizmos.DrawWireCube(transform.position + AimingOffsetUp.ToVector3(transform.position.z), new Vector3(cubeSize, cubeSize, cubeSize));
        Gizmos.DrawWireCube(transform.position + AimingOffsetDown.ToVector3(transform.position.z), new Vector3(cubeSize, cubeSize, cubeSize));
        Gizmos.DrawWireCube(transform.position + AimingOffsetLeft.ToVector3(transform.position.z), new Vector3(cubeSize, cubeSize, cubeSize));
        Gizmos.DrawWireCube(transform.position + AimingOffsetRight.ToVector3(transform.position.z), new Vector3(cubeSize, cubeSize, cubeSize));
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
            Debug.Log(sound.name);
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
            CurrentWeapon.OnFireUp(GetAimingOrigin());
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
                index = (weapons.Count + index + dir) % weapons.Count;
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
        rbody.mass = mass;
    }
}
