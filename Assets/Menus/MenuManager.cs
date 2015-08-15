using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : MonoBehaviour 
{
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

