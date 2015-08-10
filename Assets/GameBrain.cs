using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameBrain : MonoBehaviour 
{
    public static GameBrain Instance
    {
        get { return FindObjectOfType<GameBrain>(); }
    }

    public static int NumberOfPlayers = 2;

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
        var canvasGroup = GameObject.FindObjectOfType<PlayerHudCanvas>().GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        // create a new camera which will display the entire play area
        var cam = new GameObject("StartupCamera").AddComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.transform.position = new Vector3(0,0,-10);
        cam.orthographic = true;
        cam.orthographicSize = Arena.Instance.ArenaSizeY / 2f;

        // 0 - wob
        // 2 - fade in ui
        // 3 - Get Ready / fade to game
        // 5 - go!

        // fade in UI
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "delay", 2, "time", 0.5f, "onupdate", (Action<object>) (newVal =>  
        { 
            canvasGroup.alpha = (float)newVal;
        })));

        // Get ready!
        WaitAndThenCall(3, () =>
        {
            PlayerHudCanvas.Instance.ShowMessage("Get Ready!", 2);
            if(GetReadySound != null)
                AudioSource.PlayClipAtPoint(GetReadySound, Vector3.zero);
        });

        // fade cam to black (requires GUILayer)
        cam.gameObject.AddComponent<GUILayer>();
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
