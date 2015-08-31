using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerCamera : MonoBehaviour
{
    private float minSpeed = 1f; // world units per second
    private float maxSpeed = 10f; // world units per second

    public int PlayerIndex;
    private Camera camera;

    [HideInInspector]
    public PlayerControl Player;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Use this for initialization
    void Start()
    {
        if (Player == null)
        {
            FindPlayer();
            if (Player != null)
            {
                var pos = Player.transform.position;
                pos.z = transform.position.z;
                transform.position = pos;
            }
        }

        // set up layers
        camera.cullingMask = 1 
            | LayerMask.GetMask("UI") 
            | LayerMask.GetMask("Projectiles") 
            | LayerMask.GetMask("Pickups") 
            | LayerMask.GetMask("Explosions") 
            | LayerMask.GetMask("LowFurniture") 
            | LayerMask.GetMask("Shields") 
            | LayerMask.GetMask("UIPlayer" + (PlayerIndex + 1));
    }

    private void FindPlayer()
    {
        Player = FindObjectsOfType<PlayerControl>().FirstOrDefault(x => (int)x.PlayerIndex == this.PlayerIndex);
    }

    void Update()
    {
        if (Player == null)
        {
            FindPlayer();
        }

        if (Player != null)
        {
            var target = Player.GetAimingViewPoint();
            var path = target - transform.position.ToVector2();

            var speed = Mathf.Clamp(path.magnitude, minSpeed, maxSpeed);
            var move = ((path.normalized * speed) + Player.GetComponent<Rigidbody2D>().velocity) * Time.deltaTime;

            if (move.magnitude > path.magnitude)
                transform.position = target.ToVector3(transform.position.z);
            else
                transform.position += move.ToVector3(0);
        }
    }
}
