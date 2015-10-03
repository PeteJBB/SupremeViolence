using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoorV: MonoBehaviour 
{
	private Transform topDoor;
    private Transform bottomDoor;
    private Transform doorEdge;

    private List<int> objectsInSensor = new List<int>();
    private bool isDoorOpen = false;
    private SpriteRenderer topDoorSpriteRenderer;

    public AudioClip OpenSound;

	void Awake() 
    {
        topDoor = transform.Find("door_top");
        bottomDoor = transform.Find("door_bottom");
        doorEdge = transform.Find("door_edge");

        topDoorSpriteRenderer = topDoor.GetComponent<SpriteRenderer>();
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
            objectsInSensor.Remove(other.gameObject.GetInstanceID());
            
            if(isDoorOpen && !objectsInSensor.Any())
                CloseDoor();
        }
    }

    void Update()
    {
        // keep edge aligned with top door as it scales
        var pos = doorEdge.transform.position;
        pos.y = topDoorSpriteRenderer.bounds.min.y;
        doorEdge.transform.position = pos;
    }

    void OpenDoor()
    {
        iTween.StopByName(topDoor.gameObject, "topDoor");
        iTween.StopByName(bottomDoor.gameObject, "bottomDoor");
        iTween.StopByName(doorEdge.gameObject, "doorEdge");

        iTween.ScaleTo(topDoor.gameObject, iTween.Hash("name", "topDoor", "y", 0.05f, "time", 1f));
        iTween.ScaleTo(bottomDoor.gameObject, iTween.Hash("name", "bottomDoor", "y", 0.05f, "time", 1f));

        Helper.PlaySoundEffect(OpenSound);
        
        isDoorOpen = true;
    }

    void CloseDoor()
    {
        iTween.StopByName(topDoor.gameObject, "topDoor");
        iTween.StopByName(bottomDoor.gameObject, "bottomDoor");
        iTween.StopByName(doorEdge.gameObject, "doorEdge");

        iTween.ScaleTo(topDoor.gameObject, iTween.Hash("name", "topDoor", "y", 1, "time", 1f));
        iTween.ScaleTo(bottomDoor.gameObject, iTween.Hash("name", "bottomDoor", "y", 1, "time", 1f));

        Helper.PlaySoundEffect(OpenSound);

        isDoorOpen = false;
    }
}