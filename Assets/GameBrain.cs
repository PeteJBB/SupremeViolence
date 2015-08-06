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

    public GameState State = GameState.Startup;
    public bool EnableStartupSequence = true;

    public AudioClip StartupSound;
    public AudioClip StartupSequenceBeginSound;
    public AudioClip GameOnSound;

    private Camera camera1;
    private Camera camera2;

    private PlayerControl player1;
    private PlayerControl player2;

    private float targetOrthSize;

	void Awake () 
    {

	}

    void Start()
    {
        // get players
        var players = GameObject.FindGameObjectsWithTag("Player");
        player1 = players[0].GetComponent<PlayerControl>();
        player2 = players[1].GetComponent<PlayerControl>();
        
        // set each camera to cover half the play area then pan to each player
        var cameras = GameObject.FindGameObjectsWithTag("MainCamera")
            .OrderBy(cam => cam.GetComponent<Camera>().rect.x)
            .ToList();
        
        // disable camera tracking while intro pan is running
        camera1 = cameras[0].GetComponent<Camera>();
        camera2 = cameras[1].GetComponent<Camera>();

        targetOrthSize = camera1.orthographicSize;

        if(StartupSound != null)
            AudioSource.PlayClipAtPoint(StartupSound, Vector3.zero);
        
        if (EnableStartupSequence)
        {
            RunStartupSequence();
        } 
        else
        {
            GameOn();
        }
    }

    void RunStartupSequence()
    {
        camera1.GetComponent<TrackObject>().enabled = false;
        camera2.GetComponent<TrackObject>().enabled = false;
        camera1.GetComponent<PixelPerfectCamera>().enabled = false;
        camera2.GetComponent<PixelPerfectCamera>().enabled = false;

        
        var aspect = (float)Screen.width / Screen.height;
        float baseX = (Arena.Instance.ArenaSizeX + 2) / 4;//  position if width and height are equal
        float aspectAdjustX;
        if (aspect < 1)
        {
            //Debug.Log("Higher than wide");
            var orth = (Arena.Instance.ArenaSizeX + 2) / 2;
            camera1.orthographicSize = orth / aspect;
            camera2.orthographicSize = orth / aspect;
            
            aspectAdjustX = 0;
        } 
        else
        {
            //Debug.Log("Wider than high");
            camera1.orthographicSize = (Arena.Instance.ArenaSizeY + 2) / 2;
            camera2.orthographicSize = (Arena.Instance.ArenaSizeY + 2) / 2;

            // orth size is for y axis so what is it on x?
            var orthX = camera1.orthographicSize * aspect;
            aspectAdjustX = (orthX - camera1.orthographicSize) / 2; // adjusted for aspect ratio
        }
        camera1.transform.position = new Vector3(-baseX - aspectAdjustX, 0, camera1.transform.position.z);
        camera2.transform.position = new Vector3(baseX + aspectAdjustX, 0, camera2.transform.position.z);

        WaitAndThenCall(2, () => PanCameraIntro());
    }

    // Update is called once per frame
	void Update () 
    {

	}

    private void PanCameraIntro()
    {
        var panTime = 3;

        if(StartupSound != null)
            AudioSource.PlayClipAtPoint(StartupSequenceBeginSound, Vector3.zero);

        var mainCanvas = GameObject.FindObjectOfType<MainCanvas>();
        mainCanvas.ShowMessage("Get Ready!", panTime);

        // adjust zoom
        iTween.ValueTo(gameObject, iTween.Hash("from", camera1.orthographicSize, "to", targetOrthSize, "time", panTime, "onupdate", "CamPerspectiveTween", "easetype", iTween.EaseType.easeOutExpo));

        // pan
        var guy1 = camera1.gameObject.GetComponent<TrackObject>().target;
        iTween.MoveTo(camera1.gameObject, iTween.Hash("x", guy1.transform.position.x, "y", guy1.transform.position.y, "time", panTime, "easetype", iTween.EaseType.easeOutExpo));
        var guy2 = camera2.gameObject.GetComponent<TrackObject>().target;
        iTween.MoveTo(camera2.gameObject, iTween.Hash("x", guy2.transform.position.x, "y", guy2.transform.position.y, "time", panTime, "easetype", iTween.EaseType.easeOutExpo, "oncomplete", "GameOn", "oncompletetarget", gameObject));
    }

    void CamPerspectiveTween(float val){
        camera1.orthographicSize = val;
        camera2.orthographicSize = val;
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
        camera1.GetComponent<TrackObject>().enabled = true;
        camera2.GetComponent<TrackObject>().enabled = true;

        camera1.GetComponent<PixelPerfectCamera>().enabled = true;
        camera2.GetComponent<PixelPerfectCamera>().enabled = true;
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
