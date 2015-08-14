using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PickupIcon: MonoBehaviour 
{
    [HideInInspector]
    public Pickup PickupPrefab;

    void Start()
    {
        if(PickupPrefab != null && PickupPrefab.Icon != null)
        {
            transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = PickupPrefab.Icon;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            var player = other.GetComponent<PlayerControl>();
            var instance = Instantiate<Pickup>(PickupPrefab);
            instance.CollectPickup(player);
            Destroy(gameObject);
        }
    }
}