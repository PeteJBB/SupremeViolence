using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Door: MonoBehaviour 
{
    private Transform leftDoor;
    private Transform rightDoor;
    private List<int> objectsInSensor = new List<int>();
    private bool isDoorOpen = false;

	void Start () 
    {
        leftDoor = transform.Find("door_left");
        rightDoor = transform.Find("door_right");
	}
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            objectsInSensor.Add(other.gameObject.GetInstanceID());

            if(!isDoorOpen)
                OpenDoor();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            objectsInSensor.Remove(other.gameObject.GetInstanceID());
            
            if(isDoorOpen && !objectsInSensor.Any())
                CloseDoor();
        }
    }

    void OpenDoor()
    {
        iTween.StopByName(leftDoor.gameObject, "leftdoor");
        iTween.StopByName(rightDoor.gameObject, "rightdoor");
        iTween.MoveTo(leftDoor.gameObject, iTween.Hash("name", "leftdoor", "x", transform.position.x - 0.4f, "time", 1f));
        iTween.MoveTo(rightDoor.gameObject, iTween.Hash("name", "rightdoor", "x", transform.position.x + 0.4f, "time", 1f));

        isDoorOpen = true;
    }

    void CloseDoor()
    {
        iTween.StopByName(leftDoor.gameObject, "leftdoor");
        iTween.StopByName(rightDoor.gameObject, "rightdoor");
        iTween.MoveTo(leftDoor.gameObject, iTween.Hash("name", "leftdoor", "x", transform.position.x, "time", 1f));
        iTween.MoveTo(rightDoor.gameObject, iTween.Hash("name", "rightdoor", "x", transform.position.x, "time", 1f));

        isDoorOpen = false;
    }
}