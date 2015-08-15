using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Linq;

public class OptionCycler: CustomSelectable, ISubmitHandler, IMoveHandler
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

    public override Selectable FindSelectableOnLeft()
    {
        GameSettings.IncrementValue(SettingName, -1).ToString();
        return this;
    }

    public override Selectable FindSelectableOnRight()
    {
        GameSettings.IncrementValue(SettingName).ToString();
        return this;
    }
}