using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PlayerHud : MonoBehaviour 
{
    public int PlayerIndex;
    private PlayerControl player;

    private Transform healthBar;
    private Transform healthBarBorder;
    private Text weaponLabel;
    private Text score;

	// Use this for initialization
	void Start () 
    {
        healthBar = transform.FindChild("HealthBar");
        healthBarBorder = transform.FindChild("HealthBarBorder");
        weaponLabel = transform.FindChild("WeaponLabel").GetComponent<Text>();
        score = transform.FindChild("Score").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(player == null)
        {
            var players = FindObjectsOfType<PlayerControl>();
            player = players.FirstOrDefault(x => (int)x.PlayerIndex == PlayerIndex);

        }

        if(player != null)
        {
            var damageable = player.GetComponent<Damageable>();
            var health = Mathf.Clamp(damageable.Health / damageable.StartingHealth, 0, 1);
            healthBar.localScale = new Vector3(health, 1, 1);

            if(player.CurrentWeapon == null)
                weaponLabel.text = "";
            else
            {
                weaponLabel.text = player.CurrentWeapon.Name;
                var ammo = player.CurrentWeapon.GetAmmoCount();
                if(ammo != int.MaxValue)
                    weaponLabel.text += " " + ammo;
            }

            score.text = player.Score.ToString();
        }
	}
}
