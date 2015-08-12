using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Linq;

public class OptionCycler: MonoBehaviour, ISubmitHandler, IMoveHandler, IPointerClickHandler
{
    Text valText;
    public string SettingName;
    public string DisplayFormat;

    void Start()
    {
        valText = GetComponent<Text>();
    }

    void Update()
    {
        var val = GameSettings.GetValue(SettingName);
        if(val == null)
            valText.text = string.Empty;

        if(string.IsNullOrEmpty(DisplayFormat))
            valText.text = val.ToString();
        else
            valText.text = string.Format(DisplayFormat, val);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        GameSettings.IncrementValue(SettingName, 1, true).ToString();
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        var dir = eventData.button == PointerEventData.InputButton.Left ? 1 : -1;
        GameSettings.IncrementValue(SettingName, dir, true).ToString();
    }

    public void OnMove(AxisEventData eventData)
    {
        if(eventData.moveDir == MoveDirection.Left)
        {
            GameSettings.IncrementValue(SettingName, -1).ToString();
        }
        if(eventData.moveDir == MoveDirection.Right)
        {
            GameSettings.IncrementValue(SettingName).ToString();
        }
    }
}