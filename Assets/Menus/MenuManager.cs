using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour 
{
    private Stack<Canvas> navStack = new Stack<Canvas>();
    public Canvas ActiveCanvas;
    private EventSystem eventSys;

    void Start()
    {
        eventSys = FindObjectOfType<EventSystem>();
    }

	public void NavigateForwards(Canvas canvas)
    {
        navStack.Push(ActiveCanvas);
        ActiveCanvas.gameObject.SetActive(false);
        ActiveCanvas = canvas;
        ActiveCanvas.gameObject.SetActive(true);

        eventSys.SetSelectedGameObject(canvas.transform.GetChild(0).gameObject);
    }

    public void GoBack()
    {
        if(navStack.Count > 0)
        {
            ActiveCanvas.gameObject.SetActive(false);
            ActiveCanvas = navStack.Pop();
            ActiveCanvas.gameObject.SetActive(true);
        }
    }

    public void LoadScene(string sceneName)
    {
        Application.LoadLevelAsync(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

