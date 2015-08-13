using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : MonoBehaviour 
{
    private Stack<Canvas> navStack = new Stack<Canvas>();
    public Canvas ActiveCanvas;
    private EventSystem eventSys;
    public GameObject lastSelected;

    void Start()
    {
        eventSys = FindObjectOfType<EventSystem>();
    }

    void Update()
    {
        if(eventSys.currentSelectedGameObject == null)
        {
            eventSys.SetSelectedGameObject(lastSelected);
            lastSelected = null;
        }
        else if(lastSelected == null)
        {
            lastSelected = eventSys.currentSelectedGameObject;
        }
    }

	public void NavigateForwards(Canvas canvas)
    {
        navStack.Push(ActiveCanvas);
        ActiveCanvas.gameObject.SetActive(false);
        ActiveCanvas = canvas;
        ActiveCanvas.gameObject.SetActive(true);

        SelectFirstVisibleItem();
    }

    public void GoBack()
    {
        if(navStack.Count > 0)
        {
            ActiveCanvas.gameObject.SetActive(false);
            ActiveCanvas = navStack.Pop();
            ActiveCanvas.gameObject.SetActive(true);
            SelectFirstVisibleItem();
        }
    }

    private void SelectFirstVisibleItem()
    {
        var selectables = ActiveCanvas.GetComponentsInChildren<Selectable>();
        var first = selectables.OrderByDescending(x => x.GetComponent<RectTransform>().position.y).FirstOrDefault();
        eventSys.SetSelectedGameObject(first.gameObject);
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


}

