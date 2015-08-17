using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class LeaderboardCanvas: MonoBehaviour 
{
    public GameObject LeaderboardItemPrefab;
    
    private Transform itemsPanel;
    
    // Use this for initialization
    void Start () 
    {
        itemsPanel = transform.Find("Panel/Items");
        GameBrain.Instance.OnGameOver.AddListener(ShowResults);
    }
    
    public void ShowResults()
    {
        var players = GameState.Players.OrderByDescending(x => x.TotalScore).ToList();
        for(var i=0; i<players.Count; i++)
        {
            var player = players[i];

            var item = Instantiate(LeaderboardItemPrefab);
            item.transform.SetParent(itemsPanel);
            
            item.transform.Find("Pos").GetComponent<Text>().text = (i+1).ToString();
            item.transform.Find("Color").GetComponent<Image>().color = player.Color;
            item.transform.Find("Player").GetComponent<Text>().text = player.Name;
            item.transform.Find("Score").GetComponent<Text>().text = player.TotalScore.ToString();
        }

        var roundsLabel = transform.Find("Panel/Round").GetComponent<Text>();
        if(GameState.CurrentRound == GameSettings.NumberOfRounds)
            roundsLabel.text = "Final Standings";
        if(GameState.CurrentRound == GameSettings.NumberOfRounds - 1)
            roundsLabel.text = "1 round to go!";
        else
        {
            roundsLabel.text = string.Format("{0} rounds to go", GameSettings.NumberOfRounds - GameState.CurrentRound);
        }
    }
}