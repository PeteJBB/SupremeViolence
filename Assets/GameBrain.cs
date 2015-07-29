using UnityEngine;
using System.Collections;
using System;

public class GameBrain : MonoBehaviour 
{
    public static GameBrain Instance;
    public GameState State = GameState.Startup;
    public bool EnableStartupSequence = true;

    public AudioClip StartupSound;
    public AudioClip StartupSequenceBeginSound;
    public AudioClip GameOnSound;

    private Camera camera1;
    private Camera camera2;

    private Arena arena;

    private PlayerControl player1;
    private PlayerControl player2;

	void Awake () 
    {
        GameBrain.Instance = this;  
        arena = Transform.FindObjectOfType<Arena>();

        // get players
        var players = GameObject.FindGameObjectsWithTag("Player");
        player1 = players[0].GetComponent<PlayerControl>();
        player2 = players[1].GetComponent<PlayerControl>();

        // set each camera to cover half the play area then pan to each player
        var cameras = GameObject.FindGameObjectsWithTag("MainCamera");

        // disable camera tracking while intro pan is running
        camera1 = cameras[0].GetComponent<Camera>();
        camera2 = cameras[1].GetComponent<Camera>();

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
        
        
        var aspect = (float)Screen.width / Screen.height;
        float baseX = (arena.ArenaSize.x + 2) / 4;//  position if width and height are equal
        float aspectAdjustX;
        if (aspect < 1)
        {
            Debug.Log("Higher than wide");
            var orth = (arena.ArenaSize.x + 2) / 2;
            camera1.orthographicSize = orth / aspect;
            camera2.orthographicSize = orth / aspect;
            
            aspectAdjustX = 0;
        } 
        else
        {
            Debug.Log("Wider than high");
            camera1.orthographicSize = (arena.ArenaSize.y + 2) / 2;
            camera2.orthographicSize = (arena.ArenaSize.y + 2) / 2;
            
            // orth size is for y axis so what is it on x?
            var orthX = camera1.orthographicSize * aspect;
            aspectAdjustX = (orthX - camera1.orthographicSize) / 2; // adjusted for aspect ratio
        }
        camera1.transform.position = new Vector3(-baseX - aspectAdjustX, 0, camera1.transform.position.z);
        camera2.transform.position = new Vector3(baseX + aspectAdjustX, 0, camera2.transform.position.z);

        WaitAndThenCall(() => PanCameraIntro(), 2);
    }

    // Update is called once per frame
	void Update () 
    {

	}

    private void PanCameraIntro()
    {
        if(StartupSound != null)
            AudioSource.PlayClipAtPoint(StartupSequenceBeginSound, Vector3.zero);

        // adjust zoom
        iTween.ValueTo(gameObject, iTween.Hash("from", camera1.orthographicSize, "to", 4, "time", 3, "onupdate", "CamPerspectiveTween", "easetype", iTween.EaseType.easeOutExpo));

        // pan
        var guy1 = camera1.gameObject.GetComponent<TrackObject>().target;
        iTween.MoveTo(camera1.gameObject, iTween.Hash("x", guy1.transform.position.x, "y", guy1.transform.position.y, "time", 3, "easetype", iTween.EaseType.easeOutExpo));
        var guy2 = camera2.gameObject.GetComponent<TrackObject>().target;
        iTween.MoveTo(camera2.gameObject, iTween.Hash("x", guy2.transform.position.x, "y", guy2.transform.position.y, "time", 3, "easetype", iTween.EaseType.easeOutExpo, "oncomplete", "GameOn", "oncompletetarget", gameObject));
    }

    void CamPerspectiveTween(float val){
        camera1.orthographicSize = val;
        camera2.orthographicSize = val;
    }

    public void WaitAndThenCall(Action funcToRun, float waitSeconds)
    {
        StartCoroutine(CoWaitAndThenCall(funcToRun, waitSeconds));
    }

    public IEnumerator CoWaitAndThenCall(Action funcToRun, float waitSeconds)
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
    }

}

public enum GameState
{
    Startup,
    GameOn,
    GameOver
}
