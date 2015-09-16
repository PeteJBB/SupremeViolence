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

    public AudioClip OpenSound;

    private float leftDoorClosedPos;
    private float rightDoorClosedPos;

	void Awake() 
    {
        leftDoor = transform.Find("door_left");
        rightDoor = transform.Find("door_right");

        leftDoorClosedPos = leftDoor.position.x;
        rightDoorClosedPos = rightDoor.position.x;
	}
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            var id = other.gameObject.GetInstanceID();
            if(!objectsInSensor.Contains(id))
                objectsInSensor.Add(id);

            if(!isDoorOpen)
                OpenDoor();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            var id = other.gameObject.GetInstanceID();
            objectsInSensor.Remove(id);
            
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

        AudioSource.PlayClipAtPoint(OpenSound, transform.position);

        isDoorOpen = true;
    }

    void CloseDoor()
    {
        iTween.StopByName(leftDoor.gameObject, "leftdoor");
        iTween.StopByName(rightDoor.gameObject, "rightdoor");
        iTween.MoveTo(leftDoor.gameObject, iTween.Hash("name", "leftdoor", "x", leftDoorClosedPos, "time", 1f));
        iTween.MoveTo(rightDoor.gameObject, iTween.Hash("name", "rightdoor", "x", rightDoorClosedPos, "time", 1f));

        AudioSource.PlayClipAtPoint(OpenSound, transform.position);

        isDoorOpen = false;
    }
}