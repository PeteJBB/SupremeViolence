using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class GameState 
{
    public static List<PlayerState> Players;
    public static int CurrentRound;
    public static bool IsGameStarted = false;

    public static void StartNewGame()
    {
        Debug.Log("GameState StartNewGame");

        CurrentRound = 0;
        Players = new List<PlayerState>();
        for(var i=0; i<GameSettings.NumberOfPlayers; i++)
        {
            var pState = new PlayerState(i);
            switch(i)
            {
                case 0:
                default:
                    pState.Color = new Color(.2f, .4f, 1); // blue
                    break;
                case 1:
                    pState.Color = Color.red;
                    break;
                case 2:
                    pState.Color = Color.green;
                    break;
                case 3:
                    pState.Color = new Color(1,0,1); // purple
                    break;
            }
            Players.Add(pState);
        }

        AudioListener.volume = GameSettings.SoundVolume;

        IsGameStarted = true;
    }

    public static void StartNewRound()
    {
        foreach(var p in Players)
        {
            p.RoundScore = 0;
        }

        CurrentRound++;
    }
}

public class PlayerState
{
    public int PlayerIndex;
    public string Name;
    public int TotalScore;
    public int RoundScore;
    public int Cash;
    public Color Color;

    public List<PickupState> PickupStates;

    public PlayerState(int playerIndex)
    {
        PlayerIndex = playerIndex;
        Name = "Player " + (playerIndex + 1);
        Cash = GameSettings.StartingCash;
        Color = Color.white;
        TotalScore = 0;
        RoundScore = 0;

        PickupStates = new List<PickupState>();
        PickupStates.Add(PickupState.FromPrefab(GameSettings.PickupPrefabs.First(x => x.PickupName == "Pistol")));

        if(GameSettings.StartWeapon.PickupName != "Pistol")
            PickupStates.Add(PickupState.FromPrefab(GameSettings.StartWeapon));
    }
}

public class PickupState
{
    public string Name;
    public int Ammo;

    public Pickup GetPrefab()
    {
        var prefab = GameSettings.PickupPrefabs.FirstOrDefault(x => x.PickupName == Name);
        if(prefab == null)
            Debug.LogError("Couldnt find prefab with name '" + Name + "'");

        return prefab;
    }

    public Pickup Instantiate()
    {
        var pickup = GameObject.Instantiate<Pickup>(GetPrefab());
        pickup.Ammo = Ammo;
        return pickup;
    }

    public static PickupState FromPrefab(Pickup prefab)
    {
        return new PickupState()
        {
            Name = prefab.PickupName,
            Ammo = prefab.Ammo
        };
    }

    public static PickupState FromPickupInstance(Pickup instance)
    {
        var state = new PickupState();
        state.Ammo = instance.Ammo;
        state.Name = instance.PickupName;
        return state;
    }
}