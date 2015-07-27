using UnityEngine;
using System.Collections;
using System;

public class GameBrain : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
        GameBrain.Instance = this;   
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public static GameBrain Instance;
    

    public void WaitAndReactivateObject(GameObject obj, float waitSeconds)
    {
        StartCoroutine(CoWaitAndReactivateObject(obj, waitSeconds));
    }

    private IEnumerator CoWaitAndReactivateObject(GameObject obj, float waitSeconds)
    {
        Debug.Log("Brain waiting...");
        yield return new WaitForSeconds(waitSeconds);
        obj.SetActive(true);
        Debug.Log("Brain done!");
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
