using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Meds: Pickup 
{
    public AudioClip ActivateSound;
    public AudioClip CompleteSound;

    private AudioSource activateSoundSource;

    private bool isActivated = false;
    private float activateTime;
    private float chargeDuration = 5;
    private bool medsHaveBeenApplied = false;

    public override string GetDescription()
    {
        return "Gauze, bandages and plasters to cover up your boo-boos crammed into a lunch box with a whole packet of Flintstones' chewable morphine. ";
    }

    void Awake()
    {
        activateSoundSource = gameObject.AddComponent<AudioSource>();
        activateSoundSource.clip = ActivateSound;
        activateSoundSource.playOnAwake = false;
    }

    void Update()
    {
        if (Player != null)
        {
            if (isActivated && !medsHaveBeenApplied && Time.time - activateTime >= chargeDuration)
            {
                // heal player!
                var dam = Player.GetComponent<Damageable>();
                dam.SetHealth(dam.StartingHealth);
                medsHaveBeenApplied = true;
                AudioSource.PlayClipAtPoint(CompleteSound, transform.position);
                Ammo--;
            }
            else
            {
                var gamepadState = Helper.GetGamePadInput(Player.PlayerIndex);
                if (Ammo > 0 && gamepadState.Buttons.X == ButtonState.Pressed && !isActivated)
                {
                    ActivateMeds();
                }
                else if (gamepadState.Buttons.X == ButtonState.Released && isActivated)
                {
                    DeactivateMeds();
                }
            }
        }
    }

    private void ActivateMeds()
    {
        isActivated = true;
        medsHaveBeenApplied = false;
        activateTime = Time.time;
        Player.SetAmmoBarSource(this, new FillBarColorPoint[] { new FillBarColorPoint(new Color(1,1,1), 0), new FillBarColorPoint(new Color(0, 1, 0), 1) });
        activateSoundSource.Play();
    }

    private void DeactivateMeds()
    {
        isActivated = false;
        Player.RestoreAmmoBarSource();
        activateSoundSource.Stop();
    }

    public override float GetAmmoBarValue()
    {
        if (medsHaveBeenApplied)
            return 1;
        
        if (isActivated)
            return (Time.time - activateTime) / chargeDuration;
        
        return 0;
    }

    public override float GetLegStrengthMultiplier()
    {
        return isActivated && !medsHaveBeenApplied
            ? 0.5f
            : 1;
    }
}