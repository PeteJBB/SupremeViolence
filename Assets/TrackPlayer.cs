using UnityEngine;
using System.Collections;
using System.Linq;

public class TrackPlayer : MonoBehaviour {

    private float maxSpeed = 6f; // world units per second

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
            var path = Player.transform.position - transform.position;
            path.z = 0;

//            if(path.magnitude < maxSpeed * Time.deltaTime)
//            {
//                transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
//            }
//            else 
            if(path.magnitude > maxSpeed * Time.deltaTime)
            {
                path = path.normalized * maxSpeed * Time.deltaTime;
            }

            transform.position += path;
        }
	}
}
