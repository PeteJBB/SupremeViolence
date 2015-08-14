using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState 
{
    public static List<PlayerState> Players;
    public static int CurrentRound;
    public static bool IsGameStarted = false;

    public static void StartNewGame()
    {
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

    public List<Pickup> Pickups;

    public PlayerState(int playerIndex)
    {
        Name = "Player " + (playerIndex + 1);
        Score = 0;
        Cash = GameSettings.StartingCash;
        Pickups = new List<Pickup>();
        Pickups.Add(GameSettings.StartWeapon);
    }
}