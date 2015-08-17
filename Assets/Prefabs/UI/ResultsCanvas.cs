using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class ResultsCanvas: MonoBehaviour 
{
    public GameObject PlayerResultPrefab;

    private Transform playerResultsPanel;

	// Use this for initialization
	void Start () 
    {
        playerResultsPanel = transform.Find("Panel/PlayerResults");
        GameBrain.Instance.OnGameOver.AddListener(ShowResults);
	}
	
	public void ShowResults()
    {
        var players = GameState.Players.OrderByDescending(x => x.RoundScore).ToList();
        for(var i=0; i<players.Count; i++)
        {
            var player = players[i];

            var result = Instantiate(PlayerResultPrefab);
            result.transform.SetParent(playerResultsPanel);

            result.transform.Find("Color").GetComponent<Image>().color = player.Color;
            result.transform.Find("Name").GetComponent<Text>().text = player.Name;
            result.transform.Find("Kills").GetComponent<Text>().text = player.RoundScore.ToString();
            result.transform.Find("Winnings").GetComponent<Text>().text = (player.RoundScore * GameSettings.CashForKill).ToString();
        }

        transform.Find("Panel/Round").GetComponent<Text>().text = string.Format("Round {0} of {1}", GameState.CurrentRound, GameSettings.NumberOfRounds);
    }
}