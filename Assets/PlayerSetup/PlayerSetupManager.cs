using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerSetupManager : MonoBehaviour
{
    [HideInInspector]
    public bool IsFinished = false;

    public void SavePlayerName()
    {
        var playerIndex = GetComponent<PerPlayerUiWindow>().PlayerIndex;
        var name = transform.Find("CanvasName/Name").GetComponent<Text>();
        GameState.Players[playerIndex].Name = name.text;
    }

    public void SavePlayerColor()
    {
        var playerIndex = GetComponent<PerPlayerUiWindow>().PlayerIndex;
        var torso = transform.Find("CanvasColor/Torso").GetComponent<Image>();
        GameState.Players[playerIndex].Color = torso.color;
    }

    public void Finish()
    {
        IsFinished = true;

        var others = GameObject.FindObjectsOfType<PlayerSetupManager>();
        if (others.All(x => x.IsFinished))
        {
            // go to next level
            iTween.CameraFadeAdd();
            iTween.CameraFadeTo(iTween.Hash("amount", 1, "delay", 1, "time", 0.5f, "oncomplete", (System.Action)(() =>
            { 
                Application.LoadLevelAsync("Shop");
            })));
        }
    }
}
