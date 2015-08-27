using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class AutoTurret: MonoBehaviour 
{
    public GameObject ProjectilePrefab;
    public float FireRate = 1f;
        
    private float lastFireTime;
    //private bool isTriggerDown;

	// Use this for initialization
	void Start () 
    {
        //weapon = Instantiate(WeaponPrefab).GetComponent<Pickup>();
        //weapon.transform.SetParent(transform);
        //weapon.transform.localPosition = Vector3.zero;
        //weapon.transform.localRotation = Quaternion.identity;

        lastFireTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Time.time - lastFireTime > FireRate)
        {
            lastFireTime = Time.time;
            var bullet = Instantiate(ProjectilePrefab);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, 6f), ForceMode2D.Impulse);
        }
        //else if (isTriggerDown)
        //{
        //    isTriggerDown = false;
        //    weapon.OnFireUp(transform.position);
        //}
	}
}