using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XInputDotNetPure;

public class PlayerControl : MonoBehaviour 
{
    public GameObject CameraPrefab;

    private float baseLegStrength = 34f;
    private float baseMass = 1;

    public int PlayerIndex;
    public bool FreezeControl = false;

    public List<Pickup> Pickups = new List<Pickup>();
    public Pickup CurrentWeapon;
    public AudioClip[] CollectPickupSounds; // one sound is chosen at random when collecting a pickup

    public float AimingAngle = 90;
    private bool triggerDown = false;

    [HideInInspector]
    private Vector2 viewOffset = Vector2.zero; // used to work out where the camera is centered

    private Rigidbody2D rbody;

    private bool muteSounds = true; // sounds muted while we assign pickups etc
    private Orientation orientation = Orientation.Down;

    public Vector2 AimingOffsetUp;
    public Vector2 AimingOffsetDown;
    public Vector2 AimingOffsetLeft;
    public Vector2 AimingOffsetRight;

    private GamePadState lastGamePadState;

    private bool isFirstUpdate = true;

    private FillBar ammobar;
    //private SpriteRenderer weaponIcon;
    private WeaponCycler weaponCycler;

    private Pickup ammoBarSource;

	// Use this for initialization
	void Awake () 
    {
        // make sure mass is right
        rbody = this.GetComponent<Rigidbody2D>();
        rbody.mass = baseMass;
        
        if (!GameState.IsGameStarted)
                GameState.StartNewGame();

        muteSounds = true;
        Pickups = new List<Pickup>();

        ammobar = transform.FindChild("UI/ammobar").GetComponent<FillBar>();        
        weaponCycler = GetComponentInChildren<WeaponCycler>();
        
    }

    void Start () 
    {
        CreateCamera();

        // initialise pickups bought in shop / carried over from previous match
        foreach(var ps in GameState.Players[PlayerIndex].PickupStates)
        {
            var pickup = ps.Instantiate();
            pickup.PickupSound = null;
            pickup.CollectPickup(this);
        }

        SelectNextWeapon();
        ammoBarSource = CurrentWeapon;

        GameBrain.Instance.OnGameOver.AddListener(OnGameOver);
        BroadcastMessage("SetOrientation", orientation);
        BroadcastMessage("SetAnimationSpeed", 0f);

        var uiLayer = LayerMask.NameToLayer("UIPlayer" + (PlayerIndex + 1));
        Helper.SetLayerRecursive(transform.Find("UI/weaponCycler").gameObject, uiLayer);
        Helper.SetLayerRecursive(transform.Find("UI/ammobar").gameObject, uiLayer);
	}

    private void CreateCamera()
    {
        if (CameraPrefab != null)
        {
            var cam = Instantiate(CameraPrefab).GetComponent<Camera>();
            cam.orthographicSize = 4;
            cam.name = "PlayerCamera" + PlayerIndex;
            var track = cam.GetComponent<PlayerCamera>();
            track.PlayerIndex = PlayerIndex;

            if (PlayerIndex > 0)
                cam.GetComponent<AudioListener>().enabled = false;

            // set camera rect
            var w = 0.5f;
            var h = GameSettings.NumberOfPlayers > 2 ? 0.5f : 1;
            var x = (PlayerIndex % 2) * 0.5f;
            var y = GameSettings.NumberOfPlayers < 3 || PlayerIndex > 1
                ? 0f
                    : 0.5f;

            cam.rect = new Rect(x, y, w, h);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        if(isFirstUpdate)
        {
            muteSounds = false;
            isFirstUpdate = false;
        }

        if(GameBrain.Instance.State == PlayState.GameOn && !FreezeControl)
        {
            var gamepadState = GetGamePadInput();

            // move
            var moveInput = new Vector2(gamepadState.ThumbSticks.Left.X, gamepadState.ThumbSticks.Left.Y);
            var moveForce = moveInput * baseLegStrength * GetLegStrengthMultiplier();
            //if(gamepadState.Buttons.A == ButtonState.Pressed)
            //    moveForce *= 1.3f;
            rbody.AddForce(moveForce * Time.deltaTime);

            // aim
            var aimInput = new Vector2(gamepadState.ThumbSticks.Right.X, gamepadState.ThumbSticks.Right.Y);
            viewOffset = aimInput;// *2;
            if (aimInput.magnitude == 0)
                aimInput = moveInput;


            if (aimInput.magnitude > 0)
            {
                // rotate to face input dir
                float angle = Mathf.Rad2Deg * Mathf.Atan2(-aimInput.x, aimInput.y);
                if (angle >= -45 && angle < 45)
                {
                    orientation = Orientation.Up;                    
                }
                else if (angle >= 45 && angle < 135)
                {
                    orientation = Orientation.Left;
                }
                else if (angle >= -135 && angle < -45)
                {
                    orientation = Orientation.Right;
                }
                else if (angle >= 135 || angle < -135)
                {
                    orientation = Orientation.Down;
                }
                BroadcastMessage("SetOrientation", orientation);

                // update aim angle for pickups to use
                AimingAngle = angle;
            }

            // change weapon
            if (gamepadState.Buttons.LeftShoulder == ButtonState.Pressed && lastGamePadState.Buttons.LeftShoulder == ButtonState.Released)
            {
                SelectNextWeapon(-1);
            }
            if (gamepadState.Buttons.RightShoulder == ButtonState.Pressed && lastGamePadState.Buttons.RightShoulder == ButtonState.Released)
            {
                SelectNextWeapon();
            }

    		// shoot?
            if (gamepadState.Triggers.Right > 0.5f)
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

            lastGamePadState = gamepadState;
        }

        // update weapon icon and ammo bars
        if (ammoBarSource != null)
        {
            ammobar.SetFill(ammoBarSource.GetAmmoBarValue());
        }

        BroadcastMessage("SetAnimationSpeed", rbody.velocity.magnitude);
	}

    public void SetAmmoBarSource(Pickup p, Color color)
    {
        SetAmmoBarSource(p, new FillBarColorPoint[] { new FillBarColorPoint(color, 0) });
    }

    public void SetAmmoBarSource(Pickup p, FillBarColorPoint[] colors = null)
    {
        ammoBarSource = p;
        weaponCycler.SetIcon(p.Icon);

        if (colors == null)
            colors = new FillBarColorPoint[] { new FillBarColorPoint(Color.yellow, 0) };

        ammobar.Colors = colors;
    }

    public void RestoreAmmoBarSource()
    {
        SetAmmoBarSource(CurrentWeapon);
        weaponCycler.ClearIcon(CurrentWeapon.Icon);
        ammobar.Colors = new FillBarColorPoint[] { new FillBarColorPoint(Color.yellow, 0) };
    }

    public void OnGameOver()
    {
        if(triggerDown)
        {
            triggerDown = false;
            if(CurrentWeapon != null)
            {
                CurrentWeapon.OnFireUp(GetAimingOrigin());
            }
        }

        // update weapons on playerinfo object
        var info = GameState.Players[PlayerIndex];
        info.PickupStates.Clear();
        info.PickupStates = Pickups.Select(x => PickupState.FromPickupInstance(x)).ToList();
    }

    private GamePadState GetGamePadInput()
    {
        switch(PlayerIndex)
        {
            case 0:
            default:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.One);
            case 1:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Two);
            case 2:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Three);
            case 3:
                return GamePad.GetState(XInputDotNetPure.PlayerIndex.Four);
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
            case Orientation.Up:
                return transform.position.ToVector2() + AimingOffsetUp;
            case Orientation.Down:
            default:
                return transform.position.ToVector2() + AimingOffsetDown;
        }
    }

    /// <summary>
    /// Used by the camera to track where you are looking with the right stick
    /// </summary>
    /// <returns></returns>
    public Vector2 GetAimingViewPoint()
    {
        return transform.position.ToVector2() + viewOffset;
    }

    void OnDrawGizmos()
    {
        // show the Aiming Origin markers - this is where bullets will originate from in each orientation
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

        var weapons = Pickups.Where(x => x.PickupType == PickupType.Weapon).ToList();
        if(CurrentWeapon == null)
        {
            // just pick the first available weapon
            CurrentWeapon = weapons.FirstOrDefault();
        }
        else
        {
            CurrentWeapon.DeselectWeapon();

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
            CurrentWeapon.SelectWeapon();

        SetAmmoBarSource(CurrentWeapon);
        weaponCycler.Cycle(dir);
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
