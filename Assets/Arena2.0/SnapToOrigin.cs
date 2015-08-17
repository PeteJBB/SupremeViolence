using UnityEngine;
using System.Collections;

public class SnapToOrigin: MonoBehaviour 
{
	// Update is called once per frame
	void LateUpdate () 
    {
        transform.localPosition = Vector3.zero;
	}
}