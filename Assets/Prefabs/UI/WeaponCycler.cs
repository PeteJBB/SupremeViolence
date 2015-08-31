using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class WeaponCycler : MonoBehaviour
{
    private PlayerControl player;

    private SpriteRenderer left2;
    private SpriteRenderer left;
    private SpriteRenderer center;
    private SpriteRenderer right;
    private SpriteRenderer right2;

    private SpriteRenderer icon; // this icons stays visible after animation

    private Text label;

    // Use this for initialization
    void Start()
    {
        player = GetComponentInParent<PlayerControl>();
        label = GetComponentInChildren<Text>();

        left2 = transform.FindChild("left2").GetComponent<SpriteRenderer>();
        left = transform.FindChild("left").GetComponent<SpriteRenderer>();
        center = transform.FindChild("center").GetComponent<SpriteRenderer>();
        right = transform.FindChild("right").GetComponent<SpriteRenderer>();
        right2 = transform.FindChild("right2").GetComponent<SpriteRenderer>();

        icon = transform.FindChild("icon").GetComponent<SpriteRenderer>();


        left2.color = new Color(1, 1, 1, 0);
        left.color = new Color(1, 1, 1, 0);
        center.color = new Color(1, 1, 1, 0);
        right.color = new Color(1, 1, 1, 0);
        right2.color = new Color(1, 1, 1, 0);

        icon.enabled = true;
    }

    // Update is called once per frame
    public void Cycle(int dir = 1)
    {
        // get icons
        var weapons = player.Pickups.Where(x => x.PickupType == PickupType.Weapon).ToList();
        var current = player.CurrentWeapon;        
        var index = current == null ? 0 : weapons.IndexOf(current);

        left2.sprite = weapons[(index + weapons.Count - 2) % weapons.Count].Icon;
        left.sprite = weapons[(index + weapons.Count - 1) % weapons.Count].Icon;
        center.sprite = weapons[index].Icon;
        right.sprite = weapons[(index + 1) % weapons.Count].Icon;
        right2.sprite = weapons[(index + 2) % weapons.Count].Icon;

        label.text = current.PickupName;
        label.color = new Color(1,1,1,1);

        var moveStep = 0.3f;

        if (dir == 1)
        {
            left2.color = new Color(1, 1, 1, 0.5f);
            left.color = new Color(1, 1, 1, 1);
            center.color = new Color(1, 1, 1, 0.5f);
            right.color = new Color(1, 1, 1, 0);
            right2.color = new Color(1, 1, 1, 0);

            left2.transform.localPosition = new Vector3(-moveStep, left2.transform.localPosition.y, 0);
            left.transform.localPosition = new Vector3(0, left.transform.localPosition.y, 0);
            center.transform.localPosition = new Vector3(moveStep, center.transform.localPosition.y, 0);
            right.transform.localPosition = new Vector3(2 * moveStep, right.transform.localPosition.y, 0);
            right2.transform.localPosition = new Vector3(3 * moveStep, right2.transform.localPosition.y, 0);

            right2.transform.localScale = new Vector3(0, 0, 1);
            right.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            center.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            left.transform.localScale = new Vector3(1, 1, 0);
            left2.transform.localScale = new Vector3(0.6f, 0.6f, 0);
            
            icon.enabled = false;

            iTween.StopByName(gameObject, "weaponCycler");
            iTween.ValueTo(gameObject, iTween.Hash("name", "weaponCycler", "from", 0, "to", 1, "time", 0.75f, "easetype", iTween.EaseType.easeOutCirc, "onupdate", (Action<object>)((obj) =>
            {
                var val = (float)obj;

                // start everything shifted right by one and move it left
                left2.transform.localPosition = new Vector3(-moveStep - (val * moveStep), left2.transform.localPosition.y, 0);
                left.transform.localPosition = new Vector3(0 - (val * moveStep), left.transform.localPosition.y, 0);
                center.transform.localPosition = new Vector3(moveStep - (val * moveStep), center.transform.localPosition.y, 0);
                right.transform.localPosition = new Vector3((2 * moveStep) - (val * moveStep), right.transform.localPosition.y, 0);
                right2.transform.localPosition = new Vector3((3 * moveStep) - (val * moveStep), right2.transform.localPosition.y, 0);

                left2.color = new Color(1, 1, 1, 0.5f - (val * 0.5f));
                left.color = new Color(1, 1, 1, 1 - (val * 0.5f));
                center.color = new Color(1, 1, 1, 0.5f + (val * 0.5f));
                right.color = new Color(1, 1, 1, 0 + (val * 0.5f));
                right2.color = new Color(1, 1, 1, 0);

                right2.transform.localScale = new Vector3(0.3f * val, 0.3f * val, 1);
                right.transform.localScale = new Vector3(0.3f + (0.3f * val), 0.3f + (0.3f * val), 1);
                center.transform.localScale = new Vector3(0.6f + (0.3f * val), 0.6f + (0.3f * val), 1);
                left.transform.localScale = new Vector3(1 - (0.3f * val), 1 - (0.3f * val), 0);
                left2.transform.localScale = new Vector3(0.6f - (0.3f * val), 0.6f - (0.3f * val), 0);

            }), "oncomplete", (Action)(() => 
            {
                // hide icons
                left2.color = new Color(1, 1, 1, 0);
                left.color = new Color(1, 1, 1, 0);
                center.color = new Color(1, 1, 1, 0);
                right.color = new Color(1, 1, 1, 0);
                right2.color = new Color(1, 1, 1, 0);

                label.color = new Color(1,1,1,0);

                icon.sprite = current.Icon;
                icon.enabled = true;
            })));
        }
        else
        {
            // start everything shifted left by one and move it right
            left2.color = new Color(1, 1, 1, 0);
            left.color = new Color(1, 1, 1, 0f);
            center.color = new Color(1, 1, 1, 0.5f);
            right.color = new Color(1, 1, 1, 1);
            right2.color = new Color(1, 1, 1, 0.5f);

            left2.transform.localPosition = new Vector3(-3 * moveStep, left2.transform.localPosition.y, 0);
            left.transform.localPosition = new Vector3(-2 * moveStep, left.transform.localPosition.y, 0);
            center.transform.localPosition = new Vector3(-moveStep, center.transform.localPosition.y, 0);
            right.transform.localPosition = new Vector3(0, right.transform.localPosition.y, 0);
            right2.transform.localPosition = new Vector3(moveStep, right2.transform.localPosition.y, 0);

            left2.transform.localScale = new Vector3(0, 0, 1);
            left.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            center.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            right.transform.localScale = new Vector3(1, 1, 0);
            right2.transform.localScale = new Vector3(0.6f, 0.6f, 0);

            icon.enabled = false;

            iTween.StopByName(gameObject, "weaponCycler");
            iTween.ValueTo(gameObject, iTween.Hash("name", "weaponCycler", "from", 0, "to", 1, "time", 0.75f, "easetype", iTween.EaseType.easeOutCirc, "onupdate", (Action<object>)((obj) =>
            {
                var val = (float)obj;

                left2.transform.localPosition = new Vector3((-3 * moveStep) + (val * moveStep), left2.transform.localPosition.y, 0);
                left.transform.localPosition = new Vector3((-2 * moveStep) + (val * moveStep), left.transform.localPosition.y, 0);
                center.transform.localPosition = new Vector3(-moveStep + (val * moveStep), center.transform.localPosition.y, 0);
                right.transform.localPosition = new Vector3((val * moveStep), right.transform.localPosition.y, 0);
                right2.transform.localPosition = new Vector3(moveStep + (val * moveStep), right2.transform.localPosition.y, 0);

                left2.color = new Color(1, 1, 1, 0);
                left.color = new Color(1, 1, 1, 0 + (val * 0.5f));
                center.color = new Color(1, 1, 1, 0.5f + (val * 0.5f));
                right.color = new Color(1, 1, 1, 1 - (val * 0.5f));
                right2.color = new Color(1, 1, 1, 0.5f - (val * 0.5f));

                left2.transform.localScale = new Vector3(0.3f * val, 0.3f * val, 1);
                left.transform.localScale = new Vector3(0.3f + (0.3f * val), 0.3f + (0.3f * val), 1);
                center.transform.localScale = new Vector3(0.6f + (0.3f * val), 0.6f + (0.3f * val), 1);
                right.transform.localScale = new Vector3(1 - (0.3f * val), 1 - (0.3f * val), 0);
                right2.transform.localScale = new Vector3(0.6f - (0.3f * val), 0.6f - (0.3f * val), 0);

            }), "oncomplete", (Action)(() => 
            {
                // hide icons
                left2.color = new Color(1, 1, 1, 0);
                left.color = new Color(1, 1, 1, 0);
                center.color = new Color(1, 1, 1, 0);
                right.color = new Color(1, 1, 1, 0);
                right2.color = new Color(1, 1, 1, 0);

                label.color = new Color(1,1,1,0);
                icon.sprite = current.Icon;
                icon.enabled = true;
            })));
        }
    }
}