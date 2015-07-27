using UnityEngine;
using System.Collections;

public class TrackObject : MonoBehaviour {

    public GameObject target;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(target != null)
        {
            var pos = this.transform.position;
            pos.x = target.transform.position.x;
            pos.y = target.transform.position.y;
            this.transform.position = pos;
        }
	}
}
