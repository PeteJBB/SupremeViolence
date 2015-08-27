using UnityEngine;
using System.Collections;
using System.Linq;

public class TrackPlayer : MonoBehaviour {

    private float minSpeed = 1f; // world units per second
    private float maxSpeed = 10f; // world units per second

    public int PlayerIndex;
    public PlayerControl Player;

	// Use this for initialization
	void Start () 
    {

	}
	
	// LateUpdate is called once per frame after Update
	void Update () 
    {
        if(Player == null)
        {
            Player = FindObjectsOfType<PlayerControl>().FirstOrDefault(x => (int)x.PlayerIndex == this.PlayerIndex);
            if(Player != null)
            {
                var pos = Player.transform.position;
                pos.z = transform.position.z;
                transform.position = pos;
            }
        }

        if(Player != null)
        {
            var target = Player.GetAimingViewPoint();
            var path = target - transform.position.ToVector2();

            var speed = Mathf.Clamp(path.magnitude, minSpeed, maxSpeed);
            if(path.magnitude > speed * Time.deltaTime)
            {
                path = ((path.normalized * speed) + Player.GetComponent<Rigidbody2D>().velocity) * Time.deltaTime;
            }

            transform.position += path.ToVector3(0);
        }
	}
}
