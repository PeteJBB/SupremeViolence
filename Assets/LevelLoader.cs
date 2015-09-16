using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelLoader : MonoBehaviour 
{
    public bool LoadImmediately = true;
    public LevelLoadedEvent OnLevelLoaded;
    public UnityEvent OnAllLevelsLoaded;

    public string[] LevelsToLoad;

    private int currentLoadingIndex = 0;

	void Start () 
	{
	    if(LoadImmediately)
        {
            LoadNext();
        }
	}
    
    public void LoadNext()
    {
        if(currentLoadingIndex < LevelsToLoad.Length)
        {
            var level = LevelsToLoad[currentLoadingIndex];
            StartCoroutine(LoadLevelAdditiveCoroutine(level));    
        }
    }

    public void LoadLevel(string levelToLoad)
    {
        StartCoroutine(LoadLevelAdditiveCoroutine(levelToLoad));     
    }

    public IEnumerator LoadLevelAdditiveCoroutine(string levelToLoad)
    {
        //if (OnLevelWillBeLoaded != null)
        //    OnLevelWillBeLoaded();

        Debug.Log("Loading map " + levelToLoad);
        yield return Application.LoadLevelAdditiveAsync(levelToLoad);

        if(OnLevelLoaded != null)
            OnLevelLoaded.Invoke(levelToLoad);

        currentLoadingIndex++;
        if(currentLoadingIndex < LevelsToLoad.Length)
            LoadNext();
        else if(OnAllLevelsLoaded != null)
            OnAllLevelsLoaded.Invoke();

    }
}

public class LevelLoadedEvent : UnityEvent<string>
{

}
