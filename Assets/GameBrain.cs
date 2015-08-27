using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Game brain runs an individual game, controlling cameras, startup and end game sequences and the scoreboard
/// </summary>
public class GameBrain : Singleton<GameBrain>
{
    public PlayState State = PlayState.GameOn;
    public bool EnableStartupSequence = false;

    public AudioClip StartupSound;
    public AudioClip GetReadySound;
    public AudioClip GameOnSound;

    public GameObject PlayerCameraPrefab;

    private List<Camera> cameras = new List<Camera>();
    private PlayerControl[] players;

    public UnityEvent OnGameOver;

    private bool isFirstUpdate = true;

	void Awake () 
    {
        if(OnGameOver == null)
            OnGameOver = new UnityEvent();

        if(!GameState.IsGameStarted)
            GameState.StartNewGame();
	}

    void Start()
    {
        GameState.StartNewRound();

        CreatePlayerCameras();

        var resultsCanvasObj = GameObject.Find("ResultsCanvas");
        if (resultsCanvasObj != null)
        {
            var resultsCanvas = GameObject.Find("ResultsCanvas").GetComponent<CanvasGroup>();
            resultsCanvas.alpha = 0;
        }

        if (EnableStartupSequence)
        {
            RunStartupSequence();
        } 
        else
        {
            GameOn();
        }
    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("Level was loaded " + level);
    }

    // create player cameras
    void CreatePlayerCameras()
    {
        if (PlayerCameraPrefab == null)
            return;

        for(var i=0; i<GameSettings.NumberOfPlayers; i++)
        {
            var cam = Instantiate(PlayerCameraPrefab).GetComponent<Camera>();
            cameras.Add(cam);
            cam.orthographicSize = 4;
            cam.name = "PlayerCamera" + i;
            var track = cam.GetComponent<TrackPlayer>();
            track.PlayerIndex = i;
            if(i > 0)
                cam.GetComponent<AudioListener>().enabled = false;
            
            // set camera rect
            var w = 0.5f;
            var h = GameSettings.NumberOfPlayers > 2 ? 0.5f : 1;
            var x = (i % 2) * 0.5f;
            var y = GameSettings.NumberOfPlayers < 3 || i > 1
                ? 0f
                    : 0.5f;
            
            cam.rect = new Rect(x,y,w,h);
        }
    }

    void RunStartupSequence()
    {
        Helper.DebugLogTime("RunStartupSequence");
        

        if(StartupSound != null)
            AudioSource.PlayClipAtPoint(StartupSound, Vector3.zero);

        // hide UI
        var uiCanvas = GameObject.FindObjectOfType<PlayerHudCanvas>().GetComponent<CanvasGroup>();
        uiCanvas.alpha = 0;
        var resultsCanvas = GameObject.Find("ResultsCanvas").GetComponent<CanvasGroup>();
        resultsCanvas.alpha = 0;

        // create a new camera which will display the entire play area
        var cam = new GameObject("StartupCamera").AddComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.transform.position = (Arena.Instance.GetArenaSize() / 2).ToVector3(-10);
        cam.orthographic = true;
        cam.orthographicSize = Arena.Instance.GetArenaSize().y / 2f;
        cam.gameObject.AddComponent<GUILayer>();
        cam.depth = 1;

        // 0 - wob
        // 2 - fade in ui
        // 3 - Get Ready / fade to game
        // 5 - go!

        // fade in UI
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "delay", 2, "time", 0.5f, "onupdate", (Action<object>) (newVal =>  
        { 
            uiCanvas.alpha = (float)newVal;
        })));

        // Get ready!
        Helper.Instance.WaitAndThenCall(3, () =>
        {
            PlayerHudCanvas.Instance.ShowMessage("Get Ready!", 2);
            if(GetReadySound != null)
                AudioSource.PlayClipAtPoint(GetReadySound, Vector3.zero);
        });

        // fade cam to black (requires GUILayer)
        iTween.CameraFadeAdd();
        iTween.CameraFadeTo(iTween.Hash("amount", 1, "delay", 3, "time", 0.5f));

        // remove startup cam
        Destroy(cam.gameObject, 3.5f);

        // fade to game view
        iTween.CameraFadeTo(iTween.Hash("amount", 0, "delay", 3.5, "time", 0.5f));

        Helper.Instance.WaitAndThenCall(5, () =>
        {
            GameOn();
        });
    }

    // Update is called once per frame
	void Update () 
    {
        if(State == PlayState.GameOn)
        {
            // check player scores, when limit reached, end the game
            if(players == null)
                players = GameObject.FindObjectsOfType<PlayerControl>().ToArray();

            foreach(var p in players)
            {
                var score = GameState.Players[p.PlayerIndex].RoundScore;
                if(score >= GameSettings.ScoreLimit)
                {
                    GameOver();
                }
            }
        }
	}

    private void GameOn()
    {
        if(EnableStartupSequence && GameOnSound != null)
            AudioSource.PlayClipAtPoint(GameOnSound, Vector3.zero);

        State = PlayState.GameOn;
    }

    private void GameOver()
    {
        PlayerHudCanvas.Instance.ShowMessage("Game Over!", 2);

        // slow mo!
        Time.timeScale = 0.05f;
        var physicsTimeDefault = Time.fixedDeltaTime;
        Time.fixedDeltaTime = physicsTimeDefault * Time.timeScale;
        Helper.Instance.WaitAndThenCall(0.25f, () => { Time.timeScale = 1; Time.fixedDeltaTime = physicsTimeDefault ; });

        // fade to black
        iTween.CameraFadeAdd();
        iTween.CameraFadeTo(iTween.Hash("amount", 1, "delay", 1, "time", 0.5f));

        // fade out UI
        var uiCanvas = GameObject.FindObjectOfType<PlayerHudCanvas>().GetComponent<CanvasGroup>();
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "delay", 1, "time", 0.5f, "onupdate", (Action<object>) (newVal =>  
        { 
            uiCanvas.alpha = (float)newVal;
        })));

        // show results table
        var resultsCanvas = GameObject.Find("ResultsCanvas").GetComponent<CanvasGroup>();
        resultsCanvas.alpha = 0;
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "delay", 2, "time", 0.5f, "onupdate", (Action<object>) (newVal =>                                                                                                           { 
            resultsCanvas.alpha = (float)newVal;
        })));

        var menuController = GetComponent<CustomMenuInputController>();
        Helper.Instance.WaitAndThenCall(2, () =>
        {
            menuController.NavigateForwards(resultsCanvas.GetComponent<Canvas>());
        });


        // create a new camera which will display the entire play area
        var cam = new GameObject("StartupCamera").AddComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.transform.position =  (Arena.Instance.GetArenaSize() / 2).ToVector3(-10);
        cam.orthographic = true;
        cam.orthographicSize = Arena.Instance.GetArenaSize().y / 2f;
        cam.enabled = false;
        cam.gameObject.AddComponent<GUILayer>();
        cam.depth = 1;

        // fade in arena cam
        Helper.Instance.WaitAndThenCall(3, () => { cam.enabled = true; });
        iTween.CameraFadeTo(iTween.Hash("amount", 0, "delay", 3, "time", 0.5f));

        State = PlayState.GameOver;
        OnGameOver.Invoke();
    }

    /// <summary>
    /// Set values ont he results ui prior to displaying at end of match
    /// </summary>
    private void SetResultsValues()
    {
        var resultsCanvas = GameObject.Find("ResultsCanvas").GetComponent<CanvasGroup>();

        // rank players by score
        var playersRanked = GameState.Players.OrderByDescending(x => x.RoundScore).ToList();
        for(var i=0; i<4; i++)
        {
            var row = resultsCanvas.transform.Find("Panel/PlayerResults/PlayerResult" + i).gameObject;
            if(playersRanked.Count > i)
            {
                var pl = playersRanked[i];

                row.SetActive(true);
                row.transform.Find("Color").GetComponent<Image>().color = pl.Color;
                row.transform.Find("Name").GetComponent<Text>().text = "Player " + (pl.PlayerIndex+1);
                row.transform.Find("Kills").GetComponent<Text>().text = pl.RoundScore.ToString();
                row.transform.Find("Winnings").GetComponent<Text>().text = "$" + (pl.RoundScore * GameSettings.CashForKill);
            }
            else
            {
                row.SetActive(false);
            }
        }
    }

    public void GameOverContinue()
    {
        Application.LoadLevelAsync("Shop");
    }

    public static bool IsEditMode()
    {
        return !EditorApplication.isPlaying;
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(Screen.width - 120, Screen.height-40, 100, 20), "Test");
    }
}

/// <summary>
/// The state of play during a match
/// </summary>
public enum PlayState
{
    Startup,
    GameOn,
    GameOver
}
