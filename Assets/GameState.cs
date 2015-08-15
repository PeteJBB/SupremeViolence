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
            Players.Add(pState);
        }

        IsGameStarted = true;
    }
}

public class PlayerState
{
    public string Name;
    public int Score;
    public int Cash;

    public List<PickupState> PickupStates;

    public PlayerState(int playerIndex)
    {
        Name = "Player " + (playerIndex + 1);
        Score = 0;
        Cash = GameSettings.StartingCash;

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