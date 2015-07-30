using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PlayerHud : MonoBehaviour 
{
    public PlayerControl Player;

    private Damageable damageable;
    private Transform healthBar;
    private Transform healthBarBorder;
    private Text weaponLabel;

	// Use this for initialization
	void Start () 
    {
        damageable = Player.GetComponent<Damageable>();
        healthBar = transform.FindChild("HealthBar");
        healthBarBorder = transform.FindChild("HealthBarBorder");
        weaponLabel = transform.FindChild("WeaponLabel").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        var health = damageable.health / damageable.startingHealth;
        healthBar.localScale = new Vector3(health, 1, 1);

        if(Player.CurrentWeapon == null)
            weaponLabel.text = "";
        else
        {
            weaponLabel.text = Player.CurrentWeapon.Name;
            var ammo = Player.CurrentWeapon.GetAmmoCount();
            if(ammo >= 0)
                weaponLabel.text += " " + ammo;
        }

	}
}
