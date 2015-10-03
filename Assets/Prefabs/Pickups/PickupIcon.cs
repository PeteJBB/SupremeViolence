using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PickupIcon: MonoBehaviour 
{
    public Pickup PickupPrefab;
    private float pickupTime;
    private float respawnTime = 15;
    private bool isHidden = false;
    
    void Start()
    {
        if(PickupPrefab != null && PickupPrefab.Icon != null)
        {
            transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = PickupPrefab.Icon;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!isHidden && other.tag == "Player")
        {
            var player = other.GetComponent<PlayerControl>();
            Pickup(player);
        }
    }

    void Update()
    {
        if (isHidden && Time.time - pickupTime >= respawnTime)
        {
            Respawn();
        }
    }

    private void Pickup(PlayerControl player)
    {
        var instance = Instantiate<Pickup>(PickupPrefab);
        instance.CollectPickup(player);

        pickupTime = Time.time;
        isHidden = true;
        
        var pos = transform.position;
        pos.z = -100;
        transform.position = pos;
        GetComponent<Collider2D>().enabled = false;

        var tracker = GetComponent<GridTrackedObject>();
        tracker.MapColor = new Color(0.5f, 0.5f, 0);
        tracker.UpdateGrid();
    }

    private void Respawn()
    {
        isHidden = false;
        var pos = transform.position;
        pos.z = 0;
        transform.position = pos;
        GetComponent<Collider2D>().enabled = true;

        var tracker = GetComponent<GridTrackedObject>();
        var color = tracker.MapColor;
        color.a = 0.2f;
        tracker.MapColor = new Color(1, 1, 0);
        tracker.UpdateGrid();

    }
}