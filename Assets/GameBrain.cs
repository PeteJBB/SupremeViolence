using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class GameBrain : MonoBehaviour 
{
    public static GameBrain Instance
    {
        get { return FindObjectOfType<GameBrain>(); }
    }

    public static int NumberOfPlayers = 2;
    public static int ScoreLimit = 1;

    public GameState State = GameState.Startup;
    public bool EnableStartupSequence = true;

    public AudioClip StartupSound;
    public AudioClip GetReadySound;
    public AudioClip GameOnSound;

    public GameObject PlayerCameraPrefab;

    private List<Camera> cameras = new List<Camera>();
    private PlayerControl[] players;

	void Awake () 
    {

	}

    void Start()
    {
        if(StartupSound != null)
            AudioSource.PlayClipAtPoint(StartupSound, Vector3.zero);

        CreatePlayerCameras();

        var resultsCanvas = GameObject.Find("ResultsCanvas").GetComponent<CanvasGroup>();
        resultsCanvas.alpha = 0;

        if (EnableStartupSequence)
        {
            RunStartupSequence();
        } 
        else
        {
            GameOn();
        }
    }

    // create player cameras
    void CreatePlayerCameras()
    {
        for(var i=0; i<NumberOfPlayers; i++)
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
            var h = 2f / GameBrain.NumberOfPlayers;
            var x = (i % 2) * 0.5f;
            var y = NumberOfPlayers < 3 || i > 1
                ? 0f
                    : 0.5f;
            
            cam.rect = new Rect(x,y,w,h);
        }
    }

    void RunStartupSequence()
    {
        // hide UI
        var uiCanvas = GameObject.FindObjectOfType<PlayerHudCanvas>().GetComponent<CanvasGroup>();
        uiCanvas.alpha = 0;
        var resultsCanvas = GameObject.Find("ResultsCanvas").GetComponent<CanvasGroup>();
        resultsCanvas.alpha = 0;

        // create a new camera which will display the entire play area
        var cam = new GameObject("StartupCamera").AddComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.transform.position = new Vector3(0,0,-10);
        cam.orthographic = true;
        cam.orthographicSize = Arena.Instance.ArenaSizeY / 2f;
        cam.gameObject.AddComponent<GUILayer>();

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
        WaitAndThenCall(3, () =>
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

        WaitAndThenCall(5, () =>
        {
            GameOn();
        });
    }

    // Update is called once per frame
	void Update () 
    {
        if(State == GameState.GameOn)
        {
            // check player scores, when limit reached, end the game
            if(players == null)
                players = GameObject.FindObjectsOfType<PlayerControl>().ToArray();

            foreach(var p in players)
            {
                if(p.Score >= ScoreLimit)
                {
                    GameOver();
                }
            }
        }
	}

    public void WaitAndThenCall(float waitSeconds, Action funcToRun)
    {
        StartCoroutine(CoWaitAndThenCall(waitSeconds, funcToRun));
    }

    public IEnumerator CoWaitAndThenCall(float waitSeconds, Action funcToRun)
    {
        yield return new WaitForSeconds(waitSeconds);
        funcToRun();
    }

    private void GameOn()
    {
        if(GameOnSound != null)
            AudioSource.PlayClipAtPoint(GameOnSound, Vector3.zero);

        State = GameState.GameOn;
    }

    private void GameOver()
    {
        PlayerHudCanvas.Instance.ShowMessage("Game Over!", 2);

        // slow mo!
        Time.timeScale = 0.05f;
        var physicsTimeDefault = Time.fixedDeltaTime;
        Time.fixedDeltaTime = physicsTimeDefault * Time.timeScale;
        WaitAndThenCall(0.25f, () => { Time.timeScale = 1; Time.fixedDeltaTime = physicsTimeDefault ; });

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
        SetResultsValues();
        var resultsCanvas = GameObject.Find("ResultsCanvas").GetComponent<CanvasGroup>();
        var eventSys = FindObjectOfType<EventSystem>();
        eventSys.SetSelectedGameObject(resultsCanvas.transform.Find("Panel/Continue").gameObject);

        // fade in results
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "delay", 2, "time", 0.5f, "onupdate", (Action<object>) (newVal =>                                                                                                           { 
            resultsCanvas.alpha = (float)newVal;
        })));


        // create a new camera which will display the entire play area
        var cam = new GameObject("StartupCamera").AddComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.transform.position = new Vector3(0,0,-10);
        cam.orthographic = true;
        cam.orthographicSize = Arena.Instance.ArenaSizeY / 2f;
        cam.enabled = false;
        cam.gameObject.AddComponent<GUILayer>();

        // fade in arena cam
        WaitAndThenCall(3, () => { cam.enabled = true; });
        iTween.CameraFadeTo(iTween.Hash("amount", 0, "delay", 3, "time", 0.5f));
        

        State = GameState.GameOver;
    }

    /// <summary>
    /// Set values ont he results ui prior to displaying at end of match
    /// </summary>
    private void SetResultsValues()
    {
        var resultsCanvas = GameObject.Find("ResultsCanvas").GetComponent<CanvasGroup>();

        // rank players by score
        var playersRanked = players.OrderByDescending(x => x.Score).ToList();
        for(var i=0; i<4; i++)
        {
            var row = resultsCanvas.transform.Find("Panel/PlayerResults/PlayerResult" + i).gameObject;
            if(playersRanked.Count > i)
            {
                var pl = playersRanked[i];

                row.SetActive(true);
                row.transform.Find("Color").GetComponent<Image>().color = pl.transform.Find("Torso").GetComponent<SpriteRenderer>().color;
                row.transform.Find("Name").GetComponent<Text>().text = "Player " + (pl.PlayerIndex+1);
                row.transform.Find("Kills").GetComponent<Text>().text = pl.Score.ToString();
                row.transform.Find("Winnings").GetComponent<Text>().text = "$" + (pl.Score * 500);
            }
            else
            {
                row.SetActive(false);
            }
        }
    }

    public static bool IsEditMode()
    {
        return !EditorApplication.isPlaying;
    }
}

public enum GameState
{
    Startup,
    GameOn,
    GameOver
}
