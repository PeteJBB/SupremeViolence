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

    private AsyncOperation loadOperation;
    private Text progressText;

    public string[] LevelsToLoad;

    private int currentLoadingIndex = 0;

    void Awake()
    {
        progressText = transform.Find("Canvas/Progress").GetComponent<Text>();
    }
	void Start () 
	{
	    if(LoadImmediately)
        {
            LoadNext();
        }
	}

    void Update()
    {
        if(loadOperation != null)
            progressText.text = string.Format("{0}%", Mathf.RoundToInt(loadOperation.progress * 100));
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
        loadOperation = Application.LoadLevelAdditiveAsync(levelToLoad);        
        yield return loadOperation;

        if(OnLevelLoaded != null)
            OnLevelLoaded.Invoke(levelToLoad);

        currentLoadingIndex++;
        if (currentLoadingIndex < LevelsToLoad.Length)
            LoadNext();
        else
        {
            var group = transform.Find("Canvas").GetComponent<CanvasGroup>();
            iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 0.5f, "onupdate", (System.Action<object>)((obj) =>
            {
                var val = (float)obj;
                group.alpha = val;
            }), 
            "oncomplete", (System.Action)(() =>
            {
                Helper.Instance.WaitAndThenCall(2, () =>
                {
                    if (OnAllLevelsLoaded != null)
                        OnAllLevelsLoaded.Invoke();

                    gameObject.SetActive(false);
                });
            })));
            //iTween.CameraFadeAdd();
            //iTween.CameraFadeTo(iTween.Hash("amount", 1, "delay", 0, "time", 5f, "oncomplete", (System.Action)(() =>
            //{
            //    iTween.CameraFadeDestroy();
            //    transform.Find("Canvas").gameObject.SetActive(false);
            //    if (OnAllLevelsLoaded != null)
            //        OnAllLevelsLoaded.Invoke();    
            //})));

            
        }
    }
}

public class LevelLoadedEvent : UnityEvent<string>
{

}
