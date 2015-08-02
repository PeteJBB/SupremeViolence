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
    private Text score;

	// Use this for initialization
	void Start () 
    {
        damageable = Player.GetComponent<Damageable>();
        healthBar = transform.FindChild("HealthBar");
        healthBarBorder = transform.FindChild("HealthBarBorder");
        weaponLabel = transform.FindChild("WeaponLabel").GetComponent<Text>();
        score = transform.FindChild("Score").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        var health = Mathf.Clamp(damageable.health / damageable.startingHealth, 0, 1);
        healthBar.localScale = new Vector3(health, 1, 1);

        if(Player.CurrentWeapon == null)
            weaponLabel.text = "";
        else
        {
            weaponLabel.text = Player.CurrentWeapon.Name;
            var ammo = Player.CurrentWeapon.GetAmmoCount();
            if(ammo != int.MaxValue)
                weaponLabel.text += " " + ammo;
        }

        score.text = Player.Score.ToString();
	}
}
