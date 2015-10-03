using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class TeleporterPad: MonoBehaviour 
{
    private SpriteRenderer lights;
    public TeleporterPad ReceivingPad;
    public GameObject TeleportEffectPrefab;
    public AudioClip ActivateSoundClip;

    private float lastTeleportTime = -20;
    
	void Awake () 
    {
        lights = transform.Find("lights").GetComponent<SpriteRenderer>();
	}

	void Update() 
    {
        // normally i'd put this in start but it seems to freak out over old PrefabLoader instances, so...
        if (ReceivingPad == null)
        {
            var allOtherPads = Transform.FindObjectsOfType<TeleporterPad>().Where(x => x != null && x != this && x.ReceivingPad == null).ToList();
            ReceivingPad = allOtherPads.FirstOrDefault();            
            
            if (ReceivingPad != null)
            {
                ReceivingPad.ReceivingPad = this;
            }
            else
            {
                 Debug.Log("Cant find matching teleporter pad.. destroying");
                 Destroy(gameObject);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (ReceivingPad != null && collider.tag == "Player" && Time.time - lastTeleportTime > 4)
        {
            SendTeleport(collider.gameObject);
            ReceivingPad.ReceiveTeleport(collider.gameObject);
        }
    }

    public void SendTeleport(GameObject obj)
    {
        var player = obj.GetComponent<PlayerControl>();
        if (player != null)
            player.FreezeControl = true;

        var effect = Instantiate(TeleportEffectPrefab, transform.position, Quaternion.identity);
        Helper.PlaySoundEffect(ActivateSoundClip);

        // get object into center
        iTween.MoveTo(obj, iTween.Hash("position", transform.position, "time", 1f));

        // tween lights
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 1, "onupdate", (Action<object>) (newVal =>  
        {
            var val = (float)newVal;
            lights.color = new Color(1, 1, 1, val);
        }), "oncomplete", (Action) (() =>  
        { 
            // teleport guy
            obj.transform.position = ReceivingPad.transform.position;  
          
            // tween back to original state
            iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "delay", 2, "time", 1, "onupdate", (Action<object>)(newVal =>
            {
                var val = (float)newVal;
                lights.color = new Color(1, 1, 1, val);
            })));
        })));

        lastTeleportTime = Time.time;
    }

    public void ReceiveTeleport(GameObject obj)
    {
        var effect = Instantiate(TeleportEffectPrefab, transform.position, Quaternion.identity);

        // tween lights
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 1, "onupdate", (Action<object>) (newVal =>  
        {
            var val = (float)newVal;
            lights.color = new Color(1, 1, 1, val);
        }), "oncomplete", (Action) (() =>  
        { 
            // tween back to original state
            iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "delay", 2, "time", 1, "onupdate", (Action<object>)(newVal =>
            {
                var val = (float)newVal;
                lights.color = new Color(1, 1, 1, val);
            }), "oncomplete", (Action)(() =>
            {
                var player = obj.GetComponent<PlayerControl>();
                if (player != null)
                    player.FreezeControl = false;
            })));
        })));

        lastTeleportTime = Time.time;
    }
}