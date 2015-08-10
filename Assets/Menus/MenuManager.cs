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
        Debug.Log("LoadScene " + sceneName);
        Application.LoadLevelAsync(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetNumberOfPlayers(int num)
    {
        Debug.Log("SetNumberOfPlayers " + num);
        if(num < 2)
            GameBrain.NumberOfPlayers = 2;
        else if (num > 4)
            GameBrain.NumberOfPlayers = 4;
        else
            GameBrain.NumberOfPlayers = num;
        
    }
}

