using UnityEngine;
using System.Collections;

public class TrackObject : MonoBehaviour {

    public GameObject target;
    private float maxSpeed = 6f; // world units per second

	// Use this for initialization
	void Start () 
    {
        var pos = target.transform.position;
        pos.z = transform.position.z;
        transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(target != null)
        {
            var path = target.transform.position - transform.position;
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
