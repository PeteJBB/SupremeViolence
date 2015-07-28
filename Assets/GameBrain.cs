using UnityEngine;
using System.Collections;
using System;

public class GameBrain : MonoBehaviour 
{
    public static GameBrain Instance;
    public GameState State = GameState.Startup;

    public Camera Camera1;
    public Camera Camera2;

	void Awake () 
    {
        GameBrain.Instance = this;  

        // set each camera to cover half the play area then pan to each player
        var cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        var arena = Transform.FindObjectOfType<Arena>();

        // disable camera tracking while intro pan is running
        Camera1 = cameras[0].GetComponent<Camera>();
        Camera1.GetComponent<TrackObject>().enabled = false;
        Camera2 = cameras[1].GetComponent<Camera>();
        Camera2.GetComponent<TrackObject>().enabled = false;


        var aspect = (float)Screen.width / Screen.height;
        float baseX = (arena.ArenaSize.x + 2) / 4;//  position if width and height are equal
        float aspectAdjustX;
        if(aspect < 1)
        {
            Debug.Log("Higher than wide");
            var orth = (arena.ArenaSize.x + 2) / 2;
            Camera1.orthographicSize = orth / aspect;
            Camera2.orthographicSize = orth / aspect;

            aspectAdjustX = 0;
        }
        else
        {
            Debug.Log("Wider than high");
            Camera1.orthographicSize = (arena.ArenaSize.y + 2) / 2;
            Camera2.orthographicSize = (arena.ArenaSize.y + 2) / 2;

            // orth size is for y axis so what is it on x?
            var orthX = Camera1.orthographicSize * aspect;
            aspectAdjustX = (orthX - Camera1.orthographicSize) / 2; // adjusted for aspect ratio
        }
        Camera1.transform.position = new Vector3(-baseX - aspectAdjustX, 0, Camera1.transform.position.z);
        Camera2.transform.position = new Vector3(baseX + aspectAdjustX, 0, Camera2.transform.position.z);

        WaitAndThenCall(() => PanCameraIntro(), 2);
	}

    // Update is called once per frame
	void Update () 
    {

	}

    private void PanCameraIntro()
    {
        var ht = new Hashtable();
        ht.Add("from", Camera1.orthographicSize);
        ht.Add("to", 4);
        ht.Add("time", 4);
        ht.Add("onupdate", "CamPerspectiveTween");
        ht.Add("easetype", iTween.EaseType.linear);
        iTween.ValueTo(gameObject, ht);

        var guy1 = Camera1.gameObject.GetComponent<TrackObject>().target;
        iTween.MoveTo(Camera1.gameObject, iTween.Hash("x", guy1.transform.position.x, "y", guy1.transform.position.y, "time", 4));
        var guy2 = Camera2.gameObject.GetComponent<TrackObject>().target;
        iTween.MoveTo(Camera2.gameObject, iTween.Hash("x", guy2.transform.position.x, "y", guy2.transform.position.y, "time", 4));
    }

    void CamPerspectiveTween(float val){
        Camera1.orthographicSize = val;
        Camera2.orthographicSize = val;
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


}

public enum GameState
{
    Startup,
    GameOn,
    GameOver
}
